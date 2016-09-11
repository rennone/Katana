using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class DefenceDoor : GimmickBase
{
    const float DOOR_OPEN_TIME = 0.5f;

    [SerializeField]
    Transform door1;
    [SerializeField]
    Transform door2;

    void OnTriggerEnter(Collider collider)
    {
        bool isCollisionEnd = false;

        if (isCollisionEnd) return;

        if (collider.tag != TagName.Player)
            return;

        var player = collider.GetComponent<Katana.Player>();

        //持ち物を取得
        bool haveKey = player.RemoveItem(Katana.ItemKind.DoorKey);
        if (!haveKey)
        {
            return;
        }

        // コライダーを消す
        this.GetComponent<BoxCollider>().enabled = false;
        door1.DOLocalRotate(Vector3.down * 90,DOOR_OPEN_TIME);
        door2.DOLocalRotate(Vector3.up * 90, DOOR_OPEN_TIME);

        isCollisionEnd = true;
    }
}
