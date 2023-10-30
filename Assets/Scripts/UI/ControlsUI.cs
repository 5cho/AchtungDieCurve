using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsUI : MonoBehaviour
{
    [Header("Menu buttons")]
    [SerializeField] private Button closeButton;
    [Header("Player rows array")]
    [SerializeField] private GameObject[] playerRows;

    private void Start()
    {
        // On closeButton press the controlsUI window is hidden and a new game session is started
        closeButton.onClick.AddListener(() => {
            AchtungGameManager.Instance.StartNewGame();
            Hide();
        });

        Hide();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);

        // Get the number of players in session
        int numberOfPlayers = AchtungGameManager.Instance.GetNumberOfPlayers();

        // For each player in the game show one row of controls and hide the rest
        for(int i = 0; i < 4; i++)
        {
            if(i < numberOfPlayers)
            {
                playerRows[i].gameObject.SetActive(true);
            }
            else
            {
                playerRows[i].gameObject.SetActive(false);
            }
        }
    }
}
