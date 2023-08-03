using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pcx4D
{
    public class Tilt4D : MonoBehaviour
    {
        [SerializeField] float _tiltAngle = 1f; // in degree
        [SerializeField] bool _OppositeTilt = false;

        public float tiltAngle
        {
            get { return _tiltAngle; }
            set { _tiltAngle = value; SetMatrices(); }
        }
        public bool oppositeTilt
        {
            get { return _OppositeTilt; }
            set { _OppositeTilt = value; SetMatrices(); }
        }

        void OnEnable()
        {
            var tempMaterial = GetComponent<Renderer>().material;
            GetComponent<Renderer>().sharedMaterial = tempMaterial;
            SetMatrices();
        }

        private void OnValidate()
        {
            SetMatrices();
        }

        void SetMatrices()
        {
            float delta = _tiltAngle / 180 * Mathf.PI; // convert to radian
            float c = Mathf.Cos(delta), s = Mathf.Sin(delta);
            //float t = Time.time, c = Mathf.Cos(t), s = Mathf.Sin(t);
            Matrix4x4 _tilt4D_LeftEye = new Matrix4x4(new Vector4(c, 0, 0, s), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(-s, 0, 0, c));
            Matrix4x4 _tilt4D_RightEye = new Matrix4x4(new Vector4(c, 0, 0, -s), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(s, 0, 0, c));

            GetComponent<Renderer>().sharedMaterial.SetMatrix("_Tilt4D_LeftEye", _tilt4D_LeftEye);
            if (_OppositeTilt)
            {
                GetComponent<Renderer>().sharedMaterial.SetMatrix("_Tilt4D_RightEye", _tilt4D_RightEye);
            }
            else
            {
                GetComponent<Renderer>().sharedMaterial.SetMatrix("_Tilt4D_RightEye", _tilt4D_LeftEye);
            }
        }

        Matrix4x4 id4 = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 0, 0, 1));
        private void OnDisable()
        {
            Matrix4x4 _tilt4D_LeftEye = id4; //new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 0, 0, 1));
            Matrix4x4 _tilt4D_RightEye = id4; // new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 0, 0, 1));

            GetComponent<Renderer>().sharedMaterial.SetMatrix("_Tilt4D_LeftEye", _tilt4D_LeftEye);
            GetComponent<Renderer>().sharedMaterial.SetMatrix("_Tilt4D_RightEye", _tilt4D_RightEye);

        }

        void Update()
        {
            GetComponent<Renderer>().sharedMaterial.SetMatrix("_VMain", Camera.main.worldToCameraMatrix);
        }
    }

}