using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    athena = 0,
    ares = 10,
    zeus = 20,
    hades = 30,
    aphrodite = 40,
    booster =100,
    obstacle=110,
}
[Serializable]
public class TileType 
{
  
    [SerializeField]private Type type;
    public Type tileType => type;
    public void SetTileType(int type)
    {
        this.type=(Type)type;
    }
}
