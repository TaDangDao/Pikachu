using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUnit : MonoBehaviour
{
    public EffectType effectType;
    public EffectType EffectType
    {
        get { return effectType; }
        private set { }
    }
    private Transform tf;
    public Transform TF
    {
        get
        {
            if (tf == null)
            {
                tf = transform;
            }
            return tf;
        }
    }
}
