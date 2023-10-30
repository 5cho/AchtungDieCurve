using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class ScoreboardSingleUI : MonoBehaviour
{
    [Header("Scripts references")]
    [SerializeField] private ScoreboardUI scoreboardUI;
    [Header("Scoreboard single template")]
    [SerializeField] private Transform template;

    private void Start()
    {
        scoreboardUI.OnScoreboardShown += ScoreboardUI_OnScoreboardShown;
    }

    private void ScoreboardUI_OnScoreboardShown(object sender, System.EventArgs e)
    {
        ClearScoreboard();

        // Get the dictionary of players on the scoreboard and set them in descending order
        Dictionary<int, int> scoreboardDictionary = AchtungGameManager.Instance.GetScoreboardDictionary().OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        // Spawn and setup a template for each player in the game
        foreach (var pair in scoreboardDictionary)
        {
            Transform scoreboardSingle = Instantiate(template, this.transform);
            scoreboardSingle.gameObject.SetActive(true);
            TextMeshProUGUI scoreboardSingleText = scoreboardSingle.GetComponentInChildren<TextMeshProUGUI>();
            scoreboardSingleText.text = "Player " + (pair.Key + 1) + ": " + pair.Value;
            scoreboardSingleText.color = AchtungGameManager.Instance.GetColorByIndex(pair.Key);
        }
    }
    public void ClearScoreboard()
    {
        // Destroy all children in the scoreboard
        foreach(Transform child in transform)
        {
            // Excluding the template gameobject from being destroyed from the scoreboard
            if(child == template)
            {
                continue;
            }

            Destroy(child.gameObject);
        }
    }
}
