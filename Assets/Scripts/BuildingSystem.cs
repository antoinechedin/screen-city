using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public GameObject prefab;
    public LayerMask buildMask;
    public float buildDistance;

    GameObject currentObj;

    void Update()
    {
        RaycastHit buildHit;
        if (Physics.Raycast(transform.position, transform.forward, out buildHit, buildDistance, buildMask))
        {
            if (currentObj == null)
                currentObj = Instantiate(prefab);

            currentObj.transform.position = snapToGrid(buildHit.point + buildHit.normal * 0.1f);
        }
        else
        {
            if (currentObj != null)
                Destroy(currentObj);
        }

        if (currentObj != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentObj.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
                currentObj.layer = LayerMask.NameToLayer("World");
                currentObj = null;
            }

            if (Input.GetMouseButtonDown(1) && buildHit.transform != null)
            {
                Destroy(buildHit.transform.gameObject);
            }
        }
    }

    Vector3 snapToGrid(Vector3 vec)
    {
        // TODO: For now it only snap to closest integer, change it with the grid system
        return new Vector3(Mathf.Round(vec.x), Mathf.Round(vec.y), Mathf.Round(vec.z));
    }
}
