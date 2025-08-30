using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightEye : MonoBehaviour
{
    public bool rightEye = false;
    void OnPreRender()
    {
        if (rightEye)
        {
            Shader.SetGlobalInt("_RightEye", 1);
        }
        else
        {
            Shader.SetGlobalInt("_RightEye", 0);
        }
    }

    void OnPostRender()
    {
        // reset the value to avoid affecting other cameras
        Shader.SetGlobalInt("_RightEye", 0);
    }
}

