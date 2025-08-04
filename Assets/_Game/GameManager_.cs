using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
public enum GameState
{
    Start = 0,
    Playing = 10,
    Pause = 20,
    EndGame = 30
}
public class GameManager_ : MonoBehaviour
{
    [SerializeField] private TileManager tileManager;
    [SerializeField] private Image timeCounter;
    [SerializeField] private float timeCount;
    [SerializeField] private float hintsCooldown;
    [SerializeField] private bool canShowHints;
    [SerializeField] private Image showhintCooldown;
    [SerializeField] private Image boosterProgressImg;
    [SerializeField] private int bestScore;
    [SerializeField] private TextMeshProUGUI streakTextMesh;
    [SerializeField] private TextMeshProUGUI scoreTextMesh;
    [SerializeField] private TextMeshProUGUI boosterCountTextMesh;
    [SerializeField] private int default_ScorePoints = 10;
    [SerializeField] private int streak;
    [SerializeField] private int boosterMultiple = 5;
    [SerializeField] private int streakCondition = 3;
    [SerializeField] private Animator blinkAnimator;
    public event EventHandler OnBoosterCountChanged;
    private bool canBlink;
    private static GameManager_ instance;
    public static GameManager_ Instance => instance;
    private static int currentLevel = 1;
    private int score;
    private int boosterCount;
    private int boosterProgression;
    public int BoosterCount=>boosterCount;
    private static string BEST_SCORE = "BestScore_";
    private float timeShowHintCount;
    private const string HIGHEST_LEVEL = "Highest_Level";
    private int highestLevel;

    private GameState state;
    public GameState State => state;
    private float timer;
    private void Awake()
    {
        instance = this;
        //tranh viec nguoi choi cham da diem vao man hinh
        Input.multiTouchEnabled = false;
        //target frame rate ve 60 fps
        Application.targetFrameRate = 60;
        //tranh viec tat man hinh
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        
        OnInit(GameState.Start);
    }
    // khoi tao cac gia tri ban dau
    public void OnInit(GameState state)
    {
        Debug.Log(PlayerPrefs.GetInt(HIGHEST_LEVEL));
        this.state = state;
        timer = timeCount;
        timeCounter.fillAmount = 1;
        hintsCooldown = 1f;
        timeShowHintCount = 0;
        canShowHints = true;
        highestLevel = PlayerPrefs.GetInt(HIGHEST_LEVEL,1);
        //SetScoreText(score);
        //SetStreakText(streak);
        SetUpScore();
        boosterProgressImg.fillAmount = 0;
        boosterProgression = 0;
        SetBoosterCountText(boosterCount);
        SoundManager.PlayBackgroundSound();
        canBlink = true;
        blinkAnimator.gameObject.SetActive(false);  
    }
    // Tiep tuc khi unpause
    public void OnContinue()
    {
        ChangeGameState(GameState.Playing);
        SoundManager.PlaySoundEffect(SoundType.UNPAUSE);
    }
    // doi level hien tai   
    public static void ChangeCurrentLevel(int level)
    {
        currentLevel = level;
    }
    // lay level hien tai
    public static int GetCurrentLevel()
    {
        return currentLevel ;
    }
    private void Update()
    {
        // neu dang choi thi moi cap nhat thoi gian
        if (state == GameState.Playing)
        {
            timer -= Time.deltaTime;
            timeCounter.fillAmount = timer / timeCount;
            if (timer < 0)
            {
                if (tileManager.CheckEndGame())
                {
                    ShowEndGameCanvas(1);
                }
                else
                {
                    ShowEndGameCanvas(0);
                }
                //tileManager.OnDespawn();
                timer = timeCount;
            }else if (canBlink && timeCounter.fillAmount < 0.3f)// neu thoi gian it hon 30% thi bat dau nhap nhay
            {
                blinkAnimator.gameObject.SetActive(true);
                canBlink = false;
            }
        }
        // cap nhat thoi gian cho cooldown hint
        if (!canShowHints)
        {
            timeShowHintCount += Time.deltaTime;
            showhintCooldown.fillAmount= timeShowHintCount / hintsCooldown;
            if (timeShowHintCount > hintsCooldown)
            {
                canShowHints = true;
                timeShowHintCount = 0;
            }
        }
    }
    //public void ChangeGameStateToPlaying()
    //{
    //    ChangeGameState(GameState.Playing);
    //    OnInit(GameState.Playing);
    //    tileManager.OnDespawn();
    //    tileManager.OnInit(currentLevel);
    //}

    // pause game
    public void ChangeGameStateToPause()
    {
        ChangeGameState(GameState.Pause);
        SoundManager.PlaySoundEffect(SoundType.PAUSE);
    }
    // chuyen trang thai game
    public void ChangeGameState(GameState state)
    {
        this.state = state;
    }
    // Hien thi canvas Endgame( hoac win hoac lose)
    public void ShowEndGameCanvas(int i)
    {
        string bestScoreOfCurrentLevel = BEST_SCORE + currentLevel.ToString();
        bestScore = PlayerPrefs.GetInt(bestScoreOfCurrentLevel, 0);// lay bestsoere
        if (i == 1)
        {
            SetBestScore(score);
            bestScore = PlayerPrefs.GetInt(bestScoreOfCurrentLevel, 0);
            ShowVictoryCanvas(bestScore);
        }
        else
        {
            ShowFailCanvas(bestScore);
        }
        SetUpScore();// reset lai score va streak hien tai
    }
    // hien thi canvas fail
    public void ShowFailCanvas(int bestScore )
    {
        ChangeGameState(GameState.EndGame);
        UIManager_.Instance.OpenAfter<CanvasFail>(0.5f).SetBestScore(bestScore);
        UIManager_.Instance.OpenAfter<CanvasFail>(0.5f).SetScore(score);
    }
    //hien thi canvas victory
    public void ShowVictoryCanvas(int bestScore)
    {
        ChangeGameState(GameState.EndGame);
        UIManager_.Instance.OpenAfter<CanvasVictory>(0.5f).SetBestScore(bestScore); 
        UIManager_.Instance.OpenAfter<CanvasVictory>(0.5f).SetScore(score);
        Debug.Log("Win");
    }
    // next level
    public void ButtonNextLevel()
    {
        currentLevel++;
        // cap nhat max level
        if (PlayerPrefs.GetInt(HIGHEST_LEVEL,1) < currentLevel)
        {
            PlayerPrefs.SetInt(HIGHEST_LEVEL, currentLevel);
        }
        SoundManager.PlayBackgroundSound();
        UIManager_.Instance.GetUI<CanvasGamePlay>().SetUp();
        RestartGame();
    }
    // cac button ma cac canvas khac goi de thuc hien logic
    public void ButtonRetry()
    {
        RestartGame();
    }

    public void ButtonShowHint()
    {
        if (canShowHints)
        {
            canShowHints = false;
            showhintCooldown.fillAmount = 0f;
            tileManager.ShowHints();
            SoundManager.PlaySoundEffect(SoundType.HINT);
            
        }
    }
    public void ClearButtonBooster()
    {
        if (boosterCount > 0)
        {
            tileManager.Booster();
        }
    }
      public void TimerButtonBooster()
    {
        if (boosterCount > 0)
        {          
            timer=Mathf.Clamp(0,timeCount,timer+50);
            timeCounter.fillAmount = timer / timeCount;
            blinkAnimator.gameObject.SetActive(false);
            canBlink=true;
            ChangeBoosterCount(-1);
            tileManager.CheckOddTile();
        }
    }

    // khoi tao lai game voi level hien tai 
    public void RestartGame()
    {
        OnInit(GameState.Playing);
        SetUpScore();
        tileManager.OnDespawn();
        tileManager.OnInit(currentLevel);
    }
    public bool CanShowHint()
    {
        return canShowHints == true;
    }
    public int GetHighestLevel()
    {
        return highestLevel;
    }
    // tinh toan score
    public void CalculateScore(int s=0)
    {
        score += default_ScorePoints + streak+s;
        // cu moi 3 strak thi  + progress 
        if (streak % 3 > 0)
        {
            boosterProgression += (streak % streakCondition) * boosterMultiple;
            boosterProgressImg.fillAmount=Mathf.Clamp((float)boosterProgression / 100,0,1);
            if (boosterProgression >= 100)
            {
                ChangeBoosterCount(1);
                boosterProgression = 0;
                boosterProgressImg.fillAmount = 0;
            }
        }
        Debug.Log("Score " + score);
        SetScoreText(score);
    }
    public void PlusStreak()
    {
        this.streak++;
        SetStreakText(streak);
    }
    public void ResetStreak()
    {
     this.streak = 0;
        SetStreakText(streak);
    }

    // kiem tra bestscore
    public void SetBestScore(int bestScore)
    {
        string bestScoreOfCurrentLevel = BEST_SCORE + currentLevel.ToString();
        if (bestScore> PlayerPrefs.GetInt(bestScoreOfCurrentLevel, 0))
        {
            PlayerPrefs.SetInt(bestScoreOfCurrentLevel, bestScore);
        }
    }
    public void SetScore(int score)
    {
        this.score = score;
    }
    // set up lai score va streak = 0
    public void SetUpScore()
    {
        score = 0;
        SetScoreText(score);
        ResetStreak();
    }
    public void SetStreakText(int streak)
    {
        streakTextMesh.SetText(streak.ToString());
    }
    public void SetScoreText(int score)
    {
        scoreTextMesh.SetText(score.ToString());
    }
    public void SetBoosterCountText(int boosterCount)
    {
        boosterCountTextMesh.SetText(boosterCount.ToString());
    }
    // phat su kien event moi khi co su thay doi ve so luong booster
    public void ChangeBoosterCount(int i)
    {
        boosterCount+=i;
        OnBoosterCountChanged?.Invoke(this, EventArgs.Empty);
        //SetBoosterCountText(boosterCount);
    }
}
