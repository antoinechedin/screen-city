using UnityEngine;

[CreateAssetMenu(fileName = "Boid Physics", menuName = "Boid Physics")]
public class BoidPhysics : ScriptableObject
{
    public Vector3 min;
    public Vector3 max;

    public float minSpeed;
    public float maxSpeed;
    public float maxForce;

    public float neighborDistance;
    public float separationDistance;

    [Range(0, 4)]
    public float alignementCoef;
    [Range(0, 4)]
    public float cohesionCoef;
    [Range(0, 4)]
    public float separationCoef;
    [Range(0, 4)]
    public float seekCoef;
}
