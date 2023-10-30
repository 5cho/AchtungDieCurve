using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCleaner : MonoBehaviour
{
    public static MapCleaner Instance;

    private List<GameObject> listOfSpawnedLines = new List<GameObject>();
    private List<GameObject> listOfSpawnedPlayers = new List<GameObject>();
    private List<GameObject> listOfSpawnedPowerups = new List<GameObject>();


    public void AddToListOfSpawnedLines(GameObject gameObjectToAdd)
    {
        listOfSpawnedLines.Add(gameObjectToAdd);
    }

    private void Awake()
    {
        Instance = this;
    }
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
    public void AddPlayerToMapCleaner(GameObject playerGameObject)
    {
        listOfSpawnedPlayers.Add(playerGameObject);
    }
    public void AddPowerupToMapCleaner(GameObject powerupGameObject)
    {
        listOfSpawnedPowerups.Add(powerupGameObject);
    }
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
}
