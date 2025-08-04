using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="TileData",menuName ="SciptableObject/TileData")]
public class TileData :ScriptableObject
{
    [SerializeField]private TileType type;
    public TileType TileType => type;
    [SerializeField] private Sprite sprite;
    public Sprite tileSprite => sprite;

}
