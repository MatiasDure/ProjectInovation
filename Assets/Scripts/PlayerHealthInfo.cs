using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthInfo : MonoBehaviour
{
    [SerializeField] public Image playerIcon;
    [SerializeField] List<Image> hearts = new List<Image>();

    public void DecreaseLife(int health)
    {
        for (int i = 2; i > health-1; i--)
        {
            hearts[i].color = new Color(1, 1, 1, 0.2f);
        }
    }
}
