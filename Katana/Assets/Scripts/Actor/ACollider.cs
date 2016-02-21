using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Katana
{
    public class ACollider : AMonoBehaviour
    {

        public enum CollisionTarget
        {
            Player,
            Enemy,
            Gimmick,
            Else
        }

        [SerializeField]
        public List<CollisionTarget> ignores;

        bool CollidableTo(CollisionTarget mask)
        {
            return ignores.Contains(mask) == false;
        }

        protected virtual bool CollidableTo(Collider collider)
        {
            switch (collider.tag)
            {
                case TagName.Player:
                    return CollidableTo(CollisionTarget.Player);
                case TagName.Enemy:
                    return CollidableTo(CollisionTarget.Enemy);
                case TagName.Gimmick:
                    return CollidableTo(CollisionTarget.Gimmick);
                default:
                    return CollidableTo(CollisionTarget.Else);
            }
        }

        protected virtual void OnTriggerEnter(Collider c)
        {
            if (CollidableTo(c) == false)
                return;
            OnTriggerEnterWith(c);
        }

        protected virtual void OnTriggerEnterWith(Collider c)
        {
            
        }
    }
}