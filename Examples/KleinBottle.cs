using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KleinBottle : MonoBehaviour
{
    [SerializeField] float R = 0.25f;
    [SerializeField] float P = 1.0f;
    [SerializeField] float epsilon = 0.25f;
    [SerializeField] int N = 256;

    float r = 1.0f;
    float a = 3.0f;

    // embedding from 
    // https://math.stackexchange.com/questions/330856/how-to-embed-klein-bottle-into-r4
    Vector4 kleinbottle(Vector2 t)
    {
        float cx = Mathf.Cos(t.x), sx = Mathf.Sin(t.x), cy = Mathf.Cos(t.y), sy = Mathf.Sin(t.y),
              cx2 = Mathf.Cos(t.x / 2), sx2 = Mathf.Sin(t.x / 2);
        float s = r * cy + a;
        return new Vector4(s * cx, s * sx, r * sy * cx2, r * sy * sx2);
    }

    // embedding from
    // https://en.wikipedia.org/wiki/Klein_bottle#4-D_non-intersecting
    Vector4 kleinbottle2(Vector2 t)
    {
        float cx = Mathf.Cos(t.x), sx = Mathf.Sin(t.x), cy = Mathf.Cos(t.y), sy = Mathf.Sin(t.y),
          cx2 = Mathf.Cos(t.x / 2), sx2 = Mathf.Sin(t.x / 2),
          s2y = Mathf.Sin(2 * t.y);
        return new Vector4(R * (cx2 * cy - sx2 * s2y),
                 R * (sx2 * cy + cx2 * s2y),
                 P * cx * (1 + epsilon * sy),
                 P * sx * (1 + epsilon * sy));
    }

    // embedding from 
    // http://www.ams.org/journals/bull/1941-47-06/S0002-9904-1941-07501-4/
    Vector4 kleinbottle3(Vector2 t)
    {
        float cx = Mathf.Cos(t.x), sx = Mathf.Sin(t.x), cy = Mathf.Cos(t.y), sy = Mathf.Sin(t.y),
              cx2 = Mathf.Cos(t.x / 2), sx2 = Mathf.Sin(t.x / 2);
        return new Vector4(cy * cx, cy * sx, 2 * sy * cx2, 2 * sy * sx2);
    }

    Mesh CreateMesh()
    {
        int count = N * N;
        List<Vector3> vs3 = new List<Vector3>(count);
        List<Vector2> uvs = new List<Vector2>(count);
        List<Color> colors = new List<Color>(count);

        Vector2 t;
        for (int i = 0; i < N; i++)
        {
            t.x = 2 * Mathf.PI * i / N;
            for (int j = 0; j < N; j++)
            {
                t.y = 2 * Mathf.PI * j / N;

                Vector4 v = kleinbottle2(t);
                vs3.Add(new Vector3(v.z, v.x, v.w));
                uvs.Add(new Vector2(v.y, 0));

                if ( ( (i % 8) == 0 )|| ( (j % 8) == 0 ) )
                {
                    colors.Add(Color.white);
                }
                else
                {
                    colors.Add(new Color((float)i / N, (float)j / N, 1));
                }
            }
        }

        var mesh = new Mesh();
        mesh.name = "KleinBottle";
        mesh.SetVertices(vs3);
        mesh.SetUVs(1, uvs);
        mesh.SetColors(colors);
        mesh.SetIndices( Enumerable.Range(0, vs3.Count).ToArray(),
                    MeshTopology.Points, 0
                );
        return mesh;
    }

    void SetMesh()
    {
        var mesh = CreateMesh();
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
    }
    // Start is called before the first frame update
    void Start()
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
            gameObject.GetComponent<MeshFilter>().mesh = null;
        }
    }
}
