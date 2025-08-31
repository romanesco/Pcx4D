using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


namespace Pcx4D
{
    public class Rotate4D : MonoBehaviour
    {

        public float period = 5;
        public bool autoRotate = true;
        bool oldAutoRotate = true;

        [SerializeField] bool stopOnDisable = true;

        public Matrix4x4 rotation4D= Matrix4x4.identity;

        public float[] angles = new float[] { 1, 0, 0, 0, 0, 1 };

        float offset = 0;
        float disableTime = 0;

        void Start()
        {
            var tempMaterial = GetComponent<Renderer>().material;
            GetComponent<Renderer>().sharedMaterial = tempMaterial;

            oldAutoRotate = autoRotate;
        }

        Matrix4x4 MatrixExponential(Matrix4x4 m)
        {
            const int deg = 5;
            Matrix4x4 A = Matrix4x4.identity, B = Matrix4x4.identity;
            int j = 1;
            for (int i=1; i<deg; i++)
            {
                j *= i;
                B *= m;
                A += 1.0f / j * (Unity.Mathematics.float4x4) B;
            }
            // need to make A orthogonal?
            return A;
        }

        // Update is called once per frame
        void OldUpdate ()
        {
            float t = (Time.time - offset) * 2 * Mathf.PI / period,
                c = Mathf.Cos(t), s = Mathf.Sin(t);
            Matrix4x4 _rotation4D = new Matrix4x4(new Vector4(c, s, 0, 0), new Vector4(-s, c, 0, 0), new Vector4(0, 0, c, -s), new Vector4(0, 0, s, c));
            GetComponent<Renderer>().sharedMaterial.SetMatrix("_Rotation4D", _rotation4D);
        }

        void OldUpdate2()
        {
            float t = Time.deltaTime * 2 * Mathf.PI / period,
                c = Mathf.Cos(t), s = Mathf.Sin(t);
            Matrix4x4 deltaRotation4D = new Matrix4x4(new Vector4(c, s, 0, 0), new Vector4(-s, c, 0, 0), new Vector4(0, 0, c, -s), new Vector4(0, 0, s, c));
            rotation4D = deltaRotation4D * rotation4D;
            GetComponent<Renderer>().sharedMaterial.SetMatrix("_Rotation4D", rotation4D);
        }

        private void Update()
        {
            if (!autoRotate) return;

            Matrix4x4 B = Matrix4x4.zero;
            float t = Time.deltaTime * 2 * Mathf.PI/period;
            B[0, 1] = t * angles[0];
            B[0, 2] = t * angles[1];
            B[0, 3] = t * angles[2];
            B[1, 2] = t * angles [3];
            B[1, 3] = t * angles [4];
            B[2, 3] = t * angles [5];

            B[1, 0] = -t * angles[0];
            B[2, 0] = -t * angles[1];
            B[3, 0] = -t * angles[2];
            B[2, 1] = -t * angles[3];
            B[3, 1] = -t * angles[4];
            B[3, 2] = -t * angles[5];

            rotation4D = MatrixExponential(B) * rotation4D;
            GetComponent<Renderer>().sharedMaterial.SetMatrix("_Rotation4D", rotation4D);
        }

        void StartRotation()
        {
            offset += Time.time - disableTime;        
        }
        void StopRotation()
        {   
            disableTime = Time.time;
        }

        private void OnEnable()
        {
            if (stopOnDisable)
            {
                StartRotation();
            }
        }

        private void OnDisable()
        {
            StopRotation();
        }
        
        void OnValidate()
        {
            if (autoRotate != oldAutoRotate)
            {
                if (autoRotate)
                {
                    StartRotation();
                }
                else
                {
                    StopRotation();
                }
                oldAutoRotate = autoRotate;
            }

            if (angles.Length != 6)
            {
                Debug.LogWarning("Don't change the 'angles' field's array size!");
                System.Array.Resize(ref angles, 6);
            }
        }
    }
}
