using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

using SFB;

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
    private List<Tuple<GameObject, bool>> undoHist = new List<Tuple<GameObject, bool>>();
    private List<Tuple<GameObject, bool>> redoHist = new List<Tuple<GameObject, bool>>();
    private Texture2D myTexture;

    public Quaternion buildOrientation;

    private void Start()
    {
        objSize = prefab.GetComponent<LShape>().size;
        Cursor.lockState = CursorLockMode.Locked;
        buildOrientation = Quaternion.identity;
    }

    void Update()
    {
        // Undo
        if (Input.GetKeyDown(KeyCode.RightControl) && undoHist.Count > 0)
        {
            redoHist.Add(new Tuple<GameObject, bool>(undoHist[undoHist.Count - 1].Item1, !undoHist[undoHist.Count - 1].Item2));
            undoHist[undoHist.Count - 1].Item1.SetActive(!undoHist[undoHist.Count - 1].Item2);
            undoHist.Remove(undoHist[undoHist.Count - 1]);
        }

        // Redo
        if (Input.GetKeyDown(KeyCode.RightShift) && redoHist.Count > 0)
        {
            undoHist.Add(new Tuple<GameObject, bool>(redoHist[redoHist.Count - 1].Item1, !redoHist[redoHist.Count - 1].Item2));
            redoHist[redoHist.Count - 1].Item1.SetActive(!redoHist[redoHist.Count - 1].Item2);
            redoHist.Remove(redoHist[redoHist.Count - 1]);
        }

        if (Input.GetKeyDown(KeyCode.M)) loadImage();

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
                undoHist.Add(new Tuple<GameObject, bool>(currentObj, true));
                foreach (Tuple<GameObject, bool> t in redoHist) if (!t.Item2) Destroy(t.Item1);
                redoHist.Clear();
                currentObj = null;
            }

            if (Input.GetMouseButtonDown(1) && buildHit.transform != null)
            {
                undoHist.Add(new Tuple<GameObject, bool>(buildHit.transform.gameObject, false));
                buildHit.transform.gameObject.SetActive(false);
                foreach (Tuple<GameObject, bool> t in redoHist) if (!t.Item2) Destroy(t.Item1);
                redoHist.Clear();
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

	private void loadImage()
	{
		var extensions = new[] {
			new ExtensionFilter("Images ", "png", "jpg", "jpeg"),
			new ExtensionFilter("Tous les fichiers ", "*"),
		};
		var path = StandaloneFileBrowser.OpenFilePanel("Choisir une image", "", extensions, true);
		if (path.Length <= 0) return;
		byte[] imgData = File.ReadAllBytes(path[0]);
		myTexture = new Texture2D(1, 1);
		myTexture.LoadImage(imgData);
		GameObject.Find("Plane").GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
		GameObject.Find("Plane").GetComponent<MeshRenderer>().material.mainTexture = myTexture;
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
