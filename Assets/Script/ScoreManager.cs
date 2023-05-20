using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    private Board board;
    [SerializeField] private TextMeshProUGUI scoreText;
    private int score;
    [SerializeField] private Image scoreBar;

    private void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void IncreaseScore(int amountToIncrease){
        score += amountToIncrease;
        if(board != null && scoreBar != null){
            int size = board.scoreGoals.Count;
            scoreBar.fillAmount = (float)score / (float)board.scoreGoals[size-1];
        }
        scoreText.text = score.ToString();
    }
}
