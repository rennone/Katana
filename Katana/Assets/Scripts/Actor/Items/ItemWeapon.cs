using UnityEngine;
using System.Collections;
using Katana.Hud;

namespace Katana
{
    public class ItemWeapon : MonoBehaviour
    {
        void OnTriggerEnter(Collider collider)
        {
            if (collider.tag != TagName.Player)
                return;
            Debug.Log("Collid Player");
            var player = collider.GetComponent<Player>();
            player.ReleaseAttack(); //武器開放
            Hud.HudManager.AddMessage(new string[]{
                "release kick",
                "left click"
            }
            , kind: HudMessage.MessageKind.Hint);
            Destroy(this.gameObject);
        }

        void Update()
        {
            var rotSpeed = 100;
            transform.Rotate(Vector3.forward, Time.deltaTime * rotSpeed);
        }
    }
}