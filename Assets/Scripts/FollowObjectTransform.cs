using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObjectTransform : MonoBehaviour
{

    public Transform toFollow;
    public SkinnedMeshRenderer _renderer; 


    private void FixedUpdate()
    {
        transform.position = toFollow.position;
        transform.rotation = toFollow.rotation;
    }
}
