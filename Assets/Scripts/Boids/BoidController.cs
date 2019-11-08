using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class BoidController : MonoBehaviour
{
    public float minVelocity = 5;
    public float maxVelocity = 5;
    public float randomness = 1;
    public float spawnRange = 0.5f;
    public int maxBoids = 500;
    public GameObject prefab;
    public GameObject startFlow;
    public GameObject endFlow;
 
    public Vector3 flockCenter;
    public Vector3 flockVelocity; 
    private List<GameObject> boids = new List<GameObject>();
    private int numCurrentBoids = 0;
 
    void Start()
    {
        StartCoroutine ("BoidSpawning");
    }

    IEnumerator BoidSpawning ()
    {
        while (true){
            if(numCurrentBoids < maxBoids){
                // random spawn position near the startFlow
                Vector3 position = startFlow.transform.position + new Vector3(Random.Range(-spawnRange, spawnRange),
                                                                              Random.Range(-spawnRange, spawnRange),
                                                                              Random.Range(-spawnRange, spawnRange));

                GameObject boid = Instantiate(prefab, transform.position, transform.rotation) as GameObject;
                boid.transform.parent = transform;
                boid.transform.localPosition = position;
                boid.GetComponent<BoidFlocking>().SetController (gameObject);

                boids.Add (boid);
                numCurrentBoids ++;
            }

            // random time between the spawn of two boids
            float waitTime = Random.Range(0.0f, 0.1f);
            yield return new WaitForSeconds (waitTime);
        }        
    }
 
    void Update ()
    {
        Vector3 theCenter = Vector3.zero;
        Vector3 theVelocity = Vector3.zero;
 
        foreach (GameObject boid in boids)
        {
            if(boid != null){
                theCenter = theCenter + boid.transform.localPosition;
                theVelocity = theVelocity + boid.GetComponent<Rigidbody>().velocity;
            }
        }
    }

    public void removeBoid(GameObject boid){
        boids.Remove(boid);
    }

    public void decrementNumBoids()
    {
        numCurrentBoids--;
    }




}