using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    [Header("Other")]
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    
    // Input fields
    private float horizontalInput;

    // Current line fields
    private GameObject currentLine;
    private LineRenderer currentLineRenderer;
    private EdgeCollider2D currentEdgeCollider;
    private List<Vector3> linePositionsList = new List<Vector3>();

    // Level bounds fields
    private float boundsX;
    private float boundsY;

    // Player states and debuffs
    private bool isDrawing = false;
    private bool isAlive = true;
    private bool isInvulnerable;
    private bool isSlowed;
    private bool isHastened;
    private bool isConfused;

    // Player details
    private Color playerColor;
    private int playerIndex;

    // Movement speeds
    private float moveSpeedSlowed = 6f;
    private float moveSpeedNormal = 8f;
    private float moveSpeedHastened = 10f;

    // Other
    public event EventHandler OnIsDrawingChanged;

    private PlayerInputActions inputActions;

    //Timers

    // Line spawn and downtime timers
    private float lineSpawnTimer = 0f;
    private float lineSpawnTimerMax = 3f;
    private float lineSpawnDowntimeTimer = 0f;
    private float lineSpawnDowntimeTimerMax = 0.5f;

    // Invulnerability powerup timers
    private float invulnerablePowerupTimer = 0f;
    private float invulnerablePowerupTimerMax = 5f;

    // Slow timers
    private float slowTimer = 0f;
    private float slowTimerMax = 4f;

    // Haste timers
    private float hasteTimer = 0f;
    private float hasteTimerMax = 4f;

    // Confusion timers
    private float confusionTimer = 0f;
    private float confusionTimerMax = 4f;


    private void Awake()
    {
        OnIsDrawingChanged += Player_OnIsDrawingChanged;
    }
    
    // On start get the level bounds
    private void Start()
    {
        boundsX = AchtungGameManager.Instance.GetBoundX();
        boundsY = AchtungGameManager.Instance.GetBoundY();
    }
    private void Update()
    {
        // If the player is alive and the game is playing register input and count the draw timers
        if(isAlive && AchtungGameManager.Instance.GetGameState() == AchtungGameManager.GameState.playing)
        {
            GetInput();

            HandleDrawTimer();
        }

        // If the player is invulnerable count the timer
        if (isInvulnerable && AchtungGameManager.Instance.GetGameState() == AchtungGameManager.GameState.playing)
        {
            HandleInvulerabilityTimer();
        }

        // If the player confused count the timer
        if (isConfused && AchtungGameManager.Instance.GetGameState() == AchtungGameManager.GameState.playing)
        {
            HandleConfusionTimer();
        }

        // If the player is slowed or hastened count the coresponding timers
        HandleMoveSpeedTimers();

        // Apply the movement speed debuffs
        HandleMoveSpeed();
    }

    private void FixedUpdate()
    {
        // If the player is alive and the game is playing move the player forward and apply rotation based on the input
        if (isAlive && AchtungGameManager.Instance.GetGameState() == AchtungGameManager.GameState.playing)
        {
            MoveForward();
            ApplyRotation();

            // If the player reaches the edge of the level it is transported to the opposite edge of the map (if PhaseBounds powerup is NOT active player will hit the edge of the level instead)
            HandleBounds();

            // If the player is invulnerable due to the powerup it will not draw any lines
            if (isInvulnerable)
            {
                return;
            }

            // If the player is currently drawing the methods will update the line renderer and the edge collider component of the Line
            if (isDrawing)
            {
                HandleLineRenderer();
                HandleEdgeCollider();
            }
        }
    }
    private void Player_OnIsDrawingChanged(object sender, EventArgs e)
    {
        // If the player is now drawing create a new line
        if (isDrawing)
        {
            CreateNewLine();
        }
        // If the player stopped drawing set the line parent to null so it is detached from the player
        else
        {
            if (currentLine != null && currentLine.transform.parent != null)
            {
                currentLine.transform.SetParent(null);
            }
        }
    }

    private void HandleLineRenderer()
    {
        if (currentLineRenderer != null)
        {
            // Get the number of positions currently in the LineRenderer
            int numberOfPositions = currentLineRenderer.positionCount;
            
            // Add 1 to the number of positions
            currentLineRenderer.positionCount++;

            // Set the last position of the line renderer to the position of the player
            currentLineRenderer.SetPosition(numberOfPositions, transform.position);

            // Update the list of LineRenderer positions (for the EdgeCollider)
            Vector3[] linePositionsArray = new Vector3[currentLineRenderer.positionCount];
            currentLineRenderer.GetPositions(linePositionsArray);
            linePositionsList = linePositionsArray.ToList();
        }
    }
    private void HandleEdgeCollider()
    {
        if(currentEdgeCollider != null)
        {
            // Create an empty list
            List<Vector2> edgePoints = new List<Vector2>();

            // Fill the list with the positions from the LineRenderer
            foreach (Vector3 worldPoint in linePositionsList)
            {
                Vector2 localPoint = transform.InverseTransformPoint(worldPoint);
                edgePoints.Add(localPoint);
            }

            // Set the positions on the EdgeCollider to match the positions of the LineRenderer
            currentEdgeCollider.points = edgePoints.ToArray();
        }
    }

    // Set the movement speed of the player (if the player is both slowed and hastened speed is set to a starting value)
    private void HandleMoveSpeed()
    {
        if (isHastened && isSlowed)
        {
            moveSpeed = moveSpeedNormal;
        }
        else if(isSlowed && !isHastened)
        {
            moveSpeed = moveSpeedSlowed;
        }
        else if (!isSlowed && isHastened)
        {
            moveSpeed = moveSpeedHastened;
        }
        else if (!isSlowed && !isHastened)
        {
            moveSpeed = moveSpeedNormal;
        }
    }

    // Input is registered only on the Action map depending on the player index
    private void GetInput()
    {
        if(playerIndex == 0)
        {
            horizontalInput = inputActions.Player1.Movement.ReadValue<float>();
        }
        if (playerIndex == 1)
        {
            horizontalInput = inputActions.Player2.Movement.ReadValue<float>();
        }
        if (playerIndex == 2)
        {
            horizontalInput = inputActions.Player3.Movement.ReadValue<float>();
        }
        if (playerIndex == 3)
        {
            horizontalInput = inputActions.Player4.Movement.ReadValue<float>();
        }
    }

    // Moves the player object forward
    private void MoveForward()
    {
        transform.position += transform.right * moveSpeed * Time.deltaTime;
    }

    // Applies the rotation the player game object based on input
    private void ApplyRotation()
    {
        if (isConfused)
        {
            transform.rotation *= Quaternion.Euler(0, 0, turnSpeed * horizontalInput);
        }
        else
        {
            transform.rotation *= Quaternion.Euler(0, 0, turnSpeed * -horizontalInput);
        }
    }

    // Creates a new line, adds it to the list of spawned lines (for cleanup), sets the line color and sets the current linerenderer and edge collider fields
    private void CreateNewLine()
    {
        currentLine = Instantiate(linePrefab, transform);
        MapCleaner.Instance.AddToListOfSpawnedLines(currentLine);
        currentLineRenderer = currentLine.GetComponent<LineRenderer>();
        currentLineRenderer.startColor = playerColor;
        currentLineRenderer.endColor = playerColor;
        currentEdgeCollider = currentLine.GetComponent<EdgeCollider2D>();
    }

    // If the player is at the edge of the level it will be transported to the opposite edge of the level
    // When the player is transported the current line is dropped and a new one is created on the opposite edge
    private void HandleBounds()
    {
        Vector3 currentPosition = transform.position;
        if (currentPosition.x >= boundsX + 0.4f)
        {
            isDrawing = false;
            OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
            transform.position = new Vector3(-transform.position.x + 0.3f, transform.position.y, transform.position.z);
            isDrawing = true;
            OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
        }
        else if (currentPosition.x <= -boundsX - 0.4f)
        {
            isDrawing = false;
            OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
            transform.position = new Vector3(-transform.position.x - 0.3f, transform.position.y, transform.position.z);
            isDrawing = true;
            OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
        }
        else if (currentPosition.y >= boundsY + 0.4f)
        {
            isDrawing = false;
            OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
            transform.position = new Vector3(transform.position.x, -transform.position.y +0.3f, transform.position.z);
            isDrawing = true;
            OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
        }
        else if (currentPosition.y <= -boundsY - 0.4f)
        {
            isDrawing = false;
            OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
            transform.position = new Vector3(transform.position.x, -transform.position.y - 0.3f, transform.position.z);
            isDrawing = true;
            OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    // When the players hits something it is set to dead, and is removed from the PowerupManager and the list of alive players in the GameManager
    public void PlayerHit()
    {
        if (isDrawing && isAlive)
        {
            isAlive = false;

            PowerupManager.Instance.RemovePlayerFromList(this);

            AchtungGameManager.Instance.PlayerDied(playerIndex);
        }

    }

    // Player buffs and debuffs
    // When a buff or debuff is applied the state is set to true and the timer is reset
    public void InvulnerablePowerupPickedUp()
    {
        isInvulnerable = true;

        invulnerablePowerupTimer = 0f;

        // When invulnerable the player is not drawing a line
        if (isDrawing)
        {
            isDrawing = false;
            OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
        }      
    }
    public void SlowPlayer()
    {
        slowTimer = 0f;
        isSlowed = true;
    }
    public void HastenPlayer()
    {
        hasteTimer = 0f;
        isHastened = true;
    }
    public void ConfusePlayer()
    {
        confusionTimer = 0f;
        isConfused = true;
    }

    // Timers
    private void HandleDrawTimer()
    {
        if (isInvulnerable)
        {
            return;
        }
        if (isDrawing)
        {
            lineSpawnTimer += Time.deltaTime;
            if (lineSpawnTimer >= lineSpawnTimerMax)
            {
                lineSpawnTimer = UnityEngine.Random.Range(0f, 1f);
                isDrawing = false;
                OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            lineSpawnDowntimeTimer += Time.deltaTime;
            if (lineSpawnDowntimeTimer >= lineSpawnDowntimeTimerMax)
            {
                lineSpawnDowntimeTimer = 0f;
                isDrawing = true;
                OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    private void HandleInvulerabilityTimer()
    {
        invulnerablePowerupTimer += Time.deltaTime;

        if (invulnerablePowerupTimer >= invulnerablePowerupTimerMax)
        {
            invulnerablePowerupTimer = 0f;
            isInvulnerable = false;
        }
    }
    private void HandleMoveSpeedTimers()
    {
        if (isSlowed)
        {
            slowTimer += Time.deltaTime;

            if (slowTimer >= slowTimerMax)
            {
                slowTimer = 0f;
                isSlowed = false;
            }
        }
        if (isHastened)
        {
            hasteTimer += Time.deltaTime;

            if (hasteTimer >= hasteTimerMax)
            {
                hasteTimer = 0f;
                isHastened = false;
            }
        }
    }
    private void HandleConfusionTimer()
    {
        confusionTimer += Time.deltaTime;
        if (confusionTimer >= confusionTimerMax)
        {
            confusionTimer = 0f;
            isConfused = false;
        }
    }

    // Setters
    public void SetPlayerColor(Color colorToSet)
    {
        playerColor = colorToSet;
        playerSpriteRenderer.color = playerColor;
    }
    public void SetPlayerIndex(int playerIndexToSet)
    {
        playerIndex = playerIndexToSet;

        inputActions = new PlayerInputActions();
        inputActions.Enable();
    }

    // Getters
    public int GetPlayerIndex()
    {
        return playerIndex;
    }
    // Other 
    private void OnDisable()
    {
        inputActions.Disable();
    }
}
