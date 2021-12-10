using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pcx4D
{
    public class Axes : MonoBehaviour
    {
        [SerializeField] int numPoints = 100;

        [SerializeField] Color color1 = Color.HSVToRGB(0, 1, 1);
        [SerializeField] Color color2 = Color.HSVToRGB(0.25f, 1, 1);
        [SerializeField] Color color3 = Color.HSVToRGB(0.5f, 1, 1);
        [SerializeField] Color color4 = Color.HSVToRGB(0.75f, 1, 1);

        Color[] colors = new Color[4];

        [SerializeField] Vector4 length = new Vector4(1, 1, 1, 1);

        [SerializeField] bool _initialized = false;

        public Matrix4x4 matrix = Matrix4x4.identity;

        Mesh CreateMesh(int n)
        {
            List<Vector3> vs = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color> cols = new List<Color>();

            for (int i=0; i<4; i++)
            {
                Vector4 p = new Vector4(0,0,0,0);
                p[i] = 1;
                for ( int l = 0; l < n*length[i]; l++ )
                {
                    float t = 1f / (n - 1) * l;
                    Vector4 q = t * p;

                    q = matrix.transpose * q;

                    vs.Add(new Vector3(q.x, q.y, q.z));
                    uvs.Add(new Vector2(q.w, 0f));
                    cols.Add(colors[i]);
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

        void SetColors()
        {
            colors[0] = color1;
            colors[1] = color2;
            colors[2] = color3;
            colors[3] = color4;
        }

        void Awake()
        {
            SetColors();
            
            // avoid recreating mesh when Instantiate()
            if (!_initialized)
            {
                SetMesh();
                Debug.Log("Axes: initialize mesh.");
            }
            _initialized = true;
        }

        // Update is called once per frame
        void OnValidate()
        {
            SetColors();

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
}
