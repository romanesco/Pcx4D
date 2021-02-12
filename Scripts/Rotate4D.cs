using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pcx4D
{
    public class Rotate4D : MonoBehaviour
    {

        [SerializeField] float period = 5;
        [SerializeField] bool stopOnDisable = true;

        float offset = 0;
        float disableTime = 0;

        private void OnEnable()
        {
            var tempMaterial = GetComponent<Renderer>().material;
            GetComponent<Renderer>().sharedMaterial = tempMaterial;
            if (stopOnDisable)
            {
                offset += Time.time - disableTime;
            }
        }

        // Update is called once per frame
        void Update()
        {
            float t = (Time.time - offset) * 2 * Mathf.PI / period,
                c = Mathf.Cos(t), s = Mathf.Sin(t);
            Matrix4x4 _rotation4D = new Matrix4x4(new Vector4(c, s, 0, 0), new Vector4(-s, c, 0, 0), new Vector4(0, 0, c, -s), new Vector4(0, 0, s, c));
            GetComponent<Renderer>().sharedMaterial.SetMatrix("_Rotation4D", _rotation4D);
        }

        private void OnDisable()
        {
            disableTime = Time.time;
        }

    }
}
