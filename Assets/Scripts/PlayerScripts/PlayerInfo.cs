using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] string _charName;
    [SerializeField] Sprite _sprite; 

    public string CharName => _charName;
    public Sprite Sprite => _sprite;
}
