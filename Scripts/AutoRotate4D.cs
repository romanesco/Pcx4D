using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pcx4D {
    public class AutoRotate4D : MonoBehaviour
    {
        public float period = 5;
        [SerializeField] bool assignRandomRotationAtStart = true;

        // Start is called before the first frame update
        void Start()
        {
            if (assignRandomRotationAtStart)
            {
                GetComponent<Rotate4D>().rotation4D = RandomRotation.randomDistributionOnSO4();
            }
        }

        // Update is called once per frame
        void Update()
        {
            Rotate4D rotate4D = GetComponent<Rotate4D>();
            if (period <= 0) period = 5;
            rotate4D.period = period;
            int j = (int)(Time.time / period) % 6;
            Debug.Log(Time.time);
            Debug.Log(Time.time/period);
            Debug.Log(j);
            for (int i = 0; i < 6; i++)
            {
                rotate4D.angles[i] = 0;
            }
            rotate4D.angles[j] = 1;
        }
    }

}