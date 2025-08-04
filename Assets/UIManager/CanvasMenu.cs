using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMenu : UICanvas
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingButton;
    public override void SetUp()
    {
        base.SetUp();
        playButton.onClick.AddListener(() => {
            PlayButton();
        });
        settingButton.onClick.AddListener(() => {
            SettingButton();
        });
    }
    public void PlayButton()
    {
        Close(0);
        UIManager_.Instance.Open<CanvasLevel>();
    }
    public void SettingButton()
    {
        UIManager_.Instance.Open<CanvasSetting>().SetState(this);
    }
}
