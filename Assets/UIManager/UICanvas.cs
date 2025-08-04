using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [SerializeField] bool isDestroyOnClose = false;
    private void Awake()
    {
        RectTransform rect = GetComponent<RectTransform>();
        float ratio = (float)Screen.width / (float)Screen.height;
        if (ratio > 2.1f)
        {
            // Xu ly tai tho trong Iphone
            Vector2 leftBottom = rect.offsetMin;
            Vector2 rightTop = rect.offsetMax;
            leftBottom.y = 0f;
            rightTop.y = -100f;

            rect.offsetMin = leftBottom;
            rect.offsetMax = rightTop;
        }
    }
    // Goi truoc khi canvas duoc active
    public virtual void SetUp()
    {

    }
    // Goi sau khi canvas duoc active
    public virtual void Open()
    {
        SetUp();
        gameObject.SetActive(true);
    }
    public virtual void OpenAfter(float time)
    {
        Debug.Log("timeAfter "+time);
        Invoke(nameof(Open), time);
    }
    // Goi khi muon dong sau 1 khoang time
    public virtual void Close(float time)
    {
        Invoke(nameof(CloseDirect), time);
    }
    // Goi khi muon dong lap tuc
    public virtual void CloseDirect()
    {
        if (!isDestroyOnClose)
        {

            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public virtual void ButtonClickSoundEffect()
    {
        SoundManager.PlaySoundEffect(SoundType.UIBUTTON);
    }
}
