using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

// 🖕 para los que usan trigonometría (lookin at u appsl)
public class NewMonoBehaviourScript : MonoBehaviour
{
    public Mesh mesh;
    public GameObject cylinder;

    public struct Edge
    {
        public int a;
        public int b;
    };
    const string baseBegin = "{\"ModVersion\": \"0.3.12\",\"Version\": 1,\"LocalTransform\": {\"Position\": {\"x\": 0.0,\"y\": 0.0,\"z\": 0.0},\"Rotation\": {\"x\": 0.0,\"y\": 0.0,\"z\": 0.0},\"Scale\": {\"x\": 1.0,\"y\": 1.0,\"z\": 1.0}},\"Modules\": [";
    const string baseEnd = "]}";
    const string module1 = "{\"ModuleId\":\"reezonate.blur-saber\",\"Version\":1,\"Config\":{\"SaberSettings\":{\"zOffsetFrom\":0.0,\"zOffsetTo\":";
    const string module2 = ",\"thickness\":0.025,\"saberProfile\":{\"interpolationType\":1,\"controlPoints\":[{\"time\":0.0,\"value\":1.0}]},\"startCap\":true,\"endCap\":true,\"verticalResolution\":2,\"horizontalResolution\":1,\"renderQueue\":3002,\"cullMode\":0,\"depthWrite\":false,\"blurFrames\":1.0,\"glowMultiplier\":1.0,\"handleRoughness\":2.0,\"handleColor\":{\"r\":0.1,\"g\":0.1,\"b\":0.1,\"a\":0.0},\"maskSettings\":{\"bladeMaskResolution\":256,\"driversMaskResolution\":32,\"handleMask\":{\"interpolationType\":2,\"controlPoints\":[{\"time\":0.0,\"value\":0.0}]},\"bladeMappings\":{\"colorOverValue\":{\"interpolationType\":0,\"controlPoints\":[{\"time\":0.0,\"value\":{\"r\":1.0,\"g\":1.0,\"b\":1.0,\"a\":1.0}}]},\"alphaOverValue\":{\"interpolationType\":0,\"controlPoints\":[{\"time\":0.0,\"value\":1.0}]},\"scaleOverValue\":{\"interpolationType\":0,\"controlPoints\":[{\"time\":0.0,\"value\":1.0}]},\"valueFrom\":0.0,\"valueTo\":1.0},\"driversSampleMode\":0,\"viewingAngleMappings\":{\"colorOverValue\":{\"interpolationType\":0,\"controlPoints\":[{\"time\":0.0,\"value\":{\"r\":1.0,\"g\":1.0,\"b\":1.0,\"a\":1.0}}]},\"alphaOverValue\":{\"interpolationType\":0,\"controlPoints\":[{\"time\":0.0,\"value\":1.0}]},\"scaleOverValue\":{\"interpolationType\":0,\"controlPoints\":[{\"time\":0.0,\"value\":1.0}]},\"valueFrom\":0.0,\"valueTo\":1.0},\"surfaceAngleMappings\":{\"colorOverValue\":{\"interpolationType\":0,\"controlPoints\":[{\"time\":0.0,\"value\":{\"r\":1.0,\"g\":1.0,\"b\":1.0,\"a\":1.0}}]},\"alphaOverValue\":{\"interpolationType\":0,\"controlPoints\":[{\"time\":0.0,\"value\":1.0}]},\"scaleOverValue\":{\"interpolationType\":0,\"controlPoints\":[{\"time\":0.0,\"value\":1.0}]},\"valueFrom\":0.0,\"valueTo\":1.0},\"drivers\":[]}},\"Enabled\":true,\"Name\":\"BlurSaber\",\"LocalTransform\":{\"Position\":{\"x\":";
    const string module3 = ",\"y\":";
    const string module4 = ",\"z\":";
    const string module5 = "},\"Rotation\":{\"x\":";
    const string module6 = ",\"y\":";
    const string module7 = ",\"z\":";
    const string module8 = "},\"Scale\":{\"x\":1.0,\"y\":1.0,\"z\":1.0}},\"ForceColorOverride\":false,\"ColorOverride\":{\"type\":0,\"hue\":0.0,\"saturation\":1.0,\"value\":1.0,\"hueShiftPerSecond\":0.0,\"fakeGlowMultiplier\":1.0}}},";

    void Start()
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        List<Edge> edges = new();
        for(int i = 0; i < triangles.Length; i+=3)
        {
            Edge a = new();
            Edge b = new();
            Edge c = new();

            a.a = triangles[i];
            a.b = triangles[i+1];
            b.a = triangles[i+1];
            b.b = triangles[i+2];
            c.a = triangles[i+2];
            c.b = triangles[i];

            edges.Add(a);
            edges.Add(b);
            edges.Add(c);
        }

        // Remove dupe edges
        for(int i = 0; i < edges.Count-1; i++)
        {
            for(int j = i+1; j < edges.Count; j++)
            {
                float distAA = Vector3.Distance(vertices[edges[i].a],vertices[edges[j].a]);
                float distAB = Vector3.Distance(vertices[edges[i].a],vertices[edges[j].b]);
                float distBA = Vector3.Distance(vertices[edges[i].b],vertices[edges[j].a]);
                float distBB = Vector3.Distance(vertices[edges[i].b],vertices[edges[j].b]);
                if((distAA < 0.001f && distBB < 0.001f) || (distAB < 0.001f && distBA < 0.001f))
                {
                    edges.RemoveAt(j);
                    j--;
                }
            }
        }

        string json = baseBegin;
        for(int i = 0; i < edges.Count-1; i++)
        {
            GameObject newCylinder = Instantiate(cylinder, vertices[edges[i].a], Quaternion.identity);
            newCylinder.transform.LookAt(vertices[edges[i].b]);
            float distance = Vector3.Distance(vertices[edges[i].a],vertices[edges[i].b]);
            newCylinder.transform.localScale = new Vector3(0.0025f,0.0025f,distance);

            json += module1;
            json += distance.ToString();
            json += module2;
            json += vertices[edges[i].a].x.ToString();
            json += module3;
            json += vertices[edges[i].a].y.ToString();
            json += module4;
            json += vertices[edges[i].a].z.ToString();
            json += module5;
            json += newCylinder.transform.eulerAngles.x.ToString();
            json += module6;
            json += newCylinder.transform.eulerAngles.y.ToString();
            json += module7;
            json += newCylinder.transform.eulerAngles.z.ToString();
            json += module8;
        }
        json += baseEnd;

        if(File.Exists("unity.json"))
        {
            Debug.Log("File already exists!");
        }
        else
        {
            var sr = File.CreateText("unity.json");
            sr.WriteLine(json);
            sr.Close();
        }

        // ¿Donde se guarda? idfk
        // Edit: Ya se donde, en la carpeta del proyecto de unity (no en Assets, en el nivel superior)
    }

    
    void Update()
    {
        
    }
}
