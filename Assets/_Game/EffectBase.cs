using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBase : GameUnit
{
    protected Vector2 pos;
    [SerializeField] private RectTransform rectTransform;
    public void OnInit(Vector2 pos, Canvas parent)
    {
        rectTransform=GetComponent<RectTransform>();
        SetEffectPositionFromWorld(pos, parent);
        rectTransform.localScale = Vector3.one*2.3f;
        Invoke(nameof(Ondespawn), 0.5f);
    }
    public void Ondespawn()
    {
        SimplePool.Despawn(this);
    }
    public void SetEffectPositionFromWorld(Vector3 worldPos, Canvas canvas)
    {
        // chuyen doi tu world position sang screen position
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);

        // chuyen doi tu screen position sang local position trong canvas
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 localPos;
        // su dung RectTransformUtility de chuyen doi
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, canvas.worldCamera, out localPos);
        Debug.Log("Effect position: " + localPos);
        //  thiet lap local position cho rectTransform
        rectTransform.localPosition = localPos;
    }
}
