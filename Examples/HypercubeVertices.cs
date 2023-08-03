using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pcx4D
{
    public class HypercubeVertices : MonoBehaviour
    {
        [SerializeField] Vector4 scale = Vector4.one;
        [SerializeField] Vector4 shear = Vector4.zero;
        [SerializeField] Vector4 frustum = Vector4.zero;
        Matrix4x4 matrix { get { return new Matrix4x4(
            new Vector4(scale.x, shear.x, 0, 0),
            new Vector4(0, scale.y, shear.y, 0),
            new Vector4(0, 0, scale.z, shear.z),
            new Vector4(shear.w, 0, 0, scale.w)
        );}}
        public bool useSingleColor = false;
        public Color singleColor = Color.white;

        Mesh CreateMesh()
        {
            List<Vector3> vs = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color> cols = new List<Color>();

            for (int i=0; i<16; i++)
            {
                Vector4 p = new Vector4();
                for (int k=0; k<4; k++) {
                    p[k] = (i >> k) & 1;
                }
                p = 2 * p - new Vector4(1, 1, 1, 1);
                //Debug.Log(p);
                Vector4 reg = p;
                for (int k = 0; k < 4; k++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (k == j) continue;
                        p[k] *= 1 + reg[j] * frustum[j];
                    }
                }
                p = matrix * p;

                vs.Add(new Vector3(p.x, p.y, p.z));
                uvs.Add(new Vector2(p.w, 0f));
                if (useSingleColor)
                {
                    cols.Add(singleColor);
                }
                else
                {
                    Color col = Color.HSVToRGB(1f / 16 * i, 1, 1);
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
            Mesh mesh = CreateMesh();
            gameObject.GetComponent<MeshFilter>().mesh = mesh;
        }

        // Start is called before the first frame update
        void Start()
        {
            SetMesh();
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
}
