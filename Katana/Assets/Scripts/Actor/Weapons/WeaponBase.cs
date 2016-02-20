using UnityEngine;
using System.Collections;
using Katana.Messages;

namespace Katana
{
    public class WeaponBase : ACollider
    {
        [SerializeField] 
        private Collider _collider; //あたり判定用コライダ

        [SerializeField] 
        private Character _owner;   //この武器の所有者

        protected Character Owner { get { return _owner; } }

        [SerializeField]
        public WeaponParameter _parameter;

        [System.Serializable]
        public struct WeaponParameter
        {
            public int Strong;
        }

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

        protected override bool CollidableTo(Collider collider)
        {
            return base.CollidableTo(collider) && collider.gameObject != Owner.gameObject;
        }

        protected override void OnTriggerEnterWith(Collider c)
        {
            base.OnTriggerEnterWith(c);
            Attack(c);
        }


        protected virtual void Attack(Collider collider)
        {
            Debug.Log("Collide");
            var target = collider.GetComponent<IDamage>();
            if (target != null)
                Owner.Attack(target, new Damage(_parameter.Strong, collider, this));
        }
    }
}