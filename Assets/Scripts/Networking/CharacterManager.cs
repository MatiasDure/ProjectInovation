using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public enum Characters
    {
        none,
        charA,
        charB,
        charC,
        charD
    }

    public static CharacterManager Instance { get; private set; }

    [SerializeField] Image charA;
    [SerializeField] Image charB;
    [SerializeField] Image charC;
    [SerializeField] Image charD;

    List<string> selected = new List<string>();
    Dictionary<int, string> clientsChars = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        WebSocketClient.OnCharSelected += LockCharacter;
        WebSocketClient.OnChangedCharacter += ChangeCharacter;
        charA.gameObject.SetActive(false);
        charB.gameObject.SetActive(false);
        charC.gameObject.SetActive(false);
        charD.gameObject.SetActive(false);
    }

    void ChangeCharacter(int id, string charSelected)
    {
        foreach (KeyValuePair<int, string> clientChar in clientsChars)
        {
            if(clientChar.Key != id) continue;

            //free the character previously selected
            UnlockCharacter(clientChar.Value);
            //lock the character selected now
            LockCharacter(id, charSelected);
        }

        //setting new value for client
        clientsChars[id] = charSelected;
    }

    void UnlockCharacter(string currentChar)
    {
        SetCharaterStatus(currentChar, false);
    }

    void LockCharacter(int id, string charSelected)
    {
        Debug.Log(charSelected);
        SetCharaterStatus(charSelected, true);

        if (clientsChars.ContainsKey(id)) return;

        clientsChars.Add(id, charSelected);
        //Debug.LogWarning(clientsChars.Count);
    }

    void SetCharaterStatus(string charSelected, bool locked)
    {
        switch (charSelected)
        {
            case "charA":
                charA.gameObject.SetActive(locked);
                break;
            case "charB":
                charB.gameObject.SetActive(locked);
                break;
            case "charC":
                charC.gameObject.SetActive(locked);
                break;
            case "charD":
                charD.gameObject.SetActive(locked);
                break;
            default:
                break;
        }
    }

    public bool IsCharacterAvailable(string characterToCheck)
    {
        foreach(KeyValuePair<int, string> clientChar in clientsChars)
        {
            if (clientChar.Value.Equals(characterToCheck)) return false;
        }

        return true;
    }
}
