using UnityEngine;
using System.Collections;

public class PlayerStartPosition : MonoBehaviour {

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    void Start()
    {
        if(Door.loadDoorID == -1)
        {
            Katana.GameManager.Instance.Player.SetPlayerPosition(this.transform.position);
        }
    }
}
