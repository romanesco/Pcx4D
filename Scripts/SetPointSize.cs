using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pcx
{
    public class SetPointSize : MonoBehaviour
    {
        [SerializeField] public float PointSize = 0.002f;

        // Use this for initialization
        void Start()
        {
            var tempMaterial = GetComponent<Renderer>().material;
            tempMaterial.SetFloat("_PointSize", PointSize);
            GetComponent<Renderer>().sharedMaterial = tempMaterial;
        }

        // Update is called once per frame
        void Update()
        {
            GetComponent<Renderer>().sharedMaterial.SetFloat("_PointSize", PointSize);
        }
    }
}
