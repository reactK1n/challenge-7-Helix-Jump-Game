using LootLocker.Requests;
using System.Collections;
using TMPro;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    const string leaderBoardID = "18312";

    [SerializeField]
    private TextMeshProUGUI playerNames;

    [SerializeField]
    private TextMeshProUGUI playerScores;

    [SerializeField]
    private GameObject gameOverUI;

    [SerializeField]
    private GameObject leaderBoardPanelUI;

    public void ActivateLeaderBoard()
    {
        StartCoroutine(FetchTopHighestScoreRoutine());

        gameOverUI.SetActive(false);
        leaderBoardPanelUI.SetActive(true);
    }

    public void DeactivateLeaderBoard()
    {
        leaderBoardPanelUI.SetActive(false);
        gameOverUI.SetActive(true);
    }

    public IEnumerator SubmitScoreRoutine(int scoreToUpload)
    {
        bool done = false;
        string playerId = PlayerPrefs.GetString("PlayerID");
        LootLockerSDKManager.SubmitScore(playerId, scoreToUpload, leaderBoardID, (response) =>
        {
            if (response.success)
            {
                Debug.Log("successfully uploaded score");
                done = true;
            }
            else
            {
                Debug.Log($"failed {response.errorData.message}");
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator FetchTopHighestScoreRoutine()
    {
        bool done = false;
        LootLockerSDKManager.GetScoreList(leaderBoardID, 10, 0, (response) =>
        {
            if (response.success)
            {
                string playerNames = "Names\n";
                string playerScores = "Scores\n";

                var members = response.items;

                foreach (var member in members)
                {
                    playerNames += member.rank + ". ";

                    playerNames += !string.IsNullOrEmpty(member.player.name) ? member.player.name : member.player.id.ToString();

                    playerScores += member.score + "\n";

                    playerNames += "\n";
                }
                done = true;
                this.playerNames.text = playerNames;
                this.playerScores.text = playerScores;

            }
            else
            {
                Debug.Log($"failed {response.errorData.message}");
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);
    }
}
