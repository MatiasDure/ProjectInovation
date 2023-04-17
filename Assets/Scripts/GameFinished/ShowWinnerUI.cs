using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        winnerName.text = FindCharacterName(winner);

        string[] players = WinnerJson.ReadString("players");

        List<string> losers = FindLosers(winner, players);

        int iteration = 0; 
        foreach(string loser in losers)
        {
            foreach(var loserObj in losersObjs)
            {
                if (loserObj.parentObj.activeInHierarchy) continue;

                loserObj.parentObj.SetActive(true);
                loserObj.loserName.text = FindCharacterName(loser);
                loserObj.loserImageComponent.sprite = FindImageOfChar(loser);
                loserObj.transformObj.SetPosition(GetPositionByIndex(losers.Count, iteration));
                loserObj.transformObj.SetScale(GetScaleByLoserCount(losers.Count));
                iteration++;

                break;
            }
        }
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

    private string FindCharacterName(string character)
    {
        switch(character)
        {
            case "charA":
                return "Bob";
            case "charB":
                return "Steve";
            case "charC":
                return "Ross";
            case "charD":
                return "Dave";
            default:
                return "";
        }
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