<?php

// Get the XML data from the HTTP request body
$xmlData = file_get_contents('C:\xampp\htdocs\2023\Assignment 2\My project\Assets\playerdata.xml');

// Parse the XML data into a PHP object
$playerData = simplexml_load_string($xmlData);

// Extract the player name and score from the PHP object
$playerName = $playerData->playerName;
$score = $playerData->score;

// Save the player data to a file on the server
$filename = "playerdata.xml";
$file = fopen($filename, "a");
fwrite($file, "$playerName,$score\n");
fclose($file);

// Send a response back to the Unity game
echo "Received player data: $playerName,$score";

?>