using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance { get; private set; }

    [SerializeField] List<Transform> players;
    [SerializeField] float followStrength;

    float camOgZPos;
    float camZoom;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        camOgZPos = transform.position.z;
    }

    private void FixedUpdate()
    {
        //Avoid processing positions if characters havent been assigned yet
        if (players.Count < 1) return;

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
    public void AddPlayerToFollow(PlayerMovement player)
    {
        players.Add(player.transform);
    }
    public void AddPlayerToFollow(Transform player)
    {
        players.Add(player);
    }
    public void RemovePlayerToFollow(Transform player)
    {
        players.Remove(player);
    }
    public void RemovePlayerToFollow(PlayerMovement player)
    {
        players.Remove(player.transform);
    }
}
