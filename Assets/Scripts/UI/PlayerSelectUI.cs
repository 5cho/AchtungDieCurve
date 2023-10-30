using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectUI : MonoBehaviour
{
    [Header("Scripts references")]
    [SerializeField] private MainMenuUI mainMenuUI;
    [SerializeField] private ControlsUI controlsUI;
    [Header("Menu buttons")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button playButton;
    [Header("Player count buttons")]
    [SerializeField] private Button twoPlayersButton;
    [SerializeField] private Button threePlayersButton;
    [SerializeField] private Button fourPlayersButton;
    [Header("Points count buttons")]
    [SerializeField] private Button tenPointsButton;
    [SerializeField] private Button fiftyPointsButton;
    [SerializeField] private Button hundredPointsButton;
    [Header("Selected visual gameobjects")]
    [SerializeField] private Transform selectedPlayers;
    [SerializeField] private Transform selectedPoints;

    private void Start()
    {
        // Show the controls UI
        playButton.onClick.AddListener(() => {
            Hide();
            controlsUI.Show();
        });

        // Go back to the main menu
        backButton.onClick.AddListener(() => {
            Hide();
            mainMenuUI.Show();
        });

        // Set the selected player count to 2 and position the selected visual accordingly
        twoPlayersButton.onClick.AddListener(() => {
            AchtungGameManager.Instance.SetNumberOfPlayer(2);
            selectedPlayers.gameObject.SetActive(true);
            selectedPlayers.position = twoPlayersButton.transform.position;
        });

        // Set the selected player count to 3 and position the selected visual accordingly
        threePlayersButton.onClick.AddListener(() => {
            AchtungGameManager.Instance.SetNumberOfPlayer(3);
            selectedPlayers.gameObject.SetActive(true);
            selectedPlayers.position = threePlayersButton.transform.position;

        });

        // Set the selected player count to 4 and position the selected visual accordingly
        fourPlayersButton.onClick.AddListener(() => {
            AchtungGameManager.Instance.SetNumberOfPlayer(4);
            selectedPlayers.gameObject.SetActive(true);
            selectedPlayers.position = fourPlayersButton.transform.position;

        });

        // Set the selected points count to 10 and position the selected visual accordingly
        tenPointsButton.onClick.AddListener(() => {
            AchtungGameManager.Instance.SetMaxPoints(10);
            selectedPoints.gameObject.SetActive(true);
            selectedPoints.position = tenPointsButton.transform.position;
        });

        // Set the selected points count to 50 and position the selected visual accordingly
        fiftyPointsButton.onClick.AddListener(() => {
            AchtungGameManager.Instance.SetMaxPoints(50);
            selectedPoints.gameObject.SetActive(true);
            selectedPoints.position = fiftyPointsButton.transform.position;
        });

        // Set the selected points count to 100 and position the selected visual accordingly
        hundredPointsButton.onClick.AddListener(() => {
            AchtungGameManager.Instance.SetMaxPoints(100);
            selectedPoints.gameObject.SetActive(true);
            selectedPoints.position = hundredPointsButton.transform.position;
        });

        // Hide selected visuals on start
        selectedPlayers.gameObject.SetActive(false);
        selectedPoints.gameObject.SetActive(false);

        // Hide the UI on start
        Hide();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
        SetDefaults();
    }
    private void SetDefaults()
    {
        // By default the number of players is set to 2 and the points max is set to 10, the visuals are set to represent that
        AchtungGameManager.Instance.SetNumberOfPlayer(2);
        AchtungGameManager.Instance.SetMaxPoints(10);
        selectedPlayers.gameObject.SetActive(true);
        selectedPoints.gameObject.SetActive(true);
        selectedPlayers.position = twoPlayersButton.transform.position;
        selectedPoints.position = tenPointsButton.transform.position;
    }
}
