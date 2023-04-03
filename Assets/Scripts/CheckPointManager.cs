using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    [HideInInspector]
    public List<CheckPoint> checkPointPositions = new List<CheckPoint>();

    List<PlayerMovement> players = new List<PlayerMovement>();


    // Start is called before the first frame update
    void Start()
    {
        SpawnTriggers(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnTriggers()
    {
        foreach (var point in checkPointPositions)
        {
            BoxCollider checkpoint = gameObject.AddComponent<BoxCollider>();
            checkpoint.center = point.position + new Vector2(0, point.size.y / 2);
            checkpoint.size = new Vector3(1, point.size.y, 1);
            checkpoint.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement playerCol)) return;
        if (!players.Contains(playerCol)) players.Add(playerCol);
        foreach (var player in players)
        {
            if (player.gameObject == other.gameObject)
            {
                Debug.Log(FindClosestCheckPoint(player.transform.position));
            }
            else continue;

        }
    }

    int FindClosestCheckPoint(Vector3 position)
    {
        float closest = float.MaxValue;
        int index = 0; 
        for (int i = 0; i < checkPointPositions.Count; i++)
        {
            float distance = Vector3.Distance(checkPointPositions[i].position + new Vector2(0, checkPointPositions[i].size.y / 2), position);
            if (distance < closest)
            {
                closest = distance;
                index = i; 
            }
        }
        return index; 
    }

    void OnDrawGizmosSelected()
    {
        foreach (var checkPoint in checkPointPositions)
        {
            Gizmos.color = new Color(0, 0, 1, 0.5f);
            Gizmos.DrawCube(new Vector2(checkPoint.position.x, checkPoint.position.y + checkPoint.size.y / 2), new Vector3(checkPoint.size.x, checkPoint.size.y, 1));
        }
    }

}

[System.Serializable]
public class CheckPoint
{
    public Vector2 position;
    public Vector2 size;
    public int index;

    public CheckPoint(Vector2 Position, Vector2 Size, int Index)
    {
        position = Position;
        size = Size;
        index = Index;
    }
}
