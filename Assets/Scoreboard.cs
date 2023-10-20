using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] private ScoreboardUI scoreboardUI;
    [SerializeField] private Transform template;

    private void Start()
    {
        scoreboardUI.OnScoreboardShown += ScoreboardUI_OnScoreboardShown;
    }

    private void ScoreboardUI_OnScoreboardShown(object sender, System.EventArgs e)
    {
        ClearScoreboard();

        Dictionary<int, int> scoreboardDictionary = AchtungGameManager.Instance.GetScoreboardDictionary().OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

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
        foreach(Transform child in transform)
        {
            if(child == template)
            {
                continue;
            }

            Destroy(child.gameObject);
        }
    }
}
