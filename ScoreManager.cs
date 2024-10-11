using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public float score;

    public Transform player;
    public TextMeshProUGUI scoreText;

    private void Update()
    {
        if (player.position.y > score)
        {
            score = player.position.y;
            scoreText.text = Mathf.FloorToInt(score).ToString() + " m";
        }
    }
}
