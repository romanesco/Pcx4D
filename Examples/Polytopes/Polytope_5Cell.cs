using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Polytope_5Cell : MonoBehaviour
{

    [SerializeField] bool _initialized = false;
    [SerializeField] int numPoints = 100;
    [SerializeField] float[] scale = new float[5] { 1, 1, 1, 1, 1 };
    public int dimension = 1;

    private static readonly float RADIUS = Mathf.Sqrt(3.2f); // normalize the original radius to 1
    private static readonly float sq5inv = 1 / Mathf.Sqrt(5);
    static readonly Vector4[] vertices = {new Vector4(1, 1, 1, -sq5inv)/RADIUS,
      new Vector4(1, -1, -1, -sq5inv)/RADIUS,
      new Vector4(-1, 1, -1, -sq5inv)/RADIUS,
      new Vector4(-1, -1, 1, -sq5inv)/RADIUS,
      new Vector4(0, 0, 0, Mathf.Sqrt(5) - sq5inv)/RADIUS };
    Vector4 vScaled(int i) { return scale[i] * vertices[i]; }

    [SerializeField] Color color1 = new Color32(255, 75, 0, 0);
    [SerializeField] Color color2 = new Color32(255, 241, 0, 0);
    [SerializeField] Color color3 = new Color32(3, 175, 122, 0);
    [SerializeField] Color color4 = new Color32(0, 90, 255, 0);
    [SerializeField] Color color5 = new Color32(77, 196, 255, 0);


    Mesh CreateMesh(int n)
    {
        if (dimension < 0) dimension = 0;
        if (dimension > 4) dimension = 4;

        List<Vector3> vs = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Color> cols = new List<Color>();

        Color[] colors = { color1, color2, color3, color4, color5 };

        if (dimension == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                // vertices only
                vs.Add(new Vector3(vScaled(i).x, vScaled(i).y, vScaled(i).z));
                uvs.Add(new Vector2(vScaled(i).w, 0));
                cols.Add(colors[i]);
            }

        } else if (dimension == 1)
        {
            // connect all vertices
            for (int i = 0; i < 4; i++)
            {
                for (int j = i + 1; j < 5; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        Vector4 v = vScaled(i) + (vScaled(j) - vScaled(i)) * (k / (float)n);
                        Color col = colors[i] + (colors[j] - colors[i]) * (k / (float)n);
                        vs.Add(v);
                        uvs.Add(new Vector2(v.w, 0));
                        cols.Add(col);
                    }
                }
            }
        } else
        {
            // Case dim > 1: put random dots on faces
            for (int k = 0; k < n; k++)
            {
                float[] t = new float[5];
                float s = 0;

                // get a random vector of given dimension
                for (int i = 0; i <= dimension; i++)
                {
                    t[i] = Random.value;
                    s += t[i];
                }
                for (int i=dimension+1; i<5; i++)
                {
                    t[i] = 0;
                }

                // normalize (sum = 1)
                for (int i=0; i<5; i++)
                {
                    t[i] /= s;
                }

                // shuffle
                for (int i = 0; i < 4; i++)
                {
                    int j = Random.Range(i, 5);
                    (t[i], t[j]) = (t[j], t[i]); // swap
                }

                Vector4 v = Vector4.zero;
                Color col = Color.black;
                // interpolate
                for (int i = 0; i < 5; i++)
                {
                    v += t[i] * vScaled(i);
                    col += t[i] * colors[i];
                    vs.Add(v);
                    uvs.Add(new Vector2(v.w, 0));
                    cols.Add(col);
                }

            }
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(vs);
        mesh.SetUVs(1, uvs);
        mesh.SetColors(cols);
        mesh.SetIndices(
                Enumerable.Range(0, vs.Count).ToArray(),
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
            Mesh mesh = CreateMesh(numPoints);
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
            Debug.Log("Hypercube: initialize mesh.");
        }
        _initialized = true;

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
