using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerBooster : BoosterBase
{
   public override void OnActivate()
    {
        base.OnActivate();
        GameManager_.Instance.TimerButtonBooster();
    }
}
