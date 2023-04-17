using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInfo))]
public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField] float jumpForce;
    [SerializeField] float stickyDragForce;

    [SerializeField] float bounceAmount;

    [SerializeField] float windForce; 


    bool onStickySurface;


    [SerializeField] bool jumping;

    [SerializeField] bool grounded; 

    Vector3 surfaceNormal;

    Surface surface;
    Surface wind;

    public PlayerInfo info;


    public FollowObjectTransform waterBag;
    public RigidBodyToShader rbToShader;

    public PlayerHealthInfo healthInfo; 

    public int health = 3;

    public static event Action<PlayerMovement> OnPlayerLostHealth;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        healthInfo = Instantiate(healthInfo,ColorsHolder.Instance.healthInfoContainer);
        healthInfo.playerIcon.sprite = info.Sprite;
        if (SceneManager.GetActiveScene().name == "TutorialScene") healthInfo.SetNotReady();
    }

    public void Move(Vector3 direction) 
    {
        bool canPlay = true;
        if (!jumping)
        {
            rb.AddForce(direction * jumpForce, ForceMode.Impulse);
            if (onStickySurface)
            {
                canPlay = false;
                if (Vector3.Dot(direction, surfaceNormal) > 0.2f)
                {
                    canPlay = true;
                    rb.drag = 0;
                }
            }
        }
        else
        {
            direction = new Vector3(direction.x, 0, 0);
            rb.AddForce(direction * jumpForce, ForceMode.Impulse);
            if (onStickySurface)
            {
                canPlay = false;
                if (Vector3.Dot(direction, surfaceNormal) > 0.2f)
                {
                    canPlay = true;
                    rb.drag = 0;
                }
            }
        }

        if (canPlay) SoundManager.Instance.PlaySound(SoundManager.Sound.Jump);
    }

    public void LostHealth() => OnPlayerLostHealth?.Invoke(this);

    private void Update()
    {
        if (jumping) return;
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
        {
            jumping = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if(Vector3.Dot(Vector3.up, surfaceNormal) > 0.2f)rb.drag = 0;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            rb.AddForce(Vector3.left * jumpForce, ForceMode.Impulse);
            if (Vector3.Dot(Vector3.left, surfaceNormal) > 0.2f) rb.drag = 0;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            rb.AddForce(Vector3.down * jumpForce, ForceMode.Impulse);
            if (Vector3.Dot(Vector3.down, surfaceNormal) > 0.2f) rb.drag = 0;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            rb.AddForce(Vector3.right * jumpForce, ForceMode.Impulse);
            if (Vector3.Dot(Vector3.right, surfaceNormal) > 0.2f) rb.drag = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        jumping = false; // add normal check
        Surface tempSurface = collision.gameObject.GetComponent<Surface>();
        if (tempSurface == null) return;

        if(tempSurface.surfaceType == Surface.SurfaceType.Death)
        {

            //Kill player
            //OffScreenRemover.Instance.InstaKill(this);
            CameraFollow.instance.RemovePlayerToFollow(this);
            this.gameObject.SetActive(false);
            this.waterBag.gameObject.SetActive(false);
            if (this.health <= 1)
            {
                // FULL DEATH
                Debug.Log("Player " + this.name + " Died ");
                if (CheckPointManager.Instance.deactivatedPlayers.Count == 0 && CameraFollow.instance.CheckIfEveryoneIsDead()) {

                    

                    //SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                    //SimpleServerDemo.Instance.RefreshWebPage();
                }/// RESTART 
            }
            else CheckPointManager.Instance.deactivatedPlayers.Add(this);
            CameraFollow.instance.CheckIfEveryoneIsDead();
            this.health--;
            OnPlayerLostHealth?.Invoke(this);
            healthInfo.DecreaseLife(this.health);

            return;
        }

        ContactPoint contact = collision.contacts[0];
        surfaceNormal = contact.normal;
        if (tempSurface != null && tempSurface.surfaceType != Surface.SurfaceType.Wind) surface = tempSurface;
        //else if(tempSurface != null && tempSurface.surfaceType == Surface.SurfaceType.Wind) wind = tempSurface; 

        if (surface.surfaceType == Surface.SurfaceType.Sticky)
        {
            onStickySurface = true;
            rb.drag = surface.stickyDragForce;
            SoundManager.Instance.PlaySound(SoundManager.Sound.Stick);
        }
        else {
            rb.drag = 0;
            onStickySurface = false;
        }


        if(surface.surfaceType == Surface.SurfaceType.Bouncy)
        {
            float impactVelocity = Mathf.Abs(Vector3.Dot(collision.relativeVelocity, surfaceNormal));
            float bounceForce = impactVelocity;
            if (impactVelocity < surface.bounceAmount) bounceForce = surface.bounceAmount;
            rb.AddForce(surfaceNormal * bounceForce, ForceMode.VelocityChange);
            SoundManager.Instance.PlaySound(SoundManager.Sound.Bounce);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        jumping = false;
    }
    private void OnCollisionExit(Collision collision)
    {
        jumping = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Surface tempSurface = other.gameObject.GetComponent<Surface>();
        if (tempSurface != null) // Check if tempSurface is not null
        {
            if (tempSurface.surfaceType == Surface.SurfaceType.Wind)
            {
                wind = tempSurface;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(wind != null)
        {
            if (wind.surfaceType == Surface.SurfaceType.Wind)
            {
                float windforce = 1 / Vector3.Distance(transform.position, other.transform.position) * wind.windForce;
                rb.AddForce(other.transform.up * windforce, ForceMode.Force);
            }
        }
    }

}