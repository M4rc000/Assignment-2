using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Networking;
using TMPro;

public class PlayerData
{
    public string playerName;
    public int score;
}

public class SaveLoadManager : MonoBehaviour
{
    public TMPro.TMP_InputField Username;
    public TMPro.TMP_InputField HighScore;
    public TextMeshProUGUI  ShowText;

    private PlayerData playerData;

    private void Start()
    {
        playerData = new PlayerData();
    }

    public void SavePlayerData()
    {
        // Get the player name and score from the input fields
        playerData.playerName = Username.text;
        playerData.score = int.Parse(HighScore.text);

        // Create an XML serializer to serialize the player data
        XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));

        // Create a file stream to write the serialized data to
        FileStream fileStream = new FileStream(Application.dataPath + "/playerdata.xml", FileMode.Create);

        // Serialize the player data to XML and write it to the file stream
        serializer.Serialize(fileStream, playerData);

        // Close the file stream
        fileStream.Close();

        // Update the UI with the saved player data
        ShowText.text = "Player Name: " + playerData.playerName + "\n" +
                              "Score: " + playerData.score;
    }

    public void SendPlayerDataToServer()
    {
        // Load the player data from the XML file
        XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
        FileStream fileStream = new FileStream(Application.dataPath + "/playerdata.xml", FileMode.Open);
        playerData = (PlayerData)serializer.Deserialize(fileStream);
        fileStream.Close();

        // Create a web request to send the player data to the server
        UnityWebRequest webRequest = UnityWebRequest.Post("http://localhost:8080/2023/PlayerData.php", "");
        webRequest.SetRequestHeader("Content-Type", "application/xml");

        // Convert the player data to an XML string and set it as the web request data
        string playerDataXml = SerializeObjectToXml(playerData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(playerDataXml);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // Send the web request and handle the response
        StartCoroutine(HandleServerResponse(webRequest));
    }

    private IEnumerator HandleServerResponse(UnityWebRequest webRequest)
    {
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Server response: " + webRequest.downloadHandler.text);
        }
        else
        {
            Debug.Log("Server error: " + webRequest.error);
        }
    }

    private string SerializeObjectToXml(object obj)
    {
        XmlSerializer serializer = new XmlSerializer(obj.GetType());
        StringWriter stringWriter = new StringWriter();
        serializer.Serialize(stringWriter, obj);
        return stringWriter.ToString();
    }
}