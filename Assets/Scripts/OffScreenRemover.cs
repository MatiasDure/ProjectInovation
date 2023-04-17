using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffScreenRemover : MonoBehaviour
{
    [SerializeField] RectTransform canvasRect;
    Dictionary<PlayerMovement, Coroutine> toKill = new Dictionary<PlayerMovement, Coroutine>(); 
    Dictionary<PlayerMovement, DeathIndicator> deathIndicators = new Dictionary<PlayerMovement, DeathIndicator>();
    [SerializeField] DeathIndicator deathPrefab;
    [SerializeField] Transform deathContainer;
    [SerializeField] Camera cam;
    [SerializeField] Canvas canvas;

    public static OffScreenRemover Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this; 
    }

    // Start is called before the first frame update
    void Start()
    {
        List<PlayerMovement> players = Spline.Instance.players;
        foreach (var player in players)
        {
            DeathIndicator newDeathIndicator = Instantiate(deathPrefab, deathContainer);
            newDeathIndicator.cam = cam;
            newDeathIndicator.canvas = canvas;
            newDeathIndicator.player = player.transform;
            newDeathIndicator.SetFishImage(player.info.Sprite);
            deathIndicators.Add(player,newDeathIndicator); 
        }

        //then you calculate the position of the UI element
        //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

    }

    public void InstaKill(PlayerMovement player)
    {
        deathIndicators[player].instaKill = true;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        List<PlayerMovement> players = Spline.Instance.players; 
        foreach (var player in players)
        {
            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(player.transform.position);
            if(ViewportPosition.x < 0 || ViewportPosition.y < 0 || ViewportPosition.x > 1 || ViewportPosition.y > 1)
            {
                if (!toKill.ContainsKey(player))
                {
                    Coroutine deathrow = StartCoroutine(Deathrow(player));
                    toKill.Add(player, deathrow);
                    if (player.gameObject.activeInHierarchy) deathIndicators[player].enableIndicator = true;
                    else deathIndicators[player].enableIndicator = false;
                }
            }
            else if (toKill.Count > 0)
            {
                foreach (var finnaDie in toKill)
                {
                    if(player == finnaDie.Key)
                    {
                        toKill.Remove(finnaDie.Key);
                        StopCoroutine(finnaDie.Value);
                        deathIndicators[player].enableIndicator = false;
                        return;
                    }
                }
            }
        }

    }

    IEnumerator Deathrow(PlayerMovement player)
    {
        yield return new WaitForSeconds(2);
        foreach (var loser in toKill)
        {
            if (player == loser.Key && player.gameObject.activeInHierarchy)
            {
                //KILLL

                CameraFollow.instance.RemovePlayerToFollow(player);
                player.gameObject.SetActive(false);
                player.waterBag.gameObject.SetActive(false);
                if (player.health <= 1)
                {
                    // FULL DEATH
                    Debug.Log("Player " + player.name + " Died ");
                }
                else CheckPointManager.Instance.deactivatedPlayers.Add(player);
                CameraFollow.instance.CheckIfEveryoneIsDead();
                player.health--;
                player.LostHealth();
                player.healthInfo.DecreaseLife(player.health);
                break;
            }
        }
    }
}
