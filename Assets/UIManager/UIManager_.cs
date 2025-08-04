using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager_ : Singleton<UIManager_>
{
    Dictionary<System.Type, UICanvas> active_Canvases = new Dictionary<System.Type, UICanvas>();
    Dictionary<System.Type, UICanvas> canvasPrefabs = new Dictionary<System.Type, UICanvas>();
    [SerializeField] private Transform parent;
    private void Awake()
    {
        // Load UI prefabs tu trong folder Resources
        UICanvas[] prefabs = Resources.LoadAll<UICanvas>("UI/");
        for (int i = 0; i < prefabs.Length; i++)
        {
            canvasPrefabs.Add(prefabs[i].GetType(), prefabs[i]);
        }
        Open<CanvasMenu>();
    }
    // mo canvas
    public T Open<T>() where T : UICanvas
    {
        T canvas = GetUI<T>();
        canvas.Open();
        return canvas;
    }
    public T OpenAfter<T>(float time) where T : UICanvas
    {
        T canvas = GetUI<T>();
        canvas.OpenAfter(time);
        return canvas;
    }
    // dong canvas sau 1 khoang time
    public void Close<T>(float time) where T : UICanvas
    {
        if (IsOpened<T>())
        {
            active_Canvases[typeof(T)].Close(time);
        }
    }
    // dong canvas lap tuc
    public void CloseDirect<T>() where T : UICanvas
    {
        if (IsOpened<T>())
        {
            active_Canvases[typeof(T)].CloseDirect();
        }
    }
    // kiem tra canvas da duoc tai chua
    public bool IsLoaded<T>() where T : UICanvas
    {
        return active_Canvases.ContainsKey(typeof(T)) && active_Canvases[typeof(T)] != null;
    }
    // kiem tra canvas duoc active chua
    public bool IsOpened<T>() where T : UICanvas
    {
        return IsLoaded<T>() && active_Canvases[typeof(T)].gameObject.activeSelf;
    }
    // lay canvas
    public T GetUI<T>() where T : UICanvas
    {
        if (!IsLoaded<T>())
        {
            T prefab = GetUIPrefab<T>();
            T canvas = Instantiate(prefab, parent);
            active_Canvases[typeof(T)] = canvas;
        }
        return active_Canvases[typeof(T)] as T;
    }
    // get prefabs;
    private T GetUIPrefab<T>() where T : UICanvas
    {
        return canvasPrefabs[typeof(T)] as T;
    }
    // dong tat ca canvas
    public void CloseAll<T>() where T : UICanvas
    {
        foreach (var canvas in active_Canvases.Values)
        {
            if (canvas != null && canvas.gameObject.activeSelf)
            {
                canvas.Close(0);
            }

        }
    }
}
