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

    private void Start()
    {
        objSize = prefab.transform.localScale.x;
        Cursor.lockState = CursorLockMode.Locked;
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

        RaycastHit buildHit;
        if (Physics.Raycast(transform.position, transform.forward, out buildHit, buildDistance, worldMask))
        {
            if (currentObj == null)
                currentObj = Instantiate(prefab);

            LShape lShape = currentObj.GetComponent<LShape>();
            currentObj.transform.position = buildHit.point;
            bool box1 = true;
            bool box2 = true;
            box1 = Physics.CheckBox(currentObj.transform.position + new Vector3(lShape.size.x / 2f, 0.05f, lShape.size.z / 2f), new Vector3(lShape.size.x / 2f, 0.05f, lShape.size.z / 2f), Quaternion.identity, worldMask);
            box2 = Physics.CheckBox(currentObj.transform.position + new Vector3(lShape.size.x / 2f, lShape.size.y / 2f, 0.05f), new Vector3(lShape.size.x / 2f, lShape.size.y / 2f, 0.05f), Quaternion.identity, worldMask);
            while (box1 || box2)
            {
                currentObj.transform.position += 0.01f * (transform.position - buildHit.point).normalized;
                box1 = Physics.CheckBox(currentObj.transform.position + new Vector3(lShape.size.x / 2f, 0.05f, lShape.size.z / 2f), new Vector3(lShape.size.x / 2f, 0.05f, lShape.size.z / 2f), Quaternion.identity, worldMask);
                box2 = Physics.CheckBox(currentObj.transform.position + new Vector3(lShape.size.x / 2f, lShape.size.y / 2f, 0.05f), new Vector3(lShape.size.x / 2f, lShape.size.y / 2f, 0.05f), Quaternion.identity, worldMask);
            }

            // currentObj.transform.position = buildHit.point;
            // Vector3 colliderHalfSize = currentObj.transform.localScale / 2f;
            // while (currentObj.GetComponent<LShape>().numCollision > 0){
            //     currentObj.transform.position += 0.01f * (transform.position - buildHit.point).normalized;
            // }
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

}
