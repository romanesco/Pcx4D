using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pcx4D
{
    public class HypercubeVertices : MonoBehaviour
    {
        
        Mesh CreateMesh()
        {
            List<Vector3> vs = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color> cols = new List<Color>();

            for (int i=0; i<16; i++)
            {
                Color col = Color.HSVToRGB(1f / 16 * i, 1, 1);
                Vector4 p = new Vector4();
                for (int k=0; k<4; k++) {
                    p[k] = (i >> k) & 1;
                }
                p = 2 * p - new Vector4(1, 1, 1, 1);
                //Debug.Log(p);

                vs.Add(new Vector3(p.x, p.y, p.z));
                uvs.Add(new Vector2(p.w, 0f));
                cols.Add(col);
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
            Mesh mesh = CreateMesh();
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
