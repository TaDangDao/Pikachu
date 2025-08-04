using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CanvasFail : UICanvas
{
    [SerializeField] private TextMeshProUGUI bestScoreTMP;
    [SerializeField] private TextMeshProUGUI scoreTMP;
    public override void SetUp()
    {
        SoundManager.StopSoundBG();
        SoundManager.PlaySoundEffect(SoundType.FAIL);
        base.SetUp();
    }
    public void MainMenuButton()
    {
        UIManager_.Instance.CloseAll<UICanvas>();
        UIManager_.Instance.Open<CanvasMenu>();
    }
    public void RetryButton()
    {
        Close(0);
        GameManager_.Instance.ButtonRetry();
    }
    public void SetScore(int Score)
    {
        scoreTMP.SetText(Score.ToString());
    }
    public void SetBestScore(int bestScore)
    {
        bestScoreTMP.SetText(bestScore.ToString());
    }
}
