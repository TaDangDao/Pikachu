using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct HintPair
{
    private Tile t1;
    private Tile t2;
    public Tile tile1=>t1;
    public Tile tile2=>t2;
    //public Mid_Line path;
    //public LineRenderer Test;
    public HintPair(Tile t1, Tile t2
        //, Mid_Line p
        //,LineRenderer Test
        )
    {
        this.t1 = t1;
        this.t2 = t2;
        // path = p;
        // this.Test = Test;
    }
    public bool Contains(Tile t)
    {
        return tile1 == t || tile2 == t;
    }
}
