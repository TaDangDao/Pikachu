using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class SimplePool
{
    private static Dictionary<EffectType, Pool> poolInstance = new Dictionary<EffectType, Pool>();
    // khoi tao pool moi
    public static void Preload(GameUnit prefab, int amount, Transform parent)
    {
        if (prefab == null)
        {
            return;
        }
        if (!poolInstance.ContainsKey(prefab.EffectType) || poolInstance[prefab.EffectType] == null)
        {
            Pool p = new Pool();
            p.PreLoad(prefab, amount, parent);
            poolInstance[prefab.EffectType] = p;
        }
    }
    // lay phan tu ra
    public static T Spawn<T>(EffectType EffectType, Vector3 pos, Quaternion rot) where T : GameUnit
    {
        if (!poolInstance.ContainsKey(EffectType))
        {
            return null;
        }
        return poolInstance[EffectType].Spawn(pos, rot) as T;
    }
    // tra phan tu ve pool
    public static void Despawn(GameUnit gameUnit)
    {
        if (!poolInstance.ContainsKey(gameUnit.EffectType))
        {
            Debug.Log("!");
        }
        else
        {
            poolInstance[gameUnit.EffectType].Despawn(gameUnit);
        }
    }
    //  thu thap phan tu
    public static void Collect(EffectType type)
    {
        if (!poolInstance.ContainsKey(type))
        {
            Debug.Log("!");
        }
        else
        {
            poolInstance[type].Collect();
        }
    }
    // thu thap tat ca
    public static void CollectAll()
    {
        foreach (var item in poolInstance.Values)
        {
            item.Collect();
        }
    }
    // xoa 1 pool
    public static void Release(EffectType type)
    {
        if (!poolInstance.ContainsKey(type))
        {
            Debug.Log("!");
        }
        else
        {
            poolInstance[type].Release();
        }
    }
    // xoa tat ca pool
    public static void ReleaseAll()
    {
        foreach (var item in poolInstance.Values)
        {
            item.Release();
        }
    }
}
public class Pool
{
    Transform parent;
    GameUnit prefab;
    //list chua cac unit dang o trong pool
    Queue<GameUnit> inactives = new Queue<GameUnit>();
    // list chua cac unit dang dc su dung
    List<GameUnit> actives = new List<GameUnit>();
    // khoi tao pool
    public void PreLoad(GameUnit prefab, int amount, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
        Debug.Log(amount);
        for (int i = 0; i < amount; i++)
        {
            Despawn(GameObject.Instantiate(prefab, parent));
        }
    }
    // lay phan tu tu ppol
    public GameUnit Spawn(Vector3 pos, Quaternion rot)
    {
        GameUnit unit;
        if (inactives.Count <= 0)
        {
            unit = GameObject.Instantiate(prefab, pos, rot, parent);
            return unit;
        }
        else
        {
            unit = inactives.Dequeue();
            unit.gameObject.SetActive(true);
        }
        unit.TF.SetPositionAndRotation(pos, rot);
        actives.Add(unit);
        return unit;
    }
    // tra phan tu ve pool
    public void Despawn(GameUnit gameUnit)
    {
        if (gameUnit != null && gameUnit.gameObject.activeInHierarchy)
        {
            actives.Remove(gameUnit);
            inactives.Enqueue(gameUnit);
            gameUnit.gameObject.SetActive(false);
        }
    }
    // thu thap tat ca pha tu dang dung ve pool
    public void Collect()
    {
        while (inactives.Count > 0)
        {
            Despawn(actives[0]);
        }
    }
    // destroy tat ca phan tu
    public void Release()
    {
        Collect();
        while (inactives.Count > 0)
        {
            GameObject.Destroy(inactives.Dequeue().gameObject);
        }
        inactives.Clear();
    }
}