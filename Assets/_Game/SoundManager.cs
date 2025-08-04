using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;


public enum SoundType
{
    MATCH = 0,
    MISS = 1,
    CLICK=2,
    PAUSE=3,
    UNPAUSE=4,
    UIBUTTON=5,
    HINT=6,
    SHOW2TILE=7,
    BOOSTER_1=8,
    VICTORY=9,
    FAIL=10,

}
[Serializable]
 public struct SoundList
{
    public string name;
    [SerializeField] private AudioClip[] soundClips;
    public AudioClip[] SoundClips => soundClips;
}

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance => instance;
    [SerializeField] private float backgroundVolume;
    [SerializeField] private float sfxVolume;
    [SerializeField] private AudioSource effectSound;
    [SerializeField] private AudioSource bgSound;
    [SerializeField] private SoundList[] soundEffects;
    [SerializeField] private AudioClip[] soundBackgrounds;
    private const string SFX_VOLUME = "SFX_Volume";
    private const string BG_VOLUME = "BG_Volume";
    private void Awake()
    {
        instance = this;
        
        // Ensure this GameObject has an AudioListener if none exists
        if (GetComponent<AudioListener>() == null)
        {
            // Check if there are any AudioListeners in the scene
            AudioListener[] audioListeners = FindObjectsOfType<AudioListener>();
            if (audioListeners.Length == 0)
            {
                // No AudioListener found, add one to this SoundManager
                gameObject.AddComponent<AudioListener>();
                Debug.Log("Added AudioListener to SoundManager");
            }
        }
    }
    // Play effect sound 
   public static void PlaySoundEffect(SoundType type, float volume=1)
   {
        instance.effectSound.volume = instance.GetSFXVolume();
        instance.effectSound.PlayOneShot(instance.soundEffects[(int)type].SoundClips[0], instance.GetSFXVolume());
   }
    // tat effect sound
    public static void StopSoundEffect()
    {
        instance.effectSound.Stop();
    }
    // play random background sound
   public static void PlayBackgroundSound(float volume=1)
   {     
        instance.bgSound.clip = instance.soundBackgrounds[UnityEngine.Random.Range(0,instance.soundBackgrounds.Length)];
        instance.bgSound.loop = true;
        instance.bgSound.volume = instance.GetBGVolume();
        instance.bgSound.Play();
   }
    // dung bg sound
    public static void StopSoundBG()
    {
        instance.bgSound.Stop();
    }
    // chinh sua do to cua am thanh bg
    public void ChangeBackgroundVolume(float volume)
    {
        instance.bgSound.volume=volume;
    }
   // chinh sua do to cua am thanh effect
    public void ChangeEffectVolume(float volume)
    {
        instance.effectSound.volume=volume;
    }
    // luu thay doi cua setting
    public void SaveBGVolumeSetting(float volume)
    {
        PlayerPrefs.SetFloat(BG_VOLUME, volume);
    }
    public void SaveSFXVolumeSetting(float volume)
    {
        PlayerPrefs.SetFloat(SFX_VOLUME, volume);

    }
    public float GetBGVolume()
    {
        return backgroundVolume = PlayerPrefs.GetFloat(BG_VOLUME, 1);
    }
    public float GetSFXVolume()
    {
        return sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME, 1);
    }
/*
#if UNITY_EDITOR
    private void OnEnable()
    {
        string[]names=Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundEffects, names.Length);
        for (int i = 0; i < soundEffects.Length; i++) { 
           soundEffects[i].name = names[i];
        }
    }

#endif
*/
}
