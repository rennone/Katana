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
            _collider.isTrigger = true;
            //最初は武器を切る
            SetActive(false);

            OnInitialize();
        }

        // Update is called once per frame
        protected override sealed void Update()
        {
            OnUpdate();
        }
        

        public virtual void SetActive(bool enable)
        {
            _collider.enabled = enable;
            enabled = enable;
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
            }
        }

        protected virtual void OnCollideGimmick(GimmickBase gimmick, Collider collider)
        {
            
        }

        protected virtual void OnCollideEnemy()
        {
            
        }

        protected virtual void OnCollideOther(Collider collider)
        {
            
        }
    }
}