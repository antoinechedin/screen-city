using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public GameObject boidPrefab;
    public BoidPhysics physics;

    [Range(0, 100)]
    public int numBoid;

    private void Start()
    {
        for (int i = 0; i < numBoid; i++)
        {
            float rx = Random.Range(physics.min.x, physics.max.x);
            float ry = Random.Range(physics.min.y, physics.max.y);
            float rz = Random.Range(physics.min.z, physics.max.z);
            Vector3 pos = new Vector3(rx, ry, rz);
            Instantiate(boidPrefab, pos, Quaternion.identity, transform);
        }
    }
}
