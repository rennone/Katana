using UnityEngine;
using System.Collections;

namespace Katana
{
    public class SearchEye : MonoBehaviour
    {
        public int range = 1;
        public int angle = 10;

        public bool Find { get; private set; }

        public System.Action OnFind { private get; set; }
        public System.Action OnLoseSight {private get; set; }

        private float searchTime = 1.0f;
        private float _lossingSightTime = 0.0f;

        void Update()
        {
            var player = GameManager.Instance.Player;
            Vector2 st = new Vector2(transform.position.x, transform.position.y);
            Vector2 ds = new Vector2(player.transform.position.x, player.transform.position.y);
                              
            var find = Check(st, ds);


            if (find)
            {
                _lossingSightTime = 0;

                if (!Find && OnFind != null)
                {
                    OnFind();
                }
                Find = true;
            }
            else
            {
                if (Find)
                {
                    _lossingSightTime += Time.deltaTime;
                    if (_lossingSightTime > searchTime)
                    {
                        OnLoseSight();
                        Find = false;
                    }
                }
            }

        }

        bool Check(Vector2 st, Vector2 ds)
        {
            // 距離が離れている
           // if ((st - ds).sqrMagnitude > range * range)
            //    return false;

            System.Random r = new System.Random();

            var ang = r.Next(-angle / 2, angle/2) * Mathf.Deg2Rad;
            var si = Mathf.Sin(ang);
            var co = Mathf.Cos(ang);
            Vector3 direction = new Vector3( transform.forward.x * co - transform.forward.y * si, transform.forward.x * si + transform.forward.y * co );

            RaycastHit hit;
            if( Physics.Raycast( new Vector3(st.x, st.y, 0), direction, out hit, range ))
            {
                if (hit.collider.gameObject.tag == TagName.Player)
                {
                    return true;
                }
            }
            

            return false;
        }
    }
}