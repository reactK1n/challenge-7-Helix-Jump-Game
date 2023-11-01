using LootLocker.Requests;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public int best;
    public int score;
    public int currentStage;

    private bool isGameOver;

    public event EventHandler OnRestartLevel;

    public event EventHandler<NextLevelEventHandler> OnNextLevel;

    public class NextLevelEventHandler : EventArgs
    {
        public int level;
    }

    [SerializeField]
    private LeaderBoard leaderBoard;

    const string _androidAdUnitId = "Interstitial_Android";
    const string _iOsAdUnitId = "Interstitial_iOS";
    string _adUnitId;

    public static GameManager Instance { get; private set; }
    void Awake()
    {
        Advertisement.Initialize("5444641", true, this);
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsAdUnitId
            : _androidAdUnitId;

        if (Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
        best = PlayerPrefs.GetInt("HighestScore");
        StartCoroutine(LoginRoutine());
    }

    /*private void Start()
    {
        //StartCoroutine(LoginRoutine());
    }*/

    public bool IsGameOver => isGameOver;
    public void NextLevel()
    {
        var helix = FindObjectOfType<Helix>();

        if (helix.AllStages() > (currentStage + 1))// add one to currentStage because of the indexing starting from zero
            currentStage++;
        else
            currentStage = 0;

        FindObjectOfType<BallController>().ResetBall();

        helix.LoadStage(currentStage);

        OnNextLevel?.Invoke(this, new NextLevelEventHandler
        {
            level = currentStage
        });
    }

    IEnumerator LoginRoutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("Session started successfully");
                PlayerPrefs.SetString("PlayerID", response.player_id.ToString());
                done = true;
            }
            else
            {
                Debug.Log("could not start session");
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
        isGameOver = false;
        Time.timeScale = 1f;
    }

    public void UploadScoreToLeaderBoard()
    {
        StartCoroutine(leaderBoard.SubmitScoreRoutine(score));
    }

    public void GameOver()
    {
        isGameOver = true;
        UploadScoreToLeaderBoard();
        PulseGame();
        FindObjectOfType<UIManager>().ActivateGameOverUI();
        LoadAd();
    }

    public void QuitGame()
    {
        Application.Quit();
    }



    public void PulseGame()
    {
        //pause game to resume after closing the ads
        Time.timeScale = 0f;
    }

    public void RestartFromPreviousJump()
    {
        //Instance.score = 0;

        //invoking an event to make sure we update the best score when restarting
        OnRestartLevel?.Invoke(this, EventArgs.Empty);

        //FindObjectOfType<BallController>().ResetBall();

        
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        if(score > best)
        {
            best = score;
            PlayerPrefs.SetInt("HighestScore", best);
        }
    }

    // Load content to the Ad Unit:
    public void LoadAd()
    {
        Advertisement.Load(_adUnitId, this);
        ShowAd();
    }

    // Show the loaded content in the Ad Unit:
    public void ShowAd()
    {
        Advertisement.Show(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        // Optionally execute code if the Ad Unit successfully loads content.
    }

    public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
    {
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
        LoadAd();
    }

    public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
    {
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
    }

    public void OnUnityAdsShowStart(string _adUnitId) 
    { }
    public void OnUnityAdsShowClick(string _adUnitId) 
    { }
    public void OnUnityAdsShowComplete(string _adUnitId, UnityAdsShowCompletionState showCompletionState) 
    {
        //play game after resuming
        //Time.timeScale = 1f;

        //load stage after dismissing the ads
        FindObjectOfType<Helix>().LoadStage(currentStage);
    }

    public void OnInitializationComplete()
    {
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
    }
}
