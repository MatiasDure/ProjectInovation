using System.Collections;
using UnityEngine;

public class RotateObj : MonoBehaviour
{
    [SerializeField] public float _speed;

    private void Update()
    {
        transform.Rotate(0, _speed * Time.deltaTime, 0);
    }
}