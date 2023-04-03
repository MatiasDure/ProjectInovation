using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyToShader : MonoBehaviour
{
    [SerializeField] Renderer _renderer;
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] Transform particle; 

    Vector3 oldVelocity = Vector3.zero;

    float amplitude = 0;

    float rise = 0;

    float singleWaveAmplitude; 
    float singleWaveWidth;

    CirclePhysics2D physicsCircle2D;

    int previousArraySize = 5;


    void Start()
    {
        physicsCircle2D = new CirclePhysics2D(transform.localScale.x / 2);
        physicsCircle2D.AddCircle(new ParticleCircle(Vector2.zero, Vector2.zero, 0.02f));
        physicsCircle2D.AddCircle(new ParticleCircle(Vector2.zero, Vector2.zero, 0.08f));
        physicsCircle2D.AddCircle(new ParticleCircle(Vector2.zero, Vector2.zero, 0.04f));
        physicsCircle2D.AddCircle(new ParticleCircle(Vector2.zero, Vector2.zero, 0.07f));
        physicsCircle2D.AddCircle(new ParticleCircle(Vector2.zero, Vector2.zero, 0.05f));

    }

    void Update()
    {


    }
    void ParticleTrail(float length, int resolution)
    {

    }

    private void FixedUpdate()
    {
        Vector3 diff = _rigidbody.velocity - oldVelocity;


        amplitude += diff.magnitude;
        amplitude *= 0.93f;

        rise += -_rigidbody.velocity.x/200;
        rise *= 0.92f;
        float t = Mathf.PingPong(Time.time*2, 1);
        float waveOffset = rise;
        float singleWave = Mathf.Lerp(-0.5f, 0.5f, t);
        float wobble = Mathf.Lerp(-waveOffset, waveOffset, t);
        singleWaveAmplitude += -_rigidbody.velocity.magnitude / 15;
        singleWaveAmplitude *= 0.90f;
        if (singleWaveAmplitude < 1.1f) singleWaveAmplitude = 1.1f;

         singleWaveWidth += -_rigidbody.velocity.magnitude / 20;
        singleWaveWidth *= 0.92f;

        float xyDiff = Mathf.Abs(_rigidbody.velocity.normalized.x);


        Vector3 velocityGravityDelta = _rigidbody.velocity - (oldVelocity - Physics.gravity*Time.fixedDeltaTime);

        velocityGravityDelta *= 0.2f;

        foreach (var ball in physicsCircle2D.GetCircles())
        {
            ball.velocity += new Vector2(velocityGravityDelta.x, velocityGravityDelta.y); 
        }


        int currentArraySize = physicsCircle2D.particlePositions.Length;
        if (currentArraySize > previousArraySize)
        {
            System.Array.Resize(ref physicsCircle2D.particlePositions, previousArraySize);
            Debug.Log("Array resized : " + previousArraySize);
        }



        _renderer.material.SetFloat("rise", wobble);
        _renderer.material.SetFloat("_AngularVelocity", _rigidbody.angularVelocity.magnitude / 100);
        _renderer.material.SetFloat("_VelocityDelta", amplitude / 700);
        _renderer.material.SetFloat("singleWaveAmplitude", singleWaveAmplitude);
        _renderer.material.SetFloat("singleWaveOffset", singleWave* xyDiff);
        _renderer.material.SetFloat("singleWaveWidth", singleWaveWidth);


        physicsCircle2D.UpdateSimulation(Time.fixedDeltaTime);
        _renderer.material.SetVectorArray("_ParticlePositions", physicsCircle2D.particlePositions);
        _renderer.material.SetInt("_ParticleCount", previousArraySize);

        oldVelocity = _rigidbody.velocity;
    }
}


public class ParticleCircle
{
    public Vector2 position;
    public Vector2 velocity;
    public float radius;

    public ParticleCircle(Vector2 position, Vector2 velocity, float radius)
    {
        this.position = position;
        this.velocity = velocity;
        this.radius = radius;
    }
}
public class CirclePhysics2D
{


    public float domainRadius;
    public List<ParticleCircle> circles;
    public float gravity = -0.1f;
    public float simulationDrag = 0.97f; 
    public float collisionDamping = 0.8f;
    public Vector4[] particlePositions = new Vector4[5];

    public CirclePhysics2D(float domainRadius)
    {
        this.domainRadius = domainRadius;
        circles = new List<ParticleCircle>();
    }

    public ParticleCircle AddCircle(ParticleCircle circle)
    {
        circles.Add(circle);
        return circle;
    }
    public List<ParticleCircle> GetCircles()
    {
        return circles;
    }

    public void UpdateSimulation(float deltaTime)
    {

        for (int i = 0; i < circles.Count; i++)
        {
            //Add gravity
            circles[i].velocity += new Vector2(0, gravity);

            //Add drag
            circles[i].velocity *= simulationDrag;

            // Update circle position based on its velocity
            circles[i].position += circles[i].velocity * deltaTime;

            // Check if the circle collides with the domain boundary
            float distanceFromCenter = circles[i].position.magnitude;
            if (distanceFromCenter + circles[i].radius > domainRadius)
            {
                // Reflect the circle's velocity based on the collision
                Vector2 normal = circles[i].position.normalized;
                float randmomVariation = 0.1f;
                Vector2 random = new Vector2(Random.Range(-randmomVariation, randmomVariation), Random.Range(-randmomVariation, randmomVariation));
                circles[i].velocity = Vector2.Reflect(circles[i].velocity * collisionDamping, normal+ random);

                // Make sure the circle stays inside the domain
                circles[i].position = normal * (domainRadius - circles[i].radius);
            }

            particlePositions[i] = new Vector4(circles[i].position.x, circles[i].position.y,0, circles[i].radius);
        }

    }
}
