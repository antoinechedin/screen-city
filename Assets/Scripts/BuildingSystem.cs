using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public GameObject prefab;
    public LayerMask worldMask;
    public float buildDistance;

    GameObject currentObj;
    float objSize = 3;

    void Update()
    {
        bool updateSize = false;
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (objSize < 7)
            {
                objSize += 2f;
                updateSize = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (objSize > 1)
            {
                objSize -= 2f;
                updateSize = true;
            }
        }

        if (updateSize)
            UpdateObjSize();

        RaycastHit buildHit;
        if (Physics.Raycast(transform.position, transform.forward, out buildHit, buildDistance, worldMask))
        {
            if (currentObj == null)
                currentObj = Instantiate(prefab);

            SnapObjectToGrid(currentObj, buildHit);
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

    private void UpdateObjSize()
    {
        if (currentObj != null)
                    currentObj.transform.localScale = new Vector3(objSize, objSize, objSize);
                prefab.transform.localScale = new Vector3(objSize, objSize, objSize);
    }

    private void SnapObjectToGrid(GameObject obj, RaycastHit hit)
    {

        // TODO: For now it only snap to closest integer, change it with the grid system
        obj.transform.position = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));
        Vector3 colliderHalfSize = currentObj.transform.localScale / 2f - Vector3.one * 0.1f;
        while (Physics.CheckBox(obj.transform.position, colliderHalfSize, Quaternion.identity, worldMask))
        {
            obj.transform.position += hit.normal;
        }
    }
}
