using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowWinnerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI winnerName;


    [SerializeField] Image imagewinnerName;



    [SerializeField] List<TextMeshProUGUI> losersTexts = new List<TextMeshProUGUI>(); 
    [SerializeField] List<Image> losersImages = new List<Image>();


    [SerializeField] List<Sprite> images = new List<Sprite>(); 




    // Start is called before the first frame update
    void Start()
    {
        string winner = WinnerJson.ReadString("winner")[0];
        winnerName.text = winner;
        foreach (var image in images)
        {
            if (winner == image.name) imagewinnerName.sprite = image; 
        }
        string[] players = WinnerJson.ReadString("players");




        for (int i = 1; i < players.Length; i++)
        {
            if (players[i].Equals("") || players[i] == winner) continue;
            losersTexts[i-1].gameObject.SetActive(true);
            losersTexts[i-1].text = players[i];
            foreach (var image in images)
            {
                if (players[i] == image.name)
                {
                    losersImages[i-1].gameObject.SetActive(true);
                    losersImages[i-1].sprite = image;
                    break;
                }
            }
        }  
    }
}
