using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasGamePlay : UICanvas
{
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private Animator[] animators;
    private int randomIndex;
    public override void SetUp()
    {
        base.SetUp();
        // random transition animtion khi bat dau game 
        randomIndex = Random.Range(0, animators.Length);
        animators[randomIndex].gameObject.SetActive(true);
        //.SetTrigger("FadeIn");
        UpdateLevel(GameManager_.GetCurrentLevel());
        Invoke(nameof(ResetAnimatorTrigger), 1f);
    }
    public void UpdateLevel(int level)
    {
        // set text level
        currentLevel.SetText(level.ToString());
    }
    public void SettingButton()
    {
        // mo canvas setting
        UIManager_.Instance.Open<CanvasSetting>().SetState(this);
        GameManager_.Instance.ChangeGameStateToPause();
    }
    public void ResetAnimatorTrigger()
    {
        // tat animation
        animators[randomIndex].gameObject.SetActive(false);
        // ResetTrigger("FadeIn");
    }
}
