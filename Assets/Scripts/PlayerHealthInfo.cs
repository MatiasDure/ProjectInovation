using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthInfo : MonoBehaviour
{
    [SerializeField] public Image playerIcon;
    [SerializeField] List<Image> hearts = new List<Image>();



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealth(int health)
    {
        for (int i = 2; i > health-1; i--)
        {
            hearts[i].color = new Color(1, 1, 1, 0.2f);
        }
    }
}
