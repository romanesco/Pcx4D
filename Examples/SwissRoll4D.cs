using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwissRoll4D : MonoBehaviour
{
    [SerializeField] int points = 2000;
    [SerializeField] bool _initialized = false;

    Vector2 SwissRoll(float x)
    {
        return new Vector2(x * Mathf.Cos(2 * Mathf.PI * x), x * Mathf.Sin(2 * Mathf.PI * x));
    }

    Mesh CreateMesh(int max)
    {

        List<Vector3> vs3 = new List<Vector3>(max);
        List<Vector2> uvs = new List<Vector2>(max);
        List<Color> colors = new List<Color>(max);
        for (int i = 0; i < max; i++)
        {
            float x = Random.value;
            Vector2 v = new Vector2(Random.value * 2 - 1, Random.value * 2 - 1),
                w = 0.5f * SwissRoll(x * 2 + 1);

            vs3.Add(new Vector3(w.x, v.x, w.y));
            //uvs.Add(new Vector2(Random.value*2-1, 0));
            uvs.Add(new Vector2(v.y, 0));
            colors.Add(Color.HSVToRGB(x, 1, 1));
        }
        var mesh = new Mesh();
        mesh.name = "SwissRoll4D";
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
            Mesh mesh = CreateMesh(points);
            GetComponent<MeshFilter>().mesh = mesh;
            Destroy(oldmesh);
        }
    }

    void Awake()
    {
        // avoid recreating mesh when Instantiate()
        if (!_initialized)
        {
            SetMesh();
        }
        _initialized = true;
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
