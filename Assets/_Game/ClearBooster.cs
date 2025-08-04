using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBooster : BoosterBase
{
   public override void OnActivate()
  {
    base.OnActivate();
    GameManager_.Instance.ClearButtonBooster();

  }
}
