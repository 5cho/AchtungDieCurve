using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AchtungGameManager : MonoBehaviour
{
    public static AchtungGameManager Instance { get; private set; }
    [SerializeField] private GameObject topBound;
    [SerializeField] private GameObject rightBound;
    [SerializeField] private GameObject playerPrefab;
    private float boundsX;
    private float boundsY;
    private float spawnOffset = 5f;
    private GameState state;

    private int numberOfPlayers;
    private Color[] arrayOfColors = new Color[4];

    private Dictionary<int, bool> playerAliveDictionary = new Dictionary<int, bool>();
    private Dictionary<int, int> scoreboardDictionary = new Dictionary<int, int>();
    private int numberOfPlayersAlive;

    

    private int maxPoints = 10;
    public event EventHandler OnRoundOver;
    public event EventHandler OnGameOver;
    private int pointsForRound;

    public enum GameState
    {
        playing,
        notPlaying
    }

    private void Awake()
    {
        Instance = this;

        boundsX = rightBound.transform.position.x;
        boundsY = topBound.transform.position.y;

        state = GameState.notPlaying;

        numberOfPlayers = 4;

        SetColorsArray();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) && state != GameState.playing)
        {
            state = GameState.playing;
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            MapCleaner.Instance.ClearAllLines();
        }
    }
    public float GetBoundX()
    {
        return boundsX;
    }
    public float GetBoundY()
    {
        return boundsY;
    }
    public GameState GetGameState()
    {
        return state;
    }
    public void SetNumberOfPlayer(int numberOfPlayersToSet)
    {
        numberOfPlayers = numberOfPlayersToSet;
    }
    public int GetNumberOfPlayers()
    {
        return numberOfPlayers;
    }
    private void SetColorsArray()
    {
        arrayOfColors[0] = Color.red;
        arrayOfColors[1] = Color.blue;
        arrayOfColors[2] = Color.green;
        arrayOfColors[3] = Color.yellow;
    }
    private void SetupPlayer(Player player, int playerIndex)
    {
        player.SetPlayerColor(arrayOfColors[playerIndex]);
        player.SetPlayerIndex(playerIndex);
    }
    public void SpawnPlayers()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-boundsX + spawnOffset, boundsX - spawnOffset), UnityEngine.Random.Range(-boundsY + spawnOffset, boundsY - spawnOffset), 0f);
            Quaternion randomRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360));
            GameObject playerGameObject = Instantiate(playerPrefab, randomPosition, randomRotation);
            MapCleaner.Instance.AddPlayerToMapCleaner(playerGameObject);
            Player player = playerGameObject.GetComponent<Player>();
            PowerupManager.Instance.AddPlayerToList(player);
            SetupPlayer(player, i);

            playerAliveDictionary.Add(i, true);
        }
        numberOfPlayersAlive = numberOfPlayers;
    }
    public void PlayerDied(int playerIndex)
    {
        numberOfPlayersAlive--;

        playerAliveDictionary[playerIndex] = false;
        UpdateScoreboard(playerIndex, pointsForRound);
        pointsForRound++;

        CheckForRoundWinner();
    }
    private void CheckForRoundWinner()
    {
        if (numberOfPlayersAlive == 1)
        {
            foreach (var kvp in playerAliveDictionary)
            {
                if (kvp.Value == true)
                {
                    DeclareWinner(kvp.Key);
                }
            }
        }
    }
    private void DeclareWinner(int playerIndex)
    {
        UpdateScoreboard(playerIndex, pointsForRound);
        for(int i = 0; i < numberOfPlayers; i++)
        {
            if(scoreboardDictionary[i] >= maxPoints)
            {
                OnGameOver?.Invoke(this, EventArgs.Empty);

                return;
            }
            else
            {
                OnRoundOver?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    
    
    public void SetMaxPoints(int pointsToSet)
    {
        maxPoints = pointsToSet;
    }
    public void SetUpNewGame()
    {
        playerAliveDictionary.Clear();

        MapCleaner.Instance.ClearMap();
        
        SpawnPlayers();

        pointsForRound = 1;
        state = GameState.notPlaying;
    }
    private void SetUpScoreboard()
    {
        for(int i = 0; i < numberOfPlayers; i++)
        {
            scoreboardDictionary.Add(i, 0);
        }
    }
    private void UpdateScoreboard(int playerIndex, int pointsAmmount)
    {
        scoreboardDictionary[playerIndex] += pointsAmmount;
    }
    public Dictionary<int, int> GetScoreboardDictionary()
    {
        return scoreboardDictionary;
    }
    public Color GetColorByIndex(int playerIndex)
    {
        return arrayOfColors[playerIndex];
    }
    public void StartNewGame()
    {
        SetUpNewGame();
        SetUpScoreboard();
    }
}
