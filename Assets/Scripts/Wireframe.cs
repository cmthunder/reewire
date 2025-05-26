using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Threading.Tasks;

// 🖕 para los que usan trigonometría (lookin at u appsl)
public class Wireframe : MonoBehaviour
{
    [Header("Generator settings")]
    public float wireframeThickness = 0.025f;
    public float horizontalResolution = 1f;
    public float verticalResolution = 2f;
    public int cullMode = 0;
    bool depthWrite = false;
    public int blurFrames = 1;
    public float glowMultiplier = 1;

    [Header("Output settings")]
    public string fileName;
    public bool beautifyJson;

    public struct Edge
    {
        public int a;
        public int b;
    };

    // Private globals
    private GameObject cylinder;

    public void Awake()
    {
        cylinder = Resources.Load("Preview Cylinder", typeof(GameObject)) as GameObject;

        float begin = Time.realtimeSinceStartup;
        string json = JsonUtility.ToJson(GetJsonWireframeObject(), beautifyJson);
        Debug.Log("Generation done! (" + (Time.realtimeSinceStartup - begin).ToString() + "s)");

        if (File.Exists(fileName))
        {
            Debug.LogError("File already exists! No file was generated.");
        }
        else
        {
            StreamWriter sr = File.CreateText(fileName);
            sr.WriteLine(json);
            sr.Close();
            Debug.Log("File written!");
        }

        foreach (Transform child in transform) child.gameObject.SetActive(false); // Hide real gameObjects, show only wireframe preview

        // ¿Donde se guarda? idfk
        // Edit: Ya se donde, en la carpeta del proyecto de unity (no en Assets, en el nivel superior)
    }

    [BurstCompile]
    private Root GetJsonWireframeObject()
    {
        Root json = new()
        {
            Modules = new()
        };

        foreach (Transform child in transform)
        {
            GameObject obj = child.gameObject;
            if (obj.GetComponent<MeshFilter>() == null) continue;

            Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            List<Edge> edges = new(); // Every two elements represent an edge (vertex indices)
            for (int i = 0; i < triangles.Length; i += 3)
            {
                Edge a, b, c;

                a.a = triangles[i];
                a.b = triangles[i + 1];
                b.a = triangles[i + 1];
                b.b = triangles[i + 2];
                c.a = triangles[i + 2];
                c.b = triangles[i];

                edges.Add(a);
                edges.Add(b);
                edges.Add(c);
            }

            // Remove dupe edges
            for (int i = 0; i < edges.Count - 1; i++)
            {
                for (int j = i + 1; j < edges.Count; j++)
                {
                    float distAA = Vector3.Distance(vertices[edges[i].a], vertices[edges[j].a]);
                    float distAB = Vector3.Distance(vertices[edges[i].a], vertices[edges[j].b]);
                    float distBA = Vector3.Distance(vertices[edges[i].b], vertices[edges[j].a]);
                    float distBB = Vector3.Distance(vertices[edges[i].b], vertices[edges[j].b]);
                    if ((distAA < 0.001f && distBB < 0.001f) || (distAB < 0.001f && distBA < 0.001f)) edges.RemoveAt(j--);
                }
            }

            for (int i = 0; i < edges.Count; i++)
            {
                GameObject newCylinder = Instantiate(cylinder, vertices[edges[i].a], Quaternion.identity);
                newCylinder.transform.LookAt(vertices[edges[i].b]);
                float distance = Vector3.Distance(vertices[edges[i].a], vertices[edges[i].b]);
                newCylinder.transform.localScale = new Vector3(wireframeThickness, wireframeThickness, distance);

                newCylinder.transform.RotateAround(Vector3.zero, Vector3.up, 90f); // Fix to make object upright inside BS

                Module module = new()
                {
                    Config = new()
                    {
                        SaberSettings = new()
                        {
                            zOffsetTo = distance,
                            thickness = wireframeThickness,
                            horizontalResolution = horizontalResolution,
                            verticalResolution = verticalResolution,
                            cullMode = cullMode,
                            depthWrite = depthWrite,
                            blurFrames = blurFrames,
                            glowMultiplier = glowMultiplier
                        },
                        LocalTransform = new()
                        {
                            Position = new()
                            {
                                x = vertices[edges[i].a].x,
                                y = vertices[edges[i].a].y,
                                z = vertices[edges[i].a].z
                            },
                            Rotation = new()
                            {
                                x = newCylinder.transform.eulerAngles.x,
                                y = newCylinder.transform.eulerAngles.y,
                                z = newCylinder.transform.eulerAngles.z
                            }
                        }
                    }
                };

                json.Modules.Add(module);

                newCylinder.transform.RotateAround(Vector3.zero, Vector3.up, -90f); // Restore rotation for preview
            }
        }

        return json;
    }
}
