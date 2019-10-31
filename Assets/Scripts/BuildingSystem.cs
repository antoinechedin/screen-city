using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public GameObject prefab;
    public Material buildMaterial;
    public Material cantBuildMaterial;
    public LayerMask worldMask;
    public float buildDistance;

    GameObject currentObj;
    float objSize;
    bool canBuild;

    public Quaternion buildOrientation;

    private void Start()
    {
        objSize = prefab.transform.localScale.x;
        Cursor.lockState = CursorLockMode.Locked;
        buildOrientation = Quaternion.identity;
    }

    void Update()
    {
        bool updateSize = false;

        if (Input.mouseScrollDelta.y != 0)
        {
            if (Input.mouseScrollDelta.y > 0 && objSize < 7)
                objSize += 2f;
            else if (Input.mouseScrollDelta.y < 0 && objSize > 1)
                objSize -= 2f;
            updateSize = true;
        }

        if (updateSize)
            UpdateObjSize();

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            buildOrientation.eulerAngles += new Vector3(0, 45, 0);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            buildOrientation.eulerAngles += new Vector3(0, -45, 0);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            buildOrientation.eulerAngles += new Vector3(0, 0, 45);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            buildOrientation.eulerAngles += new Vector3(0, 0, -45);
        }

        RaycastHit buildHit;
        if (Physics.Raycast(transform.position, transform.forward, out buildHit, buildDistance, worldMask))
        {
            if (currentObj == null){
                currentObj = Instantiate(prefab);
                currentObj.GetComponent<MeshRenderer>().material = buildMaterial;
            }

            LShape lShape = currentObj.GetComponent<LShape>();
            currentObj.transform.position = buildHit.point;
            currentObj.transform.rotation = buildOrientation;
            // bool box1 = Physics.CheckBox(currentObj.transform.position + new Vector3(lShape.size.x / 2f, 0.05f, lShape.size.z / 2f), new Vector3(lShape.size.x / 2f, 0.05f, lShape.size.z / 2f), buildOrientation, worldMask);
            // bool box2 = Physics.CheckBox(currentObj.transform.position + new Vector3(lShape.size.x / 2f, lShape.size.y / 2f, 0.05f), new Vector3(lShape.size.x / 2f, lShape.size.y / 2f, 0.05f), buildOrientation, worldMask);
            
            bool box = Physics.CheckBox(currentObj.transform.position, lShape.size / 2.0f * objSize, buildOrientation, worldMask);
            
            while (box)
            {
                currentObj.transform.position += 0.01f * (transform.position - buildHit.point).normalized;
                box = Physics.CheckBox(currentObj.transform.position, lShape.size / 2.0f * objSize, buildOrientation, worldMask);
                // box1 = Physics.CheckBox(currentObj.transform.position + new Vector3(lShape.size.x / 2f, 0.05f, lShape.size.z / 2f), new Vector3(lShape.size.x / 2f, 0.05f, lShape.size.z / 2f), buildOrientation, worldMask);
                // box2 = Physics.CheckBox(currentObj.transform.position + new Vector3(lShape.size.x / 2f, lShape.size.y / 2f, 0.05f), new Vector3(lShape.size.x / 2f, lShape.size.y / 2f, 0.05f), buildOrientation, worldMask);
            }

            canBuild = true;
        }
        else
        {
            if (currentObj != null)
                Destroy(currentObj);
        }

        if (currentObj != null)
        {
            if (Input.GetMouseButtonDown(0) && canBuild)
            {
                currentObj.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
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
        //TODO: For now it only snap to closest integer, change it with the grid system
        Vector3 colliderHalfSize = currentObj.transform.localScale / 2f;
        Vector3 newPosition = hit.point + Vector3.Scale(colliderHalfSize, hit.normal);
        Vector3 buildPos = MathUtil.RoundVec3(newPosition, 1);

        obj.transform.position = buildPos;
        if (Physics.CheckBox(obj.transform.position, colliderHalfSize - Vector3.one * 0.1f, Quaternion.identity, worldMask))
        {
            canBuild = false;
            obj.GetComponent<MeshRenderer>().material = cantBuildMaterial;
        }
        else
        {
            canBuild = true;
            obj.GetComponent<MeshRenderer>().material = buildMaterial;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (currentObj != null)
        {
            LShape lShape = currentObj.GetComponent<LShape>();
            Gizmos.DrawWireCube(currentObj.transform.position + new Vector3(lShape.size.x / 2f, 0.05f, lShape.size.z / 2f), new Vector3(lShape.size.x / 2f, 0.05f, lShape.size.z / 2f));
    }
    }

}
