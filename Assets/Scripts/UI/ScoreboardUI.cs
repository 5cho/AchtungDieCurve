using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class ScoreboardUI : MonoBehaviour
{
    // Buttons
    [Header("Buttons")]
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button nextRoundButton;

    // Events
    public event EventHandler OnScoreboardShown;

    private void Start()
    {
        AchtungGameManager.Instance.OnRoundOver += Instance_OnRoundOver;
        AchtungGameManager.Instance.OnGameOver += Instance_OnGameOver;

        // On main menu button pressed load the game from scratch
        mainMenuButton.onClick.AddListener(() => {
            SceneManager.LoadScene(0);
        });

        // On next round button pressed start the next round
        nextRoundButton.onClick.AddListener(() => {
            AchtungGameManager.Instance.SetUpNewRound();

            Hide();
        });

        Hide();
    }

    private void Instance_OnGameOver(object sender, System.EventArgs e)
    {
        Show();

        // Hide the next round button because the game is over
        nextRoundButton.gameObject.SetActive(false);

        // Set the position of the button to the middle of the screen
        RectTransform mainMenuButtonRectTransform = mainMenuButton.gameObject.GetComponent<RectTransform>();
        mainMenuButtonRectTransform.anchoredPosition = new Vector2(0, mainMenuButtonRectTransform.anchoredPosition.y);

        OnScoreboardShown?.Invoke(this, EventArgs.Empty);
    }

    private void Instance_OnRoundOver(object sender, System.EventArgs e)
    {
        Show();

        OnScoreboardShown?.Invoke(this, EventArgs.Empty);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
}
