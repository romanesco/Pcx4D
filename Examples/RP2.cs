using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pcx4D
{
    public class RP2 : MonoBehaviour
    {
        [SerializeField] int numPoints = 256;
        [SerializeField] float offset = 0f;
        public bool chiral = false;

        // convert an element in S^2 to RP^2
        // (x,y,z) -> (xy, xz, y^2-z^2, 2yz)
        Vector4 S2toRP2(Vector3 t)
        {
            return new Vector4(t.x * t.y, t.x * t.z, t.y * t.y - t.z * t.z, 2 * t.y * t.z);
        }

        // random distribution in the specified dimensional cells
        Mesh CreateMesh2(int n)
        {
            List<Vector3> vs = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color> cols = new List<Color>();

            for (int i = 0; i < n; i++) {
                Vector2 v = new Vector2(Random.value, Random.value);
                cols.Add(new Color(v.x, v.y, 1));
                v *= Mathf.PI;
                Vector3 t = new Vector3(Mathf.Cos(v.x)*Mathf.Sin(v.y), Mathf.Sin(v.x)*Mathf.Sin(v.y),Mathf.Cos(v.y));
                Vector4 q = S2toRP2(t);

                vs.Add(new Vector3(q.x, q.y, q.z));
                uvs.Add(new Vector2((chiral ? -1 : 1) * q.w, 0));
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

        // points on edges (equal interval)
        Mesh CreateMesh(int n)
        {
            List<Vector3> vs = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color> cols = new List<Color>();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    float x = i * Mathf.PI / n, y = j * Mathf.PI / n;
                    Vector3 t = new Vector3(Mathf.Cos(x) * Mathf.Sin(y), Mathf.Sin(x) * Mathf.Sin(y), Mathf.Cos(y));
                    Vector4 q = S2toRP2(t);

                    if (chiral) { q.w = -q.w; }

                    Color col = new Color((float)i / n, (float)j / n, 1);

                    if ( ((i % 8) == 0) || ( (j % 8) == 0) )
                    {
                        col = Color.white;
                    }

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
            Mesh mesh = CreateMesh(numPoints);
            gameObject.GetComponent<MeshFilter>().mesh = mesh;
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
            } else
            {
                // reduce the size of the scene
                gameObject.GetComponent<MeshFilter>().mesh = null;
            }
        }

    }
}
