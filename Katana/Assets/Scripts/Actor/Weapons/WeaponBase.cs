using UnityEngine;
using System.Collections;

namespace Katana
{
    public class WeaponBase : AMonoBehaviour
    {
        [SerializeField] private Collider _collider; //あたり判定用コライダ

        [SerializeField] private Character _owner;

        protected Character Owner { get { return _owner; } }



        // Use this for initialization
        protected override sealed void Awake()
        {
            _collider = GetComponent<Collider>();
            base.Awake();
        }

        protected override sealed void Start()
        {
            _collider.isTrigger = true;
            SetActive(false);
            base.Start();
        }


        // Update is called once per frame
        protected override sealed void Update()
        {
            base.Update();
        }
        

        public virtual void SetActive(bool enable)
        {
            enabled = enable;
            _collider.enabled = enable;
        }

        public virtual void SetVisible(bool enable)
        {
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            switch (collider.tag)
            {
                case TagName.Gimmick:
                    OnCollideGimmick(collider.GetComponent<GimmickBase>(), collider);
                    break;
                case TagName.Player:
                    OnCollidePlayer(collider.GetComponent<Player>(), collider);
                    break;
                case TagName.Enemy:
                    OnCollideEnemy(collider);
                    break;
                default:
                    OnCollideOther(collider);
                    break;
            }
        }

        protected virtual void OnCollideGimmick(GimmickBase gimmick, Collider collider)
        {
            
        }

        protected virtual void OnCollideEnemy(Collider collider)
        {
            
        }

        protected virtual void OnCollidePlayer(Player player, Collider collider)
        {
            
        }

        protected virtual void OnCollideOther(Collider collider)
        {
            
        }
    }
}