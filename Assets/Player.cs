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



    
    private void Awake()
    {
        OnIsDrawingChanged += Player_OnIsDrawingChanged;
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

            HandleTimer();
        }
        
        
        
    }

    private void FixedUpdate()
    {
        if (isAlive && AchtungGameManager.Instance.GetGameState() == AchtungGameManager.GameState.playing)
        {
            MoveForward();
            ApplyRotation();
            HandleBounds();

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
    private void HandleTimer()
    {
        if (isDrawing)
        {
            lineSpawnTimer += Time.deltaTime;
            if (lineSpawnTimer >= lineSpawnTimerMax)
            {
                lineSpawnTimer = 0f;
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
        transform.rotation *= Quaternion.Euler(0, 0, turnSpeed * -horizontalInput);
    }
    private void CreateNewLine()
    {
        currentLine = Instantiate(linePrefab, transform);
        AchtungGameManager.Instance.AddToListOfSpawnedObjects(currentLine);
        currentLineRenderer = currentLine.GetComponent<LineRenderer>();
        currentLineRenderer.startColor = playerColor;
        currentLineRenderer.endColor = playerColor;
        currentEdgeCollider = currentLine.GetComponent<EdgeCollider2D>();
    }
    private void HandleBounds()
    {
        Vector3 currentPosition = transform.position;
        float boundsX = AchtungGameManager.Instance.GetBoundX();
        float boundsY = AchtungGameManager.Instance.GetBoundY();
        if (currentPosition.x >= boundsX + 0.5f)
        {
            isDrawing = false;
            OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
            transform.position = new Vector3(-transform.position.x + 0.5f, transform.position.y, transform.position.z);
            isDrawing = true;
            OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
        }
        else if (currentPosition.x <= -boundsX - 0.5f)
        {
            isDrawing = false;
            OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
            transform.position = new Vector3(-transform.position.x - 0.5f, transform.position.y, transform.position.z);
            isDrawing = true;
            OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
        }
        else if (currentPosition.y >= boundsY + 0.5f)
        {
            isDrawing = false;
            OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
            transform.position = new Vector3(transform.position.x, -transform.position.y +0.5f, transform.position.z);
            isDrawing = true;
            OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
        }
        else if (currentPosition.y <= -boundsY - 0.5f)
        {
            isDrawing = false;
            OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
            transform.position = new Vector3(transform.position.x, -transform.position.y - 0.5f, transform.position.z);
            isDrawing = true;
            OnIsDrawingChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        isAlive = false;

        AchtungGameManager.Instance.PlayerDied(playerIndex);
    }
    */
    public void PlayerHit()
    {
        isAlive = false;

        AchtungGameManager.Instance.PlayerDied(playerIndex);
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

    

}
