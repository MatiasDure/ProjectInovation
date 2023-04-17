using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPointManager : MonoBehaviour
{
    [HideInInspector]
    public List<CheckPoint> checkPointPositions = new List<CheckPoint>();

    public List<PlayerMovement> players = new List<PlayerMovement>();

    Dictionary<PlayerMovement, int> playerCheckpoint = new Dictionary<PlayerMovement, int>();

    public static CheckPointManager Instance;

    [HideInInspector]
    public List<PlayerMovement> deactivatedPlayers = new List<PlayerMovement>();

    int latestCheckpointReached = 0;

    [SerializeField] bool tutorialScene;
    int amountChecked = 0;
    private List<GameObject> readyPlayers = new List<GameObject>();

    public static event Action<string> OnPlayerWon;
    public static event Action OnFinishedTutorial;

    [SerializeField]
    GameObject checkpointPrefab; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnTriggers();
        Spline.Instance.SetFlagPositions(checkPointPositions);
    }

    public void AddPlayer(PlayerMovement player)
    {
        players.Add(player);
        playerCheckpoint.Add(player, 0);
        Debug.Log(playerCheckpoint.Count);
        Debug.Log("players : " + players.Count);

    }
    public void RemovePlayer(PlayerMovement player)
    {
        players.Remove(player);
        playerCheckpoint.Remove(player);
    }

    void SpawnTriggers()
    {
        for (int i = 0; i < checkPointPositions.Count; i++)
        {
            CheckPoint point = checkPointPositions[i];
            BoxCollider checkpoint = gameObject.AddComponent<BoxCollider>();
            checkpoint.center = point.position + new Vector2(0, point.size.y / 2);
            checkpoint.size = new Vector3(1, point.size.y, 1);
            checkpoint.isTrigger = true;

            if (i > checkPointPositions.Count - 2 || i == 0) continue;
            GameObject checkpointObject = Instantiate(checkpointPrefab);
            checkpointObject.transform.position = point.position; 
        }
    }

    public void SpawnPlayerAtCheckPoint(PlayerMovement player,int checkpoint)
    {
        player.transform.position = checkPointPositions[checkpoint].position + Vector2.up;
        player.gameObject.SetActive(true);
        player.waterBag.gameObject.SetActive(true);
        player.rb.velocity = Vector3.zero; 
        CameraFollow.instance.AddPlayerToFollow(player);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement playerCol)) return;
        
        if(tutorialScene)
        {
            //check if everyone got to the checkpoint
            foreach (var player in playerCheckpoint.Keys)
            {
                if (player.gameObject != other.gameObject) continue;
                
                int checkpointIndex = FindClosestCheckPoint(player.transform.position);

                if (checkpointIndex == checkPointPositions.Count - 1)
                {
                    if (readyPlayers.Contains(other.gameObject)) return;
                    
                    readyPlayers.Add(other.gameObject);
                    playerCol.healthInfo.SetReady();
                }

                break;
            }

            if (readyPlayers.Count < SimpleServerDemo.Instance.AmountClients) return;
           
            OnFinishedTutorial?.Invoke();
            return;
        }

        if (!players.Contains(playerCol)) players.Add(playerCol);

        foreach (var player in playerCheckpoint.Keys)
        {
            if (player.gameObject == other.gameObject)
            {
                int checkpointIndex = FindClosestCheckPoint(player.transform.position);
                Debug.Log(checkpointIndex);
                if (playerCheckpoint[player] > checkpointIndex) playerCheckpoint[player] = checkpointIndex;

                if (checkpointIndex > latestCheckpointReached) latestCheckpointReached = checkpointIndex;
                if (checkpointIndex == checkPointPositions.Count - 1)
                {
                    PlayerWin(player);
                    return;
                }
                if (player == Spline.Instance.lastPlace) StartCoroutine(LastPlayerReachedCheckPoint(0,checkpointIndex));
            };
        }
    }
    void PlayerWin(PlayerMovement player)
    {
        Debug.Log(player.info.CharName + " has won the game ! ");
        OnPlayerWon?.Invoke(player.info.CharName);
        SoundManager.Instance.PlaySound(SoundManager.Sound.Win);
        foreach (var playerMovement in playerCheckpoint.Keys)
        {
            if (playerMovement == player) continue;
            playerMovement.enabled = false; /// NEEDS MORE WORK, this doesnt neccesarialiy stop it from moving
            CameraFollow.instance.RemovePlayerToFollow(playerMovement);
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
    public void RespawnEveryone()
    {
        foreach (var deactivatedPlayer in deactivatedPlayers)
        {
            SpawnPlayerAtCheckPoint(deactivatedPlayer, latestCheckpointReached);
        }
        deactivatedPlayers.Clear();
    }


    IEnumerator LastPlayerReachedCheckPoint(float time, int checkoint)
    {
        yield return new WaitForSeconds(time);

        foreach (var deactivatedPlayer in deactivatedPlayers)
        {
            SpawnPlayerAtCheckPoint(deactivatedPlayer, checkoint);
        }
        deactivatedPlayers.Clear();
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
