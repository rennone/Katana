using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Platforms
{
    public class Rotater : MonoBehaviour
    {
        public bool isReft = true;

        // Update is called once per frame
        void Update ()
        {
            float direct = 1;
            if (isReft)
            {
                direct = -1;
            }
            transform.Rotate(new Vector3(0,direct,0));
        }
    }
}
