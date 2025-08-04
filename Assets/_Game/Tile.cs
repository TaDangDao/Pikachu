using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class Tile:MonoBehaviour
{
    [SerializeField] protected SpriteRenderer img;
    [SerializeField] protected GameObject selected;
    [SerializeField] protected BoxCollider2D box_collider2D;
    [SerializeField] protected TileType type;
    [SerializeField] protected Vector2 pos;
    [SerializeField] protected bool isEmpty;
    [SerializeField] protected bool isSelected;
    [SerializeField] protected Animator animatorController;
    protected TileData data;
    public TileData Data => data;
    private Coroutine currentCoroutine;
    private const float TIME_MOVE_DOWN = 1f;
    private float timeDownCounter;
    public virtual void OnInit()
    {
        isEmpty = false;
        img.gameObject.SetActive(true);
        box_collider2D.enabled = true;
        timeDownCounter = 0;
    }
    public virtual void OnDeSpawn()
    {
        HideSelected();
        img.gameObject.SetActive(false);
        box_collider2D.enabled= false;
    }
    public virtual void SetTileData(TileData data)
    {
        this.data=data;
        this.type.SetTileType((int)data.TileType.tileType);
        SetImage(data.tileSprite);
    }
    public virtual bool IsSelected()
    {
        return isSelected==true;
    }
    public  virtual bool IsEmpty()
    {
        return isEmpty==true;
    }
    public virtual void SetEmpty(bool isEmpty)
    {
        this.isEmpty = isEmpty;
        if (IsEmpty()) { 
            OnDeSpawn();
        }
    }
    public virtual void SetTileType(int type)
    {
        this.type.SetTileType(type);
    }
    public virtual int GetTileType()
    {
        return (int)this.type.tileType;
    }
    public virtual void SetPosition(int x, int y)
    {
        pos.x = x;
        pos.y = y;
        //transform.localPosition = pos;
    }
    public virtual Vector2 GetPos()
    {
        return transform.position;
    }
    public virtual Vector2 GetLocalPos()
    {
        return pos;
    }
    public virtual void SetImage(Sprite sprite)
    {
        this.img.sprite=sprite;
    }
    public virtual void SetSelected(bool isSelected)
    {
        this.isSelected = isSelected;
    }
    public virtual void ShowSelected()
    {
        selected.SetActive(true);
        isSelected = true;
    }
    public virtual void HideSelected()
    {
        selected.SetActive(false);
        isSelected = false;
    }
    public virtual IEnumerator MoveDown(float delay)
    { yield return new WaitForSeconds(delay);

        while (timeDownCounter < TIME_MOVE_DOWN)
        {
            timeDownCounter += Time.deltaTime;
            transform.localPosition = Vector2.Lerp(transform.localPosition, pos, timeDownCounter / TIME_MOVE_DOWN);
            yield return null;
        }
        transform.localPosition = pos;
    }
    public virtual void PopUpAndDownAnimation()
    {
        StartCoroutine(ScaleUpCoroutine());
    }
    public virtual IEnumerator ScaleUpCoroutine()
    {
        Vector3 formerScale_tile1 = transform.localScale;
        int originalSortingOrder = img.sortingOrder;
        img.sortingOrder = originalSortingOrder+1;
        for (int i = 0; i < 20; i++)
        {
            transform.localScale = transform.localScale * 1.01f;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < 20; i++)
        {
            transform.localScale = transform.localScale / 1.01f;
            yield return new WaitForSeconds(0.01f);
        }
        img.sortingOrder = originalSortingOrder;
        yield return null;
    }
    public virtual Sprite GetImgSprite()
    {
        return this.img.sprite;
    }
    public virtual void SwapTile(TileData tileData)
    {
        //this.img.sprite = sprite;
        //SetTileType(type);
        SetTileData(tileData);

    }
    public virtual void Shake()
    {
        animatorController.SetTrigger("Shake");
    }
    public void BlowUp()
    {
        if (currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(BlowUpCoroutine());
        }
    }
    public virtual IEnumerator BlowUpCoroutine()
    {
        Vector3 formerScale_tile1 = transform.localScale;
        int originalSortingOrder = img.sortingOrder;
        img.sortingOrder = originalSortingOrder + 1;
        box_collider2D.enabled=false;
        this.isEmpty = true;
        for (int i = 0; i < 20; i++)
        {
            transform.localScale = transform.localScale * 1.01f;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < 20; i++)
        {
            transform.localScale = transform.localScale / 1.01f;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.02f);
        SetEmpty(true);
        currentCoroutine = null;
        yield return null;

    }
    public void SpecialPower(Type type)
    {
        // suc manh moi type
        switch(type){
            case Type.athena:
                break;
            case Type.ares:
                break;
            case Type.aphrodite:
                break;
            case Type.zeus:
                break;
            case Type.hades:
                break;
            case Type.obstacle:
                break;
        }
    }

}
