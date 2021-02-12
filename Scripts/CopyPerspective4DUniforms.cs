using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPerspective4DUniforms : MonoBehaviour
{
    [SerializeField] GameObject parent=null;
    [SerializeField] bool copyPointSize = false;

    // Start is called before the first frame update
    void Start()
    {
        if (parent == null)
        {
            parent = gameObject.transform.parent.gameObject;
        }

    }

    // Update is called once per frame
    void Update()
    {
        var parentMaterial = parent.GetComponent<Renderer>().sharedMaterial;
        GetComponent<Renderer>().sharedMaterial.SetMatrix("_View4D", parentMaterial.GetMatrix("_View4D"));
        GetComponent<Renderer>().sharedMaterial.SetMatrix("_Rotation4D", parentMaterial.GetMatrix("_Rotation4D"));
        GetComponent<Renderer>().sharedMaterial.SetVector("_Camera4D", parentMaterial.GetVector("_Camera4D"));

        GetComponent<Renderer>().sharedMaterial.SetFloat("_FoV", parentMaterial.GetFloat("_FoV"));
        GetComponent<Renderer>().sharedMaterial.SetVector("_Translation4D", parentMaterial.GetVector("_Translation4D"));

        if (copyPointSize)
        {
            GetComponent<Renderer>().sharedMaterial.SetFloat("_PointSize", parentMaterial.GetFloat("_PointSize"));
        }

    }
}
