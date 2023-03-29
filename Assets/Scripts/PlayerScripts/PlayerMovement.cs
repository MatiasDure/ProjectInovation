using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float jumpForce;
    [SerializeField] float stickyPullBackForce; 
    [SerializeField] float stickyDragForce;

    bool onStickySurface;

    bool jumping;

    Vector3 stickySideNormal; 





    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 direction) 
    {
        jumping = true;
        rb.AddForce(direction * jumpForce, ForceMode.Impulse);
        if (onStickySurface) 
        { 
            if(Vector3.Dot(direction, stickySideNormal) > 0.2f) rb.drag = 0;
            else jumping = false;
        }      
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
        {
            jumping = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if(Vector3.Dot(Vector3.up, stickySideNormal) > 0.2f)rb.drag = 0;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            rb.AddForce(Vector3.left * jumpForce, ForceMode.Impulse);
            if (Vector3.Dot(Vector3.left, stickySideNormal) > 0.2f) rb.drag = 0;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            rb.AddForce(Vector3.down * jumpForce, ForceMode.Impulse);
            if (Vector3.Dot(Vector3.down, stickySideNormal) > 0.2f) rb.drag = 0;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            rb.AddForce(Vector3.right * jumpForce, ForceMode.Impulse);
            if (Vector3.Dot(Vector3.right, stickySideNormal) > 0.2f) rb.drag = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        jumping = false;
        if (collision.gameObject.tag == "Sticky")
        {
            onStickySurface = true;
            rb.drag = stickyDragForce;
            ContactPoint contact = collision.contacts[0];
            stickySideNormal = contact.normal;
        }
        else {
            rb.drag = 0;
            onStickySurface = false;
        } 

    }
    private void OnCollisionExit(Collision collision)
    {
        
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