using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pcx4D
{
    public class nDCube : MonoBehaviour
    {
        [SerializeField] int numPoints = 1000;
        [SerializeField] float offset = 0f;
        [SerializeField] float size = 1f;
        public int objectDimension = 4;
        public int faceDimension = 1;
        public bool chiral = false;

        [SerializeField] Color color1 = new Color(0.75f, 0, 0);
        [SerializeField] Color color2 = new Color(0, 0.75f, 0);
        [SerializeField] Color color3 = new Color(0, 0, 0.75f);
        [SerializeField] Color color4 = new Color(0.25f, 0.25f, 0.25f);

        List<List<int>> combinations = new List<List<int>>();

        [SerializeField] bool _initialized = false;

        void InitCombination(int n=4)
        {            
            for (int k=0; k<=n; k++)
            {
                combinations.Add(new List<int>());
            }

            for (int i=0; i< (1<<n); i++)
            {
                int count = 0;
                int j = i;
                while (j>0)
                {
                    count += j & 1;
                    j >>= 1;
                }
                combinations[count].Add(i);
            }
        }

        // random distribution in the specified dimensional cells
        Mesh CreateMesh2( int n )
        {
            if (objectDimension < 0) objectDimension = 0;
            if (objectDimension > 4) objectDimension = 4;

            InitCombination(objectDimension);

            if (faceDimension < 0) faceDimension = 0;
            if (faceDimension > objectDimension) faceDimension = objectDimension;

            List<Vector3> vs = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color> cols = new List<Color>();

            if (combinations.Count > 0)
            {
                foreach (var l in combinations[faceDimension])
                {
                    for (int i = 0; i < n; i++)
                    {
                        Vector4 v = new Vector4();
                        for (int k = 0; k < objectDimension; k++)
                        {
                            if ((l & (1 << k)) > 0)
                            {
                                v[k] = Random.value * 2 - 1;
                            }
                            else
                            {
                                v[k] = (Random.value < 0.5) ? -1 : 1;
                            }
                        }
                        vs.Add(new Vector3(v.x, v.y, v.z) * size);
                        uvs.Add(new Vector2((chiral ? -1 : 1) * v.w, 0) * size);
                        cols.Add((v.x + 1) / 2 * color1 + (v.y + 1) / 2 * color2 + (v.z + 1) / 2 * color3 + (v.w + 1) / 2 * color4);
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

        // points on edges (equal interval)
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
            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter)
            {
                var oldmesh = meshFilter.sharedMesh;
                Mesh mesh = CreateMesh2(numPoints);
                GetComponent<MeshFilter>().mesh = mesh;
                Destroy(oldmesh);
            }
        }

        void Awake()
        {
            // InitCombination();

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
            } else
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
