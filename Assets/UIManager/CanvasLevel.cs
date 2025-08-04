using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasLevel : UICanvas
{
    [SerializeField]List<Image> lockImageLevel = new List<Image>();
    [SerializeField] int maxLevel;
    [SerializeField] private GameObject lockMessenger;
    public override void SetUp()
    {
        base.SetUp();
        maxLevel = GameManager_.Instance.GetHighestLevel();
        for (int i = 0; i < lockImageLevel.Count; i++)
        {
            if (i < maxLevel)
            {
                // set level da mo
                lockImageLevel[i].gameObject.SetActive(false);
            }
        }
    }
    public void SelectLevel(int level)
    {
        if (level <= maxLevel)
        {
            // dat level hien tai va bat dau game
            GameManager_.ChangeCurrentLevel(level);
            GameManager_.Instance.RestartGame();
            Close(0);
            UIManager_.Instance.Open<CanvasGamePlay>();
        }
        else
        {
            // neu level lon hon max level thi hien thi messenger
            StartCoroutine(ShowLockMessengerCoroutine());
        }
    }
    // ham hien thi messenger khi chon level lon hon max level
    public IEnumerator ShowLockMessengerCoroutine()
    {
        lockMessenger.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        lockMessenger.SetActive(false);
        yield return null;
    }
}
