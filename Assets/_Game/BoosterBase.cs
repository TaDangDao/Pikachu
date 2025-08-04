using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoosterBase : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI countTMP;
    [SerializeField] protected int boosterCost = 0;
    public virtual void Start() {
        GameManager_.Instance.OnBoosterCountChanged += GameManager_OnBoosterCountChanged;
    }

    private void GameManager_OnBoosterCountChanged(object sender, System.EventArgs e)
    {
       OnUpdateCountText();
    }

    public virtual void OnActivate() {

        if (boosterCost > 0)
        {
            OnRemoveCount();
        }
    }
    public virtual void OnAddCount() {
        boosterCost++;
        OnUpdateCountText();
    }
    public virtual void OnRemoveCount() {
        boosterCost--;
        OnUpdateCountText();
    }
    public virtual void OnResetCount() {
        boosterCost = 0;
        OnUpdateCountText();
    }
    public virtual void OnUpdateCountText() {
        countTMP.text = GameManager_.Instance.BoosterCount.ToString();
    }

}
