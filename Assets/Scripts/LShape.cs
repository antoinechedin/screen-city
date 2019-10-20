using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LShape : MonoBehaviour
{
    private MeshFilter mf;
    private MeshRenderer mr;

    public Vector3 origin;
    public Vector3 size;

    private void Awake()
    {
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Diffuse"));

        mf.mesh = LShape.BuildMesh(origin, size);
    }

    private static Mesh BuildMesh(Vector3 origin, Vector3 size)
    {
        float THICKNESS = 0.1f;

        Vector3[] vertices = new Vector3[36];
        int[] triangles = new int[60];

        vertices[0] = -origin;
        vertices[1] = -origin + new Vector3(0f, size.y, 0f);
        vertices[2] = -origin + new Vector3(size.x, size.y, 0f);
        vertices[3] = -origin + new Vector3(size.x, 0f, 0f);

        vertices[4] = -origin + new Vector3(0f, size.y, 0f);
        vertices[5] = -origin + new Vector3(0f, size.y, THICKNESS);
        vertices[6] = -origin + new Vector3(size.x, size.y, THICKNESS);
        vertices[7] = -origin + new Vector3(size.x, size.y, 0f);

        vertices[8] = -origin + new Vector3(size.x, THICKNESS, THICKNESS);
        vertices[9] = -origin + new Vector3(size.x, size.y, THICKNESS);
        vertices[10] = -origin + new Vector3(0f, size.y, THICKNESS);
        vertices[11] = -origin + new Vector3(0f, THICKNESS, THICKNESS);

        vertices[12] = -origin + new Vector3(0f, THICKNESS, THICKNESS);
        vertices[13] = -origin + new Vector3(0f, THICKNESS, size.z);
        vertices[14] = -origin + new Vector3(size.x, THICKNESS, size.z);
        vertices[15] = -origin + new Vector3(size.x, THICKNESS, THICKNESS);

        vertices[16] = -origin + new Vector3(size.x, 0f, size.z);
        vertices[17] = -origin + new Vector3(size.x, THICKNESS, size.z);
        vertices[18] = -origin + new Vector3(0f, THICKNESS, size.z);
        vertices[19] = -origin + new Vector3(0f, 0f, size.z);

        vertices[20] = -origin + new Vector3(size.x, 0f, 0f);
        vertices[21] = -origin + new Vector3(size.x, 0f, size.z);
        vertices[22] = -origin + new Vector3(0f, 0f, size.z);
        vertices[23] = -origin + new Vector3(0f, 0f, 0f);

        vertices[24] = -origin + new Vector3(0f, 0f, 0f);
        vertices[25] = -origin + new Vector3(0f, 0f, size.z);
        vertices[26] = -origin + new Vector3(0f, THICKNESS, size.z);
        vertices[27] = -origin + new Vector3(0f, THICKNESS, THICKNESS);
        vertices[28] = -origin + new Vector3(0f, size.y, THICKNESS);
        vertices[29] = -origin + new Vector3(0f, size.y, 0f);

        vertices[30] = -origin + new Vector3(size.x, 0f, 0f);
        vertices[31] = -origin + new Vector3(size.x, size.y, 0f);
        vertices[32] = -origin + new Vector3(size.x, size.y, THICKNESS);
        vertices[33] = -origin + new Vector3(size.x, THICKNESS, THICKNESS);
        vertices[34] = -origin + new Vector3(size.x, THICKNESS, size.z);
        vertices[35] = -origin + new Vector3(size.x, 0f, size.z);

        int triangleIndex = 0;
        for (int faceIndex = 0; faceIndex < 6; faceIndex++)
        {
            triangles[triangleIndex] = faceIndex * 4;
            triangles[triangleIndex + 1] = faceIndex * 4 + 1;
            triangles[triangleIndex + 2] = faceIndex * 4 + 2;
            triangles[triangleIndex + 3] = faceIndex * 4;
            triangles[triangleIndex + 4] = faceIndex * 4 + 2;
            triangles[triangleIndex + 5] = faceIndex * 4 + 3;
            triangleIndex += 6;
        }

        for (int faceIndex = 0; faceIndex < 2; faceIndex++)
        {
            triangles[triangleIndex] = 24 + faceIndex * 6;
            triangles[triangleIndex + 1] = 24 + faceIndex * 6 + 1;
            triangles[triangleIndex + 2] = 24 + faceIndex * 6 + 3;
            triangles[triangleIndex + 3] = 24 + faceIndex * 6 + 1;
            triangles[triangleIndex + 4] = 24 + faceIndex * 6 + 2;
            triangles[triangleIndex + 5] = 24 + faceIndex * 6 + 3;
            triangles[triangleIndex + 6] = 24 + faceIndex * 6;
            triangles[triangleIndex + 7] = 24 + faceIndex * 6 + 3;
            triangles[triangleIndex + 8] = 24 + faceIndex * 6 + 5;
            triangles[triangleIndex + 9] = 24 + faceIndex * 6 + 3;
            triangles[triangleIndex + 10] = 24 + faceIndex * 6 + 4;
            triangles[triangleIndex + 11] = 24 + faceIndex * 6 + 5;
            triangleIndex += 12;
        }

        Mesh mesh = new Mesh();
        mesh.name = "l-shape";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}
