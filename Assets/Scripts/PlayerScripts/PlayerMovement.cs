using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInfo))]
public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float jumpForce;
    [SerializeField] float stickyDragForce;

    [SerializeField] float bounceAmount;

    [SerializeField] float windForce; 


    bool onStickySurface;


    bool jumping;

    Vector3 surfaceNormal;

    float originalAngularDrag;

    Surface surface; 





    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalAngularDrag = rb.angularDrag; 
    }

    public void Move(Vector3 direction) 
    {
        jumping = true;
        rb.AddForce(direction * jumpForce, ForceMode.Impulse);
        if (onStickySurface) 
        { 
            if(Vector3.Dot(direction, surfaceNormal) > 0.2f) rb.drag = 0;
            else jumping = false;
        }
    }


    private void Update()
    {
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
        jumping = false;
        ContactPoint contact = collision.contacts[0];
        surfaceNormal = contact.normal;

        surface = collision.gameObject.GetComponent<Surface>();

        if (surface == null) return;


        if (surface.surfaceType == Surface.SurfaceType.Sticky)
        {
            onStickySurface = true;
            rb.drag = surface.stickyDragForce;
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
        }








    }
    private void OnCollisionExit(Collision collision)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        surface = other.gameObject.GetComponent<Surface>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (surface == null) return;
        if (surface.surfaceType == Surface.SurfaceType.Wind)
        {
            float wind = 1 / Vector3.Distance(transform.position, other.transform.position) * surface.windForce;
            rb.AddForce(other.transform.up * wind, ForceMode.Force);
        }
    }

    void StickyPullingBack()
    {
/*        if (onStickySurface)
        {
            //rb.AddForce(-stickySideNormal * stickyPullBackForce, ForceMode.Impulse);
            //Debug.Log(Vector3.Dot(rb.velocity, stickySideNormal));
*//*            if (Vector3.Dot(rb.velocity,stickySideNormal) < 0 && jumping)
            {
              
            }*//*
        }*/
    }

    void CheckOutOfStickyBounds()
    {

    }

    private void FixedUpdate()
    {

    }


}