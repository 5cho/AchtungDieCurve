using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton; 
    [SerializeField] private Button quitButton;
    [SerializeField] private PlayerSelectUI playerSelectUI;
    
    private void Start()
    {
        playButton.onClick.AddListener(() => {
            Hide();
            playerSelectUI.Show();
        });

        Show();
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
