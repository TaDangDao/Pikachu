using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class JsonHelper
{
    // lay data cua mang 1D tu json
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }
    // lay so luong hang va cot
    public static Tuple<int,int> FromJsonColsAndRows<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return new Tuple<int,int>(wrapper.cols, wrapper.rows);
    }
    // chuyen doi tu lop wrapper sang json
    public static string ToJson<T>(T[] array,int cols, int rows, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        wrapper.cols = cols;
        wrapper.rows = rows;
        Debug.Log(wrapper.Items.Length);
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }


    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
        public int rows;
        public int cols;
    }

}

