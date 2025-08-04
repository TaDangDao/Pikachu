using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSizeScale : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private CanvasScaler canvasScaler;
    private void Awake()
    {
        // chinh kich thuoc camera va canvas scaler
        float windowAspect = (float)Screen.width / (float)Screen.height;
        Debug.Log("Window Aspect Ratio: " + windowAspect);
        // dieu chinh orthographicSize de phu hop
        if (windowAspect< 0.4f)
            {
                cam.orthographicSize =15f;
                canvasScaler.matchWidthOrHeight = 0.5f;
            }
            else if (windowAspect >= 0.4f && windowAspect <= 0.7f)
            {
                 cam.orthographicSize = 11f;
                 canvasScaler.matchWidthOrHeight = 0;
            }
            else
            {
                 cam.orthographicSize = 12f;
                 canvasScaler.matchWidthOrHeight = 1f;
                 //cam.transform.position = new Vector3(0, -0.8f, -10);
            }
    }

}
