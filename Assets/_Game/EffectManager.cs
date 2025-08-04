using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    VOIDSHIELD = 0,
    FIREWORK=1,
    ELECTRIC=2,
    FIRE=3,
    HOLYCROSS=4,
    EXPLOSION=5,
    WATER = 6,
    WINDGROUNDS =7,
}
public class EffectManager : MonoBehaviour
{
    [SerializeField] private GameObject[] effectPrefabs;
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        for(int i = 0; i < 6; i++)
        {
            SimplePool.Preload(effectPrefabs[i].GetComponent<EffectBase>(), 2, transform);
        }
    }
    public void SpawnEffect(Type type, Vector2 pos)
    {
    //    //Debug.Log("spawn effect: " + type + " at position: " + pos);
    //    int index =Mathf.Clamp((int)type/10, 0, effectPrefabs.Length - 1);
    //    EffectBase e = SimplePool.Spawn<EffectBase>((EffectType)index, pos, Quaternion.identity);
    //    //Debug.Log("Spawned effect: " + e.EffectType + " at position: " + pos);
    //    e.OnInit(pos,canvas);
          StartCoroutine(SpawnEffectDelayCoroutine(type, pos));
    }
    public IEnumerator SpawnEffectDelayCoroutine(Type type, Vector2 pos)
    {
        yield return new WaitForSeconds(0.15f);
        //Debug.Log("spawn effect: " + type + " at position: " + pos);
        int index = Mathf.Clamp((int)type / 10, 0, effectPrefabs.Length - 1);
        EffectBase e = SimplePool.Spawn<EffectBase>((EffectType)index, pos, Quaternion.identity);
        //Debug.Log("Spawned effect: " + e.EffectType + " at position: " + pos);
        e.OnInit(pos, canvas);
    }
}
