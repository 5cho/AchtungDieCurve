using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PowerupManager : MonoBehaviour
{
    public static PowerupManager Instance { get; private set; }

    // List of players in current game instance
    private List<Player> listOfPlayers = new List<Player>();
    
    // Powerup spawn timer fields
    [SerializeField] private float powerupSpawnTimerMax = 8f;
    private float powerupSpawnTimer;
    
    [SerializeField] private GameObject[] boundsGameObjectArray;

    [SerializeField] private List<GameObject> listOfPowerups;


    // Phase bounds fields
    [SerializeField] private float phaseBoundsTimerMax = 10f;
    private float phaseBoundsTimer = 0f;
    private bool isPhaseBoundsPowerupActive = false;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        // If the game is not playing stop counting timers
        if(AchtungGameManager.Instance.GetGameState() != AchtungGameManager.GameState.playing)
        {
            return;
        }

        // If the PhaseBounds powerup is active count its timer
        if (isPhaseBoundsPowerupActive)
        {
            HandlePhaseBoundsPowerupTimer();
        }

        // Count the powerup spawn timer
        HandlePowerupSpawnTimer();
    }

    private void SpawnPowerup()
    {
        float spawnOffset = 3f;
        Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-AchtungGameManager.Instance.GetBoundX() + spawnOffset, AchtungGameManager.Instance.GetBoundX() - spawnOffset), UnityEngine.Random.Range(-AchtungGameManager.Instance.GetBoundY() + spawnOffset, AchtungGameManager.Instance.GetBoundY() - spawnOffset), 0f);
        GameObject spawnedPowerup = Instantiate(listOfPowerups[UnityEngine.Random.Range(0, listOfPowerups.Count)], spawnPosition, Quaternion.identity);
        MapCleaner.Instance.AddPowerupToMapCleaner(spawnedPowerup);
    }

    // When the PhaseBounds powerup is picked up reset the timer and disable bounds gameobjects (to stop them from hitting the player)
    public void PhaseBoundsPowerupPickedUp()
    {
        isPhaseBoundsPowerupActive = true;

        phaseBoundsTimer = 0f;

        SetBoundsToState(false);

    }

    // Function enables or disables the level bounds
    private void SetBoundsToState(bool state)
    {
        foreach(GameObject boundsGameObject in boundsGameObjectArray)
        {
            boundsGameObject.SetActive(state);
        }
    }

    // Functions that apply debuffs are called on all players EXCEPT the activating player
    public void SlowAllOtherPlayers(Player activatingPlayer)
    {
        foreach(Player player in listOfPlayers)
        {
            if(player == activatingPlayer)
            {
                continue;
            }
            else
            {
                player.SlowPlayer();
            }
        }
    }
    public void ConfuseAllOtherPlayers(Player activatingPlayer)
    {
        foreach(Player player in listOfPlayers)
        {
            if(player == activatingPlayer)
            {
                continue;
            }
            else
            {
                player.ConfusePlayer();
            }
        }
    }

    // Timer functions
    private void HandlePowerupSpawnTimer()
    {
        powerupSpawnTimer += Time.deltaTime;

        if(powerupSpawnTimer >= powerupSpawnTimerMax)
        {
            powerupSpawnTimer = 0f;

            SpawnPowerup();
        }
    }
    private void HandlePhaseBoundsPowerupTimer()
    {
        phaseBoundsTimer += Time.deltaTime;

        if (phaseBoundsTimer >= phaseBoundsTimerMax)
        {
            phaseBoundsTimer = 0f;
            isPhaseBoundsPowerupActive = false;

            SetBoundsToState(true);
        }
    }

    // Functions related to adding and removing players from the list
    public void AddPlayerToList(Player playerToAdd)
    {
        listOfPlayers.Add(playerToAdd);
    }
    public void ClearListOfPlayers()
    {
        listOfPlayers.Clear();
    }
    public void RemovePlayerFromList(Player player)
    {
        listOfPlayers.Remove(player);
    }

    // Getters
    public bool GetIsPhaseBoundsPowerupActive()
    {
        return isPhaseBoundsPowerupActive;
    }
}
