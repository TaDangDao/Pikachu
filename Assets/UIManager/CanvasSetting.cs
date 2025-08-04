using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSetting : UICanvas
{
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private Slider bgVolume;
    [SerializeField] private Slider sfxVolume;
    private const string SFX_VOLUME = "SFX_Volume";
    private const string BG_VOLUME = "BG_Volume";
    private void Start()
    {
        bgVolume.onValueChanged.AddListener(ChangeBGVolume);
        sfxVolume.onValueChanged.AddListener(ChangeSFXVolume);
        bgVolume.value = SoundManager.Instance.GetBGVolume();
        sfxVolume.value = SoundManager.Instance.GetSFXVolume();
    }
    public void SetState(UICanvas canvas)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
        switch (canvas)
        {
            case CanvasMenu:
                buttons[2].gameObject.SetActive(true);
                break;
            case CanvasGamePlay:
                buttons[0].gameObject.SetActive(true);
                buttons[1].gameObject.SetActive(true);
                break;
            default:
                break;

        }
    }
    public void MainMenuButton()
    {
        UIManager_.Instance.CloseAll<UICanvas>();
        UIManager_.Instance.Open<CanvasMenu>();
        //GameManager_.Instance.RestartGame();
    }
    public void ContinueButton()
    {
        GameManager_.Instance.OnContinue();
        SaveSetting();
        Close(0);
    }
    public override void Close(float time)
    {
        base.Close(time);
        SaveSetting();
        
    }
    public void ChangeBGVolume(float _)
    {
        _=bgVolume.value;
        SoundManager.Instance.ChangeBackgroundVolume(_);
    }
    public void ChangeSFXVolume(float _)
    {
        _ = sfxVolume.value;
        SoundManager.Instance.ChangeEffectVolume(_);
    }
    public void SaveSetting()
    {
        SoundManager.Instance.SaveBGVolumeSetting(bgVolume.value);
        SoundManager.Instance.SaveSFXVolumeSetting(sfxVolume.value);
    }
}
