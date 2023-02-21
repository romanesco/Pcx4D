using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pcx4D
{
    public static class RandomRotation
    {
        public static Matrix4x4 randomDistributionOnSO3()
        {
            Quaternion q = Random.rotation;
            Matrix4x4 A = Matrix4x4.Rotate(q);
            
            return A;
        }

        public static Matrix4x4 randomDistributionOnXY_ZW()
        {
            float t1 = Random.value, t2 = Random.value;
            float c1 = Mathf.Cos(t1), s1 = Mathf.Sin(t1), c2 = Mathf.Cos(t2), s2 = Mathf.Sin(t2);
            Matrix4x4 A = Matrix4x4.zero;
            A[0, 0] = c1;
            A[0, 1] = -s1;
            A[1, 0] = s1;
            A[1, 1] = c1;
            A[2, 2] = c2;
            A[2, 3] = -s2;
            A[3, 2] = s2;
            A[3, 3] = c2;

            return A;
        }

        public static Matrix4x4 randomDistributionOnSO4()
        {
            Quaternion q1 = Random.rotation;
            Quaternion q2 = Random.rotation;
            Debug.Log(q1);
            Debug.Log(q1.normalized);
            Debug.Log(q2);
            Debug.Log(q2.normalized);

            Matrix4x4 A1 = new Matrix4x4(), A2 = new Matrix4x4();
            A1[0, 0] = q1.x;
            A1[0, 1] = -q1.y;
            A1[0, 2] = -q1.z;
            A1[0, 3] = -q1.w;
            A1[1, 0] = q1.y;
            A1[1, 1] = q1.x;
            A1[1, 2] = -q1.w;
            A1[1, 3] = q1.z;
            A1[2, 0] = q1.z;
            A1[2, 1] = q1.w;
            A1[2, 2] = q1.x;
            A1[2, 3] = -q1.y;
            A1[3, 0] = q1.w;
            A1[3, 1] = -q1.z;
            A1[3, 2] = q1.y;
            A1[3, 3] = q1.x;

            A2[0, 0] = q1.x;
            A2[0, 1] = -q1.y;
            A2[0, 2] = -q1.z;
            A2[0, 3] = -q1.w;
            A2[1, 0] = q1.y;
            A2[1, 1] = q1.x;
            A2[1, 2] = q1.w;
            A2[1, 3] = -q1.z;
            A2[2, 0] = q1.z;
            A2[2, 1] = -q1.w;
            A2[2, 2] = q1.x;
            A2[2, 3] = q1.y;
            A2[3, 0] = q1.w;
            A2[3, 1] = q1.z;
            A2[3, 2] = -q1.y;
            A2[3, 3] = q1.x;
            Debug.Log(A1 * A1.transpose);
            Debug.Log(A2 * A2.transpose);

            Debug.Log((A1 * A2) * (A1 * A2).transpose);

            return A1 * A2;
        }
    }
}