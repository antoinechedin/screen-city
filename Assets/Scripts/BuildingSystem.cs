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
    private Vector3 objSize;
    bool canBuild;
    private bool sizingX = true, sizingY = true, sizingZ = true;

    public Quaternion buildOrientation;

    private void Start()
    {
        prefab.GetComponent<LShape>().size = new Vector3(1f, 1f, 1f);
        objSize = prefab.GetComponent<LShape>().size;
        Cursor.lockState = CursorLockMode.Locked;
        buildOrientation = Quaternion.identity;
    }

    void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                if (sizingX) objSize += new Vector3(1f, 0f, 0f);
                if (sizingY) objSize += new Vector3(0f, 1f, 0f);
                if (sizingZ) objSize += new Vector3(0f, 0f, 1f);
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                if (sizingX && objSize.x > 1f) objSize -= new Vector3(1f, 0f, 0f);
                if (sizingY && objSize.y > 1f) objSize -= new Vector3(0f, 1f, 0f);
                if (sizingZ && objSize.z > 1f) objSize -= new Vector3(0f, 0f, 1f);
            }
            UpdateObjSize();
        }

        if (Input.GetKeyDown(KeyCode.Keypad1)) sizingX = !sizingX;
        if (Input.GetKeyDown(KeyCode.Keypad2)) sizingY = !sizingY;
        if (Input.GetKeyDown(KeyCode.Keypad3)) sizingZ = !sizingZ;

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
            if (currentObj == null)
            {
                currentObj = Instantiate(prefab);
                currentObj.GetComponent<MeshRenderer>().material = buildMaterial;
            }

            LShape lShape = currentObj.GetComponent<LShape>();
            currentObj.transform.position = buildHit.point;
            currentObj.transform.rotation = buildOrientation;
            // bool box1 = Physics.CheckBox(currentObj.transform.position + new Vector3(lShape.size.x / 2f, 0.05f, lShape.size.z / 2f), new Vector3(lShape.size.x / 2f, 0.05f, lShape.size.z / 2f), buildOrientation, worldMask);
            // bool box2 = Physics.CheckBox(currentObj.transform.position + new Vector3(lShape.size.x / 2f, lShape.size.y / 2f, 0.05f), new Vector3(lShape.size.x / 2f, lShape.size.y / 2f, 0.05f), buildOrientation, worldMask);

            bool box = Physics.CheckBox(currentObj.transform.position, lShape.size / 2.0f, buildOrientation, worldMask);

            while (box)
            {
                currentObj.transform.position += 0.01f * (transform.position - buildHit.point).normalized;
                box = Physics.CheckBox(currentObj.transform.position, lShape.size / 2.0f, buildOrientation, worldMask);
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
        {
            LShape lShape = currentObj.GetComponent<LShape>();
            lShape.size = objSize;
            lShape.origin = objSize / 2f;
            lShape.UpdateMesh();
        }
        prefab.GetComponent<LShape>().size = objSize;
        prefab.GetComponent<LShape>().origin = objSize / 2f;
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
            Gizmos.DrawWireCube(currentObj.transform.position, lShape.size);
        }
    }

}
