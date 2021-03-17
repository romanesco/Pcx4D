using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pcx4D
{
    public class Axes : MonoBehaviour
    {
        [SerializeField] int numPoints = 100;
        
        Mesh CreateMesh(int n)
        {
            List<Vector3> vs = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color> cols = new List<Color>();

            for (int i=0; i<4; i++)
            {
                Color col = Color.HSVToRGB(1f / 4 * i, 1, 1);
                Vector4 p = new Vector4(0,0,0,0);
                p[i] = 1;
                for ( int l = 0; l < n; l++ )
                {
                    float t = 1f / (n - 1) * l;
                    Vector4 q = t * p;

                    vs.Add(new Vector3(q.x, q.y, q.z));
                    uvs.Add(new Vector2(q.w, 0f));
                    cols.Add(col);
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
            SetMesh();
        }

        // Update is called once per frame
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
}
