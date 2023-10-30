using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuUI : MonoBehaviour
{
    [Header("Script references")]
    [SerializeField] private PlayerSelectUI playerSelectUI;
    [Header("Menu buttons")]
    [SerializeField] private Button playButton; 
    [SerializeField] private Button quitButton;
    
    private void Start()
    {
        // Play button press hides the MainMenuUI window and shows the PlayerSelectUI window
        playButton.onClick.AddListener(() => {
            Hide();
            playerSelectUI.Show();
        });

        // Quit button press closes the application
        quitButton.onClick.AddListener(() => {
            Application.Quit();
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
