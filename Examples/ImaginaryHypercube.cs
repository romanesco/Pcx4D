using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImaginaryHypercube : MonoBehaviour
{
    [SerializeField] int Iteration = 6;

    Vector4[] ps = { new Vector4(1,1,1,1),
                     new Vector4(1,1,-1,-1),
                     new Vector4(1,-1,1,-1),
                     new Vector4(-1,1,1,-1),
                     new Vector4(1,-1,-1,1),
                     new Vector4(-1,1,-1,1),
                     new Vector4(-1,-1,1,1),
                     new Vector4(-1,-1,-1,-1)};

    Color[] cols = { new Color(1,0,0),
                     new Color(0,1,0),
                     new Color(0,0,1),
                     new Color(1,1,0),
                     new Color(0,1,1),
                     new Color(1,0,1),
                     new Color(0.5f,0.5f,0.5f),
                     new Color(0,0.5f,1)
    };

    void Iterate(Vector4 q, float c, int depth, List<Vector4> vector4s, List<Color> colors, int lastindex = 0)
    {
        //Debug.LogFormat("q: {0}, depth: {1}", q, depth);
        if (depth <= 0)
        {
            vector4s.Add(q);
            colors.Add(cols[lastindex]);
            return;
        }
        else
        {

            for (int i = 0; i < 8; i++)
            {
                Iterate(c * (q - ps[i]) + ps[i], c, depth - 1, vector4s, colors, i);
            }
        }
    }

    // x^y
    int Pow(int x, int y)
    {
        int z = 1;
        for (int i=0; i<y; i++)
        {
            z *= x;
        }
        return z;
    }

    Mesh CreateMesh()
    {
        int N = Pow(8, Iteration);
        List<Vector4> vector4s = new List<Vector4>(N);
        List<Color> colors = new List<Color>(N);
        Iterate(new Vector4(0, 0, 0, 0), 0.5f, Iteration, vector4s, colors);

        N = vector4s.Count; // unnecessary?

        List<Vector3> vs3 = new List<Vector3>(N);
        List<Vector2> uvs = new List<Vector2>(N);

        for (int i=0; i<N; i++)
        {
            Vector4 v = vector4s[i];
            vs3.Add(new Vector3(v.x, v.y, v.z));
            uvs.Add(new Vector2(v.w, 0));
            //colors.Add(new Color32(255, 255, 255, 255));
        }

        var mesh = new Mesh();
        mesh.name = "ImaginaryHypercube";
        mesh.SetVertices(vs3);
        mesh.SetUVs(1, uvs);
        mesh.SetColors(colors);
        mesh.SetIndices(
                    Enumerable.Range(0, vs3.Count).ToArray(),
                    MeshTopology.Points, 0
                );
        return mesh;
    }

    void SetMesh()
    {
        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter)
        {
            var oldmesh = meshFilter.sharedMesh;
            Mesh mesh = CreateMesh();
            GetComponent<MeshFilter>().mesh = mesh;
            Destroy(oldmesh);
        }
    }
    
    void Awake()
    {
        SetMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            SetMesh();
        }
        else
        {
            // reduce the size of the scene
            if (GetComponent<MeshFilter>())
            {
                GetComponent<MeshFilter>().mesh = null;
            }
        }
    }
}
