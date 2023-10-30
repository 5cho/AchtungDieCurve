using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class ScoreboardUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button nextRoundButton;

    public event EventHandler OnScoreboardShown;

    private void Start()
    {
        AchtungGameManager.Instance.OnRoundOver += Instance_OnRoundOver;
        AchtungGameManager.Instance.OnGameOver += Instance_OnGameOver;

        mainMenuButton.onClick.AddListener(() => {
            SceneManager.LoadScene(0);
        });

        nextRoundButton.onClick.AddListener(() => {
            AchtungGameManager.Instance.SetUpNewGame();

            Hide();
        });

        Hide();
    }

    private void Instance_OnGameOver(object sender, System.EventArgs e)
    {
        Show();

        nextRoundButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(true);

        OnScoreboardShown?.Invoke(this, EventArgs.Empty);
    }

    private void Instance_OnRoundOver(object sender, System.EventArgs e)
    {
        Show();

        mainMenuButton.gameObject.SetActive(false);
        nextRoundButton.gameObject.SetActive(true);

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
