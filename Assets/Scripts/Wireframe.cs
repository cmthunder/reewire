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
        Debug.Log(json);
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
            ModVersion = "0.3.12",
            Version = 1,
            LocalTransform = new()
            {
                Position = new()
                {
                    x = 0f,
                    y = 0f,
                    z = 0f
                },
                Rotation = new()
                {
                    x = 0f,
                    y = 0f,
                    z = 0f
                },
                Scale = new()
                {
                    x = 1f,
                    y = 1f,
                    z = 1f
                }
            },
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
                newCylinder.transform.localScale = new Vector3(wireframeThickness / 10f, wireframeThickness / 10f, distance);

                //newCylinder.transform.RotateAround(Vector3.zero, Vector3.up, 90f); // Fix to make object upright inside BS

                Module module = new()
                {
                    ModuleId = "reezonate.blur-saber",
                    Version = 1,
                    Config = new()
                    {
                        SaberSettings = new()
                        {
                            zOffsetFrom = 0f,
                            zOffsetTo = distance,
                            thickness = wireframeThickness,
                            saberProfile = new()
                            {
                                interpolationType = 1,
                                controlPoints = new()
                                {
                                    new ControlPoint()
                                    {
                                        time = 0f,
                                        value = 1f
                                    }
                                }
                            },
                            startCap = true,
                            endCap = true,
                            horizontalResolution = horizontalResolution,
                            verticalResolution = verticalResolution,
                            renderQueue = 3002,
                            cullMode = cullMode,
                            depthWrite = depthWrite,
                            blurFrames = blurFrames,
                            glowMultiplier = glowMultiplier,
                            handleRoughness = 2f,
                            handleColor = new()
                            {
                                r = 0.1f,
                                g = 0.1f,
                                b = 0.1f,
                                a = 0f
                            },
                            maskSettings = new()
                            {
                                bladeMaskResolution = 256,
                                driversMaskResolution = 32,
                                handleMask = new()
                                {
                                    interpolationType = 2,
                                    controlPoints = new()
                                    {
                                        new ControlPoint()
                                        {
                                            time = 0f,
                                            value = 0f
                                        }
                                    }
                                },
                                bladeMappings = new()
                                {
                                    colorOverValue = new()
                                    {
                                        interpolationType = 0,
                                        controlPoints = new()
                                        {
                                            new ColorControlPoint()
                                            {
                                                time = 0.0f,
                                                value = new()
                                                {
                                                    r = 1f,
                                                    g = 1f,
                                                    b = 1f,
                                                    a = 1f
                                                }
                                            }
                                        }
                                    },
                                    alphaOverValue = new()
                                    {
                                        interpolationType = 0,
                                        controlPoints = new()
                                        {
                                            new ControlPoint()
                                            {
                                                time = 0f,
                                                value = 1f
                                            }
                                        }
                                    },
                                    scaleOverValue = new()
                                    {
                                        interpolationType = 0,
                                        controlPoints = new()
                                        {
                                            new ControlPoint()
                                            {
                                                time = 0f,
                                                value = 1f
                                            }
                                        }
                                    },
                                    valueFrom = 0f,
                                    valueTo = 1f,
                                },
                                driversSampleMode = 0,
                                viewingAngleMappings = new()
                                {
                                    colorOverValue = new()
                                    {
                                        interpolationType = 0,
                                        controlPoints = new()
                                        {
                                            new ColorControlPoint()
                                            {
                                                time = 0,
                                                value = new()
                                                {
                                                    r = 1f,
                                                    g = 1f,
                                                    b = 1f,
                                                    a = 1f
                                                }
                                            }
                                        }
                                    },
                                    alphaOverValue = new()
                                    {
                                        interpolationType = 0,
                                        controlPoints = new()
                                        {
                                            new ControlPoint()
                                            {
                                                time = 0f,
                                                value = 1f
                                            }
                                        }
                                    },
                                    scaleOverValue = new()
                                    {
                                        interpolationType = 0,
                                        controlPoints = new()
                                        {
                                            new ControlPoint()
                                            {
                                                time = 0f,
                                                value = 1f
                                            }
                                        }
                                    },
                                    valueFrom = 0f,
                                    valueTo = 1f,
                                },
                                surfaceAngleMappings = new()
                                {
                                    colorOverValue = new()
                                    {
                                        interpolationType = 0,
                                        controlPoints = new()
                                        {
                                            new ColorControlPoint()
                                            {
                                                time = 0,
                                                value = new()
                                                {
                                                    r = 1f,
                                                    g = 1f,
                                                    b = 1f,
                                                    a = 1f
                                                }
                                            }
                                        }
                                    },
                                    alphaOverValue = new()
                                    {
                                        interpolationType = 0,
                                        controlPoints = new()
                                        {
                                            new ControlPoint()
                                            {
                                                time = 0f,
                                                value = 1f
                                            }
                                        }
                                    },
                                    scaleOverValue = new()
                                    {
                                        interpolationType = 0,
                                        controlPoints = new()
                                        {
                                            new ControlPoint()
                                            {
                                                time = 0f,
                                                value = 1f
                                            }
                                        }
                                    },
                                    valueFrom = 0f,
                                    valueTo = 1f,
                                },
                                drivers = {}
                            }
                        },
                        Enabled = true,
                        Name = "Blur Saber",
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
                            },
                            Scale = new()
                            {
                                x = 1f,
                                y = 1f,
                                z = 1f
                            }
                        },
                        ForceColorOverride = false,
                        ColorOverride = new()
                        {
                            type = 0,
                            hue = 0f,
                            saturation = 1f,
                            value = 1f,
                            hueShiftPerSecond = 0f,
                            fakeGlowMultiplier = 1f
                        }
                    }
                };

                json.Modules.Add(module);

                //newCylinder.transform.RotateAround(Vector3.zero, Vector3.up, -90f); // Restore rotation for preview
            }
        }

        return json;
    }
}
