using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCleaner : MonoBehaviour
{
    public static MapCleaner Instance;

    // List for each type of spawned object (players, lines, powerups)
    private List<GameObject> listOfSpawnedLines = new List<GameObject>();
    private List<GameObject> listOfSpawnedPlayers = new List<GameObject>();
    private List<GameObject> listOfSpawnedPowerups = new List<GameObject>();
    private void Awake()
    {
        Instance = this;
    }

    // Function used to clear the map before the new game is set up to clear all players, lines and powerups from the map
    public void ClearMap()
    {
        if(listOfSpawnedLines.Count > 0)
        {
            foreach (GameObject spawnedObject in listOfSpawnedLines)
            {
                Destroy(spawnedObject);
            }
            listOfSpawnedLines.Clear();
        }
        if (listOfSpawnedPlayers.Count > 0)
        {
            foreach (GameObject spawnedObject in listOfSpawnedPlayers)
            {
                Destroy(spawnedObject);
            }
            PowerupManager.Instance.ClearListOfPlayers();
            listOfSpawnedPlayers.Clear();
        }
        if(listOfSpawnedPowerups.Count > 0)
        {
            foreach(GameObject gameObject in listOfSpawnedPowerups)
            {
                Destroy(gameObject);
            }
            listOfSpawnedPowerups.Clear();
        }
    }

    // Function used by the ClearMap powerup
    public void ClearAllLines()
    {
        if (listOfSpawnedLines.Count > 0)
        {
            foreach (GameObject spawnedObject in listOfSpawnedLines)
            {
                Destroy(spawnedObject);
            }
            listOfSpawnedLines.Clear();
        }
    }
    public void AddPlayerToMapCleaner(GameObject playerGameObject)
    {
        listOfSpawnedPlayers.Add(playerGameObject);
    }
    public void AddPowerupToMapCleaner(GameObject powerupGameObject)
    {
        listOfSpawnedPowerups.Add(powerupGameObject);
    }
    public void AddToListOfSpawnedLines(GameObject gameObjectToAdd)
    {
        listOfSpawnedLines.Add(gameObjectToAdd);
    }
    
}
