using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    private float horizontalInput;

    [SerializeField] private GameObject linePrefab;
    private GameObject currentLine;
    private LineRenderer currentLineRenderer;
    private EdgeCollider2D currentEdgeCollider;
    private List<Vector3> linePositionsList = new List<Vector3>();

    private float lineSpawnTimer = 0f;
    private float lineSpawnTimerMax = 3f; 
    private float lineSpawnDowntimeTimer = 0f;
    private float lineSpawnDowntimeTimerMax = 0.5f;

    public event EventHandler OnIsDrawingChanged;
    private bool isDrawing = false;
    private bool isAlive = true;

    private Color playerColor;
    private int playerIndex;
    private PlayerInputActions inputActions;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    

    private float boundsX;
    private float boundsY;

    private bool isInvulnerable;
    private float invulnerablePowerupTimer = 0f;
    private float invulnerablePowerupTimerMax = 5f;

    private float moveSpeedSlowed = 6f;
    private float moveSpeedNormal = 8f;
    private float moveSpeedHastened = 10f;
    private float slowTimer = 0f;
    private float slowTimerMax = 4f;
    private float hasteTimer = 0f;
    private float hasteTimerMax = 4f;

    private bool isSlowed;
    private bool isHastened;

    private bool isConfused = false;
    private float confusionTimer = 0f;
    private float confusionTimerMax = 4f;


    private void Awake()
    {
        OnIsDrawingChanged += Player_OnIsDrawingChanged;
    }
    private void Start()
    {
        boundsX = AchtungGameManager.Instance.GetBoundX();
        boundsY = AchtungGameManager.Instance.GetBoundY();
    }
    
    private void Player_OnIsDrawingChanged(object sender, EventArgs e)
    {
        if (isDrawing)
        {
            CreateNewLine();
        }
        else
        {
            if (currentLine != null && currentLine.transform.parent != null)
            {
                currentLine.transform.SetParent(null);
            }
        }
    }

    private void Update()
    {
        if(isAlive && AchtungGameManager.Instance.GetGameState() == AchtungGameManager.GameState.playing)
        {
            GetInput();

            HandleDrawTimer();
        }
        if (isInvulnerable)
        {
            HandleInvulerabilityTimer();
        }
        if (isConfused)
        {
            HandleConfusionTimer();
        }
        HandleMoveSpeedTimers();
        HandleMoveSpeed();
    }

    private void FixedUpdate()
    {
        if (isAlive && AchtungGameManager.Instance.GetGameState() == AchtungGameManager.GameState.playing)
        {
            MoveForward();
            ApplyRotation();
            HandleBounds();

            if (isInvulnerable)
            {
                return;
            }
            if (isDrawing)
            {
                HandleLineRenderer();
                HandleEdgeCollider();
            }
        }
    }
    private void HandleLineRenderer()
    {
        if (currentLineRenderer != null)
        {
            int numberOfPositions = currentLineRenderer.positionCount;
            currentLineRenderer.positionCount++;
            currentLineRenderer.SetPosition(numberOfPositions, transform.position);

            Vector3[] linePositionsArray = new Vector3[currentLineRenderer.positionCount];
            currentLineRenderer.GetPositions(linePositionsArray);
            linePositionsList = linePositionsArray.ToList();
        }
    }
    private void HandleEdgeCollider()
    {
            if(currentEdgeCollider != null)
            {
                List<Vector2> edgePoints = new List<Vector2>();

                foreach (Vector3 worldPoint in linePositionsList)
                {
                    Vector2 localPoint = transform.InverseTransformPoint(worldPoint);
                    edgePoints.Add(localPoint);
                }

                currentEdgeCollider.points = edgePoints.ToArray();
            }
    }
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

            if(slowTimer >= slowTimerMax)
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

    private void MoveForward()
    {
        transform.position += transform.right * moveSpeed * Time.deltaTime;
    }

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
    private void CreateNewLine()
    {
        currentLine = Instantiate(linePrefab, transform);
        MapCleaner.Instance.AddToListOfSpawnedLines(currentLine);
        currentLineRenderer = currentLine.GetComponent<LineRenderer>();
        currentLineRenderer.startColor = playerColor;
        currentLineRenderer.endColor = playerColor;
        currentEdgeCollider = currentLine.GetComponent<EdgeCollider2D>();
    }
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
    public void PlayerHit()
    {
        if (isDrawing && isAlive)
        {
            isAlive = false;

            PowerupManager.Instance.RemovePlayerFromList(this);

            AchtungGameManager.Instance.PlayerDied(playerIndex);
        }

    }
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
    private void OnDisable()
    {
        inputActions.Disable();
    }
    public int GetPlayerIndex()
    {
        return playerIndex;
    }
    public void InvulnerablePowerupPickedUp()
    {
        isInvulnerable = true;

        invulnerablePowerupTimer = 0f;

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
}
