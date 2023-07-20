using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthInfo : MonoBehaviour
{
    [SerializeField] public Image playerIcon;
    [SerializeField] List<Image> hearts = new List<Image>();

    [SerializeField] Image notReady;
    [SerializeField] Image ready;

    public void Hearts()
    {
        foreach (var item in hearts)
        {
            item.gameObject.SetActive(true);
        }
        ready.gameObject.SetActive(false);
        notReady.gameObject.SetActive(false);
    }
    public void DecreaseLife(int health)
    {
        for (int i = 2; i > health-1; i--)
        {
            hearts[i].color = new Color(1, 1, 1, 0.2f);
        }
    }

    public void SetNotReady()
    {
        foreach (var heart in hearts)
        {
            heart.gameObject.SetActive(false);
        }
        notReady.gameObject.SetActive(true);
        ready.gameObject.SetActive(false);

    }

    public void SetReady()
    {
        notReady.gameObject.SetActive(false);
        ready.gameObject.SetActive(true);
    }
}
