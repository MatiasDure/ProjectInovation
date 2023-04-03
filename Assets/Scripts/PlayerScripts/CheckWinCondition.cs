using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class CheckWinCondition : MonoBehaviour
{
    SphereCollider sphereCollider;

    public static event Action<string> OnPlayerWon;
    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent<PlayerInfo>(out PlayerInfo info)) return;
        WinnerJson.WriteString("winner",info.CharName, false);
        OnPlayerWon?.Invoke(info.CharName);
    }
}
