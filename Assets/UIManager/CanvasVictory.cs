using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasVictory : UICanvas
{
    [SerializeField] private TextMeshProUGUI bestScoreTMP;
    [SerializeField] private TextMeshProUGUI scoreTMP;
    public override void SetUp()
    {
        SoundManager.StopSoundBG();
        SoundManager.PlaySoundEffect(SoundType.VICTORY);
        base.SetUp();
    }
    public void MainMenuButton()
    {
        UIManager_.Instance.CloseAll<UICanvas>();
        UIManager_.Instance.Open<CanvasMenu>();
    }
    public void SetBestScore(int bestScore)
    {
        bestScoreTMP.SetText(bestScore.ToString());
    }
    public void SetScore(int Score)
    {
        scoreTMP.SetText(Score.ToString());
    }
    public void NextLevel() { 
       GameManager_.Instance.ButtonNextLevel();
        SoundManager.StopSoundEffect();
       Close(0);
    }
}
