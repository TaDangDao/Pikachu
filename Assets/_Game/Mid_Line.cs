using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mid_Line 
{
    private Vector2 Point1;
    private Vector2 Point2;
    public Mid_Line(Vector2 point1, Vector2 point2)
    {
        this.Point1=point1;
        this.Point2=point2;
    }
    public Vector2 GetPoint1 () { return this.Point1; }
    public Vector2 GetPoint2 () { return this.Point2; }
}
