using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private TextMeshProUGUI bestScoreText;

    [SerializeField]
    private TextMeshProUGUI levelText;

    [SerializeField]
    private List<Image> lives;

    [SerializeField]
    private GameObject gameOverUI;

    private int index;

    void Start()
    {
        GameManager.Instance.OnRestartLevel += OnRestartLevel;

        GameManager.Instance.OnNextLevel += OnNextLevel;

        bestScoreText.text = $"Best: {PlayerPrefs.GetInt("HighestScore")}";

        levelText.text = $"Level: 1";
    }

    private void OnNextLevel(object sender, GameManager.NextLevelEventHandler e)
    {
        levelText.text = $"Level: {e.level + 1}";
    }

    private void OnRestartLevel(object sender, System.EventArgs e)
    {
        //bestScoreText.text = $"Best: {PlayerPrefs.GetInt("HighestScore")}";
        if(lives.Any() && index < 3)
        {
            Destroy(lives[index]);
            index++;
        }
        if(index == 3)
        {
            GameManager.Instance.GameOver();
        }
    }


    public void ActivateGameOverUI()
    {
        gameOverUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = $"Score: {GameManager.Instance.score}";
    }
}
