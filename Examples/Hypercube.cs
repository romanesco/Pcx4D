using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pcx4D
{
    public class Hypercube : MonoBehaviour
    {
        [SerializeField] int numPoints = 100;
        [SerializeField] float offset = 0f;
        
        Mesh CreateMesh(int n)
        {
            List<Vector3> vs = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color> cols = new List<Color>();

            for (int i=0; i<4; i++)
            {
                for (int j=0; j<8; j++)
                {
                    Color col = Color.HSVToRGB(1f / 32 * (i * 8 + j), 1, 1);
                    Vector4 p = new Vector4(-1,-1,-1,-1);
                    int l = 0;
                    for (int k=0; k<4; k++) {
                        if (k == i) continue;
                        p[k] = (j >> l) & 1;
                        l++;
                    }
                    
                    for ( l = 0; l < n; l++ )
                    {
                        float t = 1f / (n - 1) * l;
                        p[i] = (1-2*offset)*t + offset;
                        Vector4 q = 2 * p - new Vector4(1, 1, 1, 1);

                        vs.Add(new Vector3(q.x, q.y, q.z));
                        uvs.Add(new Vector2(q.w, 0f));
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
            Mesh mesh = CreateMesh(numPoints);
            gameObject.GetComponent<MeshFilter>().mesh = mesh;
        }

        // Start is called before the first frame update
        void Start()
        {
            SetMesh();
        }

        // Update is called once per frame
        void OnValidate()
        {
            SetMesh();
        }
    }
}
