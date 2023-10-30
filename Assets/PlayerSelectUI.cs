using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectUI : MonoBehaviour
{
    [SerializeField] private MainMenuUI mainMenuUI;
    [SerializeField] private ControlsUI controlsUI;
    [SerializeField] private Button backButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button twoPlayersButton;
    [SerializeField] private Button threePlayersButton;
    [SerializeField] private Button fourPlayersButton;
    [SerializeField] private Button tenPointsButton;
    [SerializeField] private Button fiftyPointsButton;
    [SerializeField] private Button hundredPointsButton;
    [SerializeField] private Transform selectedPlayers;
    [SerializeField] private Transform selectedPoints;

    private void Start()
    {
        playButton.onClick.AddListener(() => {
            Hide();
            controlsUI.Show();
        });

        backButton.onClick.AddListener(() => {
            Hide();
            mainMenuUI.Show();
        });

        twoPlayersButton.onClick.AddListener(() => {
            AchtungGameManager.Instance.SetNumberOfPlayer(2);
            selectedPlayers.gameObject.SetActive(true);
            selectedPlayers.position = twoPlayersButton.transform.position;
        });

        threePlayersButton.onClick.AddListener(() => {
            AchtungGameManager.Instance.SetNumberOfPlayer(3);
            selectedPlayers.gameObject.SetActive(true);
            selectedPlayers.position = threePlayersButton.transform.position;

        });
        
        fourPlayersButton.onClick.AddListener(() => {
            AchtungGameManager.Instance.SetNumberOfPlayer(4);
            selectedPlayers.gameObject.SetActive(true);
            selectedPlayers.position = fourPlayersButton.transform.position;

        });

        tenPointsButton.onClick.AddListener(() => {
            AchtungGameManager.Instance.SetMaxPoints(10);
            selectedPoints.gameObject.SetActive(true);
            selectedPoints.position = tenPointsButton.transform.position;
        });

        fiftyPointsButton.onClick.AddListener(() => {
            AchtungGameManager.Instance.SetMaxPoints(50);
            selectedPoints.gameObject.SetActive(true);
            selectedPoints.position = fiftyPointsButton.transform.position;
        });

        hundredPointsButton.onClick.AddListener(() => {
            AchtungGameManager.Instance.SetMaxPoints(100);
            selectedPoints.gameObject.SetActive(true);
            selectedPoints.position = hundredPointsButton.transform.position;
        });

        selectedPlayers.gameObject.SetActive(false);
        selectedPoints.gameObject.SetActive(false);
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
        AchtungGameManager.Instance.SetNumberOfPlayer(2);
        AchtungGameManager.Instance.SetMaxPoints(10);
        selectedPlayers.gameObject.SetActive(true);
        selectedPoints.gameObject.SetActive(true);
        selectedPlayers.position = twoPlayersButton.transform.position;
        selectedPoints.position = tenPointsButton.transform.position;
    }
}
