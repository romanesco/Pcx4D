using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Mathematics.math;

using Unity.Mathematics;

public class Perspective4D : MonoBehaviour {

    [SerializeField] Vector4 _camera4D = new Vector4(0f, 0f, 0f, 3f);
    [SerializeField] float _fieldOfView = 90;

    [SerializeField] bool _swing = true;
    [SerializeField] Vector4 _swingVector = new Vector4(1f, 0f, 0f, 0f);
    [SerializeField] float _period = 2f;

    public Matrix4x4 LookAt4D(Vector4 eye, Vector4 center, Vector4 _up1, Vector4 _up2)
    {
        float4 v1 = -(center - eye), v2, v3;
        float4 up1 = (float4)_up1;
        float4 up2 = (float4)_up2;
        float a = determinant(float3x3(v1.yzw, up1.yzw, up2.yzw));
        float b = -determinant(float3x3(v1.xzw, up1.xzw, up2.xzw));
        float c = determinant(float3x3(v1.xyw, up1.xyw, up2.xyw));
        float d = -determinant(float3x3(v1.xyz, up1.xyz, up2.xyz));
        Vector4 w = new Vector4(a, b, c, d);

        v1 = normalize(v1);
        v2 = normalize(up1 - dot(v1, up1) * v1);
        v3 = normalize(up2 - Vector4.Dot(v1, up2) * v1 - Vector4.Dot(v2, up2) * v2);

        w.Normalize();
        return new Matrix4x4(w, (Vector4)v2, (Vector4)v3, (Vector4)v1).transpose;
    }

    // Use this for initialization
    void Start () {
        var tempMaterial = GetComponent<Renderer>().material;
        GetComponent<Renderer>().sharedMaterial = tempMaterial;
        SetVariables(_camera4D);
	}

    
    void SetVariables(Vector4 c) {
        Matrix4x4 _view4D = LookAt4D(c, Vector4.zero, new Vector4(0f, 1f, 0f, 0f), new Vector4(0f, 0f, 1f, 0f));
        GetComponent<Renderer>().sharedMaterial.SetVector("_Camera4D", c);
        GetComponent<Renderer>().sharedMaterial.SetFloat("_FoV", _fieldOfView);
        GetComponent<Renderer>().sharedMaterial.SetMatrix("_View4D", _view4D);
    }

    private void OnValidate()
    {
        if (!_swing) {
            SetVariables(_camera4D);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector4 c = _camera4D;
        if (_swing)
        {
            c += Mathf.Sin(Time.time * 2f * Mathf.PI / _period) * _swingVector;
        }
        SetVariables(c);

    }
}
