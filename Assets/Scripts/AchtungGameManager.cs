using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AchtungGameManager : MonoBehaviour
{
    public static AchtungGameManager Instance { get; private set; }

    [Header("Script references")]
    [SerializeField] private GamePausedUI gamePausedUI;
    [Header("Bounds")]
    [SerializeField] private GameObject topBound;
    [SerializeField] private GameObject rightBound;
    [Header("Other")]
    [SerializeField] private GameObject playerPrefab;

    // Bounds
    private float boundsX;
    private float boundsY;

    // Dictionaries
    private Dictionary<int, bool> playerAliveDictionary = new Dictionary<int, bool>();
    private Dictionary<int, int> scoreboardDictionary = new Dictionary<int, int>();

    // Events
    public event EventHandler OnGameOver;
    public event EventHandler OnRoundOver;

    // Game state
    private GameState state;

    // Amount of players currently alive
    private int numberOfPlayersAlive;
   
    // Points for game win
    private int maxPoints = 10;

    // Other
    private Color[] arrayOfColors = new Color[4];
    private int numberOfPlayers;
    private float spawnOffset = 5f;

    public enum GameState
    {
        playing,
        notPlaying
    }

    private void Awake()
    {
        Instance = this;

        // Set the bounds
        boundsX = rightBound.transform.position.x;
        boundsY = topBound.transform.position.y;

        // Set the state
        state = GameState.notPlaying;

        // Initiate the array of colors
        SetColorsArray();
    }

    private void Update()
    {
        // Pause/unpause the game on spacebar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (state == GameState.playing)
            {
                state = GameState.notPlaying;

                gamePausedUI.Show();
            }
            else
            {
                state = GameState.playing;

                gamePausedUI.Hide();
            }
        }
    }
    // Set the color and the index of a player
    private void SetupPlayer(Player player, int playerIndex)
    {
        player.SetPlayerColor(arrayOfColors[playerIndex]);
        player.SetPlayerIndex(playerIndex);
    }

    // Spawn all players for the start of the game
    public void SpawnPlayers()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            // Spawn a player in a random position with a random rotation
            Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-boundsX + spawnOffset, boundsX - spawnOffset), UnityEngine.Random.Range(-boundsY + spawnOffset, boundsY - spawnOffset), 0f);
            Quaternion randomRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360));
            GameObject playerGameObject = Instantiate(playerPrefab, randomPosition, randomRotation);

            // Add the player gameobject to the MapCleaner
            MapCleaner.Instance.AddPlayerToMapCleaner(playerGameObject);

            // Add the player to the list of players in the PowerupManager
            Player player = playerGameObject.GetComponent<Player>();
            PowerupManager.Instance.AddPlayerToList(player);

            // Set the color and the index of the spawned player
            SetupPlayer(player, i);

            // Add the player to the dictionary of alive players
            playerAliveDictionary.Add(i, true);
        }

        // Set the number of alive players to the total number of players
        numberOfPlayersAlive = numberOfPlayers;
    }
    public void PlayerDied(int playerIndex)
    {
        // Reduce the number of players alive in the session
        numberOfPlayersAlive--;

        // Set the player as dead in the dictionary of alive players
        playerAliveDictionary[playerIndex] = false;

        // Update the scoreboard for the player that died
        UpdateScoreboard(playerIndex, 4 - numberOfPlayersAlive);       

        // Check if there is only one player left alive in the game
        CheckForRoundWinner();
    }

    // If the number of players alive is 1 the remaining player is declared as the winner of the round
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
    // Update the score of the winning player and check if any of them reached the point maximum and won the game
    private void DeclareWinner(int playerIndex)
    {
        UpdateScoreboard(playerIndex, 4);

        state = GameState.notPlaying;

        for(int i = 0; i < numberOfPlayers; i++)
        {
            if(scoreboardDictionary[i] >= maxPoints)
            {
                // Invoke the event if the game is over
                OnGameOver?.Invoke(this, EventArgs.Empty);

                return;
            }
            else
            {
                // Invoke the event if the game is NOT over to start the next round
                OnRoundOver?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    // Set up a new game session and an empty scoreboard
    public void StartNewGame()
    {
        SetUpNewRound();
        SetUpScoreboard();
    }
    
    // Set up a new round
    public void SetUpNewRound()
    {
        // Clear the dictionary of alive players
        playerAliveDictionary.Clear();

        // Clear the map of any spawned objects left
        MapCleaner.Instance.ClearMap();
        
        // Spawn all players
        SpawnPlayers();

        // Set up the game state
        state = GameState.notPlaying;

        // Show the UI
        gamePausedUI.Show();
    }
    // Set up a scoreboard for all the players in the game to 0
    private void SetUpScoreboard()
    {
        for(int i = 0; i < numberOfPlayers; i++)
        {
            scoreboardDictionary.Add(i, 0);
        }
    }
    // Update the scoreboard for the player with playerIndex and add the pointsAmmount to it in the scoreboard
    private void UpdateScoreboard(int playerIndex, int pointsAmmount)
    {
        scoreboardDictionary[playerIndex] += pointsAmmount;
    }
    
    // Getters
    public Color GetColorByIndex(int playerIndex)
    {
        return arrayOfColors[playerIndex];
    }
    public Dictionary<int, int> GetScoreboardDictionary()
    {
        return scoreboardDictionary;
    }
    public int GetNumberOfPlayers()
    {
        return numberOfPlayers;
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

    // Setters
    public void SetMaxPoints(int pointsToSet)
    {
        maxPoints = pointsToSet;
    }
    private void SetColorsArray()
    {
        arrayOfColors[0] = Color.red;
        arrayOfColors[1] = Color.blue;
        arrayOfColors[2] = Color.green;
        arrayOfColors[3] = Color.yellow;
    }
    public void SetNumberOfPlayer(int numberOfPlayersToSet)
    {
        numberOfPlayers = numberOfPlayersToSet;
    }
}
