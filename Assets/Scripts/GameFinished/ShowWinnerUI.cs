using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowWinnerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI winnerName;
    // Start is called before the first frame update
    void Start()
    {
        string winner = WinnerJson.ReadString("winner")[0];
        winnerName.text = winner;
        string[] players = WinnerJson.ReadString("players");
        foreach (string layer in players)
        {
            if (layer.Equals("") || layer.Equals(winner)) continue;
            Debug.LogWarning("Losers:" + layer);
        }
        
    }
}
