using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] List<Transform> players;
    [SerializeField] float followStrength;

    float camOgZPos;
    float camZoom; 

    // Start is called before the first frame update
    void Start()
    {
        camOgZPos = transform.position.z;
    }

    private void FixedUpdate()
    {
        Vector3 playerPos = Vector3.zero;
        float distanceApart = 0;



        float IQR = DistanceApart(players); 


        foreach (var item in players)
        {
            playerPos += item.transform.position; 
        }
        playerPos /= players.Count; 

        Vector3 camPos = transform.position;

        Vector2 lerpedPos = Vector2.Lerp(camPos, playerPos, followStrength);
        camZoom = Mathf.Lerp(camZoom, IQR, followStrength/2);

        transform.position = new Vector3(lerpedPos.x,lerpedPos.y, camOgZPos - camZoom);


    }

    float DistanceApart(List<Transform> players)
    {
        Vector3 smallest = Vector3.one * 900;
        Vector3 biggest = Vector3.zero;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].position.magnitude > biggest.magnitude) biggest = players[i].position;
            if (players[i].position.magnitude < smallest.magnitude) smallest = players[i].position;
        }
        return (biggest-smallest).magnitude; 
    }
}
