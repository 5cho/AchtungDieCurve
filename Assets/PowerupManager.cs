using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PowerupManager : MonoBehaviour
{
    public static PowerupManager Instance { get; private set; }
    private List<Player> listOfPlayers = new List<Player>();
    
    private float phaseBoundsTimer = 0f;
    [SerializeField] private float phaseBoundsTimerMax = 10f;
    private bool isPhaseBoundsPowerupActive = false;
    
    [SerializeField] private GameObject[] boundsGameObjectArray;

    [SerializeField] private List<GameObject> listOfPowerups;
    private float powerupSpawnTimer;
    private float powerupSpawnTimerMax = 8f;


    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (isPhaseBoundsPowerupActive)
        {
            phaseBoundsTimer += Time.deltaTime;

            if(phaseBoundsTimer >= phaseBoundsTimerMax)
            {
                phaseBoundsTimer = 0f;
                isPhaseBoundsPowerupActive = false;

                SetBoundsToState(true);
            }
        }
        HandlePowerupSpawnTimer();
    }
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
    public void PhaseBoundsPowerupPickedUp()
    {
        isPhaseBoundsPowerupActive = true;

        phaseBoundsTimer = 0f;

        SetBoundsToState(false);

    }
    public bool IsPhaseBoundsPowerupActive()
    {
        return isPhaseBoundsPowerupActive;
    }

    private void SetBoundsToState(bool state)
    {
        foreach(GameObject boundsGameObject in boundsGameObjectArray)
        {
            boundsGameObject.SetActive(state);
        }
    }
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
    private void HandlePowerupSpawnTimer()
    {
        powerupSpawnTimer += Time.deltaTime;

        if(powerupSpawnTimer >= powerupSpawnTimerMax)
        {
            powerupSpawnTimer = 0f;

            SpawnPowerup();
        }
    }
    private void SpawnPowerup()
    {
        float spawnOffset = 3f;
        Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-AchtungGameManager.Instance.GetBoundX() + spawnOffset, AchtungGameManager.Instance.GetBoundX() - spawnOffset), UnityEngine.Random.Range(-AchtungGameManager.Instance.GetBoundY() + spawnOffset, AchtungGameManager.Instance.GetBoundY() - spawnOffset), 0f);
        GameObject spawnedPowerup = Instantiate(listOfPowerups[UnityEngine.Random.Range(0, listOfPowerups.Count)], spawnPosition, Quaternion.identity);
        MapCleaner.Instance.AddPowerupToMapCleaner(spawnedPowerup);
    }
}
