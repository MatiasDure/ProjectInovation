using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance { get; private set; }

    [SerializeField] List<Transform> players;
    [SerializeField] float followStrength;
    [SerializeField] int priorityPlayerIndex = -1;
    [SerializeField] float priorityValue = 0f;
    [SerializeField] float margin = 2f;

    float camOgZPos;
    float camZoom;
    public Transform firstPlace;

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

        foreach (var item in players)
        {
            playerPos += item.transform.position;
        }
        playerPos /= players.Count;


        Vector3 priorityPlayerPos = firstPlace.transform.position;
        playerPos = Vector3.Lerp(playerPos, priorityPlayerPos, priorityValue);
        

        float requiredDistance = CalculateRequiredDistance(players, margin);
        Vector2 lerpedPos = Vector2.Lerp(transform.position, playerPos, followStrength);
        camZoom = Mathf.Lerp(camZoom, requiredDistance, followStrength / 2f);
        transform.position = new Vector3(lerpedPos.x, lerpedPos.y, camOgZPos - camZoom);
    }

    float CalculateRequiredDistance(List<Transform> players, float margin)
    {
        float aspectRatio = Camera.main.aspect;
        float vFov = Camera.main.fieldOfView * Mathf.Deg2Rad;
        float hFov = 2 * Mathf.Atan(Mathf.Tan(vFov / 2) * aspectRatio);

        float angleOfView = hFov / 2;

        float maxX = float.MinValue;
        float minX = float.MaxValue;
        float maxY = float.MinValue;
        float minY = float.MaxValue;

        foreach (var player in players)
        {
            Vector3 playerPos = player.position;
            maxX = Mathf.Max(maxX, playerPos.x);
            minX = Mathf.Min(minX, playerPos.x);
            maxY = Mathf.Max(maxY, playerPos.y);
            minY = Mathf.Min(minY, playerPos.y);
        }

        float width = maxX - minX + 2 * margin;
        float height = maxY - minY + 2 * margin;

        float requiredDistanceX = width / (2 * Mathf.Tan(angleOfView));
        float requiredDistanceY = height / (2 * Mathf.Tan(angleOfView / aspectRatio));

        return Mathf.Max(requiredDistanceX, requiredDistanceY);
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
