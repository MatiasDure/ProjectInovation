using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObjectTransform : MonoBehaviour
{

    [SerializeField] Transform toFollow;


    private void FixedUpdate()
    {
        transform.position = toFollow.position; 
    }
}
