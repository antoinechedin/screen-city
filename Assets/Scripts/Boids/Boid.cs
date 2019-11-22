using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Boid : MonoBehaviour
{
    public BoidPhysics physics;
    public Vector3 velocity;

    public Transform target;
    //public Box box;

    private void Start()
    {
        velocity = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized * (physics.maxSpeed + physics.minSpeed) / 2f;
    }

    private void Update()
    {
        Vector3 acceleration = Vector3.zero;

        int layerMask = 1 << LayerMask.NameToLayer("Boid");
        Collider[] neighbors = Physics.OverlapSphere(transform.position, physics.neighborDistance, layerMask);


        acceleration += physics.alignementCoef * Alignement(neighbors);
        acceleration += physics.cohesionCoef * Cohesion(neighbors);
        acceleration += physics.separationCoef * Separation(neighbors);
        acceleration += physics.seekCoef * Seek();

        // Avoid bounds
        if (transform.position.x > physics.min.x)
            acceleration += Steer(transform.position + Vector3.left);
        if (transform.position.x < physics.max.x)
            acceleration += Steer(transform.position + Vector3.right);
        if (transform.position.y > physics.min.y)
            acceleration += Steer(transform.position + Vector3.down);
        if (transform.position.y < physics.max.y)
            acceleration += Steer(transform.position + Vector3.up);
        if (transform.position.z > physics.min.z)
            acceleration += Steer(transform.position + Vector3.back);
        if (transform.position.z < physics.max.z)
            acceleration += Steer(transform.position + Vector3.forward);

        velocity += acceleration * Time.deltaTime ;
        float speed = Mathf.Clamp(velocity.magnitude, physics.minSpeed, physics.maxSpeed);
        transform.position += velocity.normalized * speed ;

        transform.LookAt(transform.position + velocity);

        // Teleport boid
        /*if (transform.position.x > box.size / 2f)
            transform.position += new Vector3(-box.size, 0, 0);
        if (transform.position.x < -box.size / 2f)
            transform.position += new Vector3(box.size, 0, 0);
        if (transform.position.y > box.size / 2f)
            transform.position += new Vector3(0, -box.size, 0);
        if (transform.position.y < -box.size / 2f)
            transform.position += new Vector3(0, box.size, 0);
        if (transform.position.z > box.size / 2f)
            transform.position += new Vector3(0, 0, -box.size);
        if (transform.position.z < -box.size / 2f)
            transform.position += new Vector3(0, 0, box.size);*/
    }

    public Vector3 Alignement(Collider[] neighbors)
    {
        Vector3 avgVelocity = Vector3.zero;
        int numNeighbor = 0;
        foreach (Collider collider in neighbors)
        {
            Boid boid = collider.GetComponent<Boid>();
            if (boid != this)
            {
                avgVelocity += boid.velocity;
                numNeighbor++;
            }
        }
        if (numNeighbor > 0)
        {
            avgVelocity /= numNeighbor;
            return Steer(transform.position + avgVelocity);
        }
        return Vector3.zero;
    }

    public Vector3 Cohesion(Collider[] neighbors)
    {
        Vector3 centerOfNeighbors = Vector3.zero;
        int numNeighbor = 0;
        foreach (Collider collider in neighbors)
        {
            Boid boid = collider.GetComponent<Boid>();
            if (boid != this)
            {
                centerOfNeighbors += boid.transform.position;
                numNeighbor++;
            }
        }
        if (numNeighbor > 0)
        {
            centerOfNeighbors /= numNeighbor;
            return Steer(centerOfNeighbors);
        }
        return Vector3.zero;
    }

    public Vector3 Separation(Collider[] neighbors)
    {
        Vector3 centerOfNeighbors = Vector3.zero;
        int numNeighbor = 0;
        foreach (Collider collider in neighbors)
        {
            Boid boid = collider.GetComponent<Boid>();
            if (boid != this)
            {
                float t = (boid.transform.position - transform.position).magnitude;
                if (t < physics.separationDistance)
                {
                    centerOfNeighbors += boid.transform.position;
                    numNeighbor++;
                }
            }
        }
        if (numNeighbor > 0)
        {
            centerOfNeighbors /= numNeighbor;
            return Steer(transform.position - (centerOfNeighbors - transform.position));
        }
        return Vector3.zero;
    }

    private Vector3 Seek()
    {
        // GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        // if (targets.Length > 0)
        // {
        //     GameObject closer = targets[0];
        //     for (int i = 1; i < targets.Length; i++)
        //     {
        //         if (Vector3.Distance(transform.position, closer.transform.position) > Vector3.Distance(transform.position, targets[i].transform.position))
        //         {
        //             closer = targets[i];
        //         }
        //     }
        //     return Steer(closer.transform.position);
        // }
        return Vector3.zero;
    }

    public Vector3 Steer(Vector3 target)
    {
        Vector3 desired = (target - transform.position);
        desired = desired.normalized * physics.maxSpeed - velocity;
        return Vector3.ClampMagnitude(desired, physics.maxForce);
    }
}
