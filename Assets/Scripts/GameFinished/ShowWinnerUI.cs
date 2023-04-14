using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class ShowWinnerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI winnerName;

    [SerializeField] TextMeshProUGUI[] losersTexts; 
    [SerializeField] Image[] losersImagesComponents;

    [SerializeField] Sprite[] loserSprites;
    [SerializeField] CharacterModels[] winnerPrefabs;
    [SerializeField] Transform winnerParentObj;
    [SerializeField] PositionScale[] differentPosScales;
    [SerializeField] LosersObjs[] losersObjs;

    // Start is called before the first frame update
    void Start()
    {
        string winner = WinnerJson.ReadString("winner")[0];
        //Debug.Log(winner);
        GameObject winnerObj = FindWinnerModel(winner);

        if (winnerObj != null)
        {
            Instantiate(winnerObj, winnerParentObj);
        }

        winnerName.text = winner;

        string[] players = WinnerJson.ReadString("players");

        List<string> losers = FindLosers(winner, players);

        int iteration = 0; 
        foreach(string loser in losers)
        {
            foreach(var loserObj in losersObjs)
            {
                if (loserObj.parentObj.activeInHierarchy) continue;

                loserObj.parentObj.SetActive(true);
                loserObj.loserName.text = loser;
                loserObj.loserImageComponent.sprite = FindImageOfChar(loser);
                loserObj.transformObj.SetPosition(GetPositionByIndex(losers.Count, iteration));
                loserObj.transformObj.SetScale(GetScaleByLoserCount(losers.Count));
                iteration++;

                break;
            }
        }

        //for (int i = 1; i < players.Length; i++)
        //{
        //    if (players[i].Equals("") || players[i] == winner) continue;

        //    losersTexts[i - 1].gameObject.SetActive(true);
        //    losersTexts[i - 1].text = players[i];

        //    foreach (var image in loserSprites)
        //    {
        //        if (players[i] + "Dead" == image.name)
        //        {
        //            losersImagesComponents[i - 1].gameObject.SetActive(true);
        //            losersImagesComponents[i - 1].sprite = image;

        //            Debug.Log("Players length " + players.Length);

        //            break;
        //        }
        //    }
        //}
    }

    private Vector3 GetScaleByLoserCount(int count)
    {
        return differentPosScales[count - 1].scale;
    }

    private Vector3 GetPositionByIndex(int amountPlayers, int index)
    {
        Vector3 pos = new Vector3();
        Debug.Log(index);

        switch(amountPlayers)
        {
            case 1:
                pos = differentPosScales[0].positions[0];
                break;
            case 2:
                pos = differentPosScales[1].positions[index];
                break;
            case 3:
                pos = differentPosScales[2].positions[index];
                break;
            default:
                pos = new(0, 0, 0);
                break;
        }

        return pos;
    }

    private Sprite FindImageOfChar(string loser)
    {
        foreach (var sprite in loserSprites)
        {
            if(loser + "Dead" == sprite.name)
                return sprite;
        }

        return null;
    }

    private List<string> FindLosers(string winner, string[] players)
    {
        List<string> losers = new();

        for (int i = 0; i < players.Length; i++)
        {
            if (string.IsNullOrEmpty(players[i]) || players[i].Equals(winner)) continue;

            losers.Add(players[i]);
        }

        return losers;
    }

    private GameObject FindWinnerModel(string winnerName)
    {
        foreach(var model in winnerPrefabs)
        {
            if (!model.characterName.Equals(winnerName)) continue;

            return model.characterModel;
        }

        return null;
    }
}

[System.Serializable]
public struct CharacterModels
{
    public string characterName;
    public GameObject characterModel;
}

[System.Serializable]
public struct PositionScale
{
    public Vector3 scale;
    public Vector3[] positions;
}

[System.Serializable]
public struct LosersObjs
{
    public GameObject parentObj;
    public TextMeshProUGUI loserName;
    public Image loserImageComponent;
    public TransformObject transformObj;
}