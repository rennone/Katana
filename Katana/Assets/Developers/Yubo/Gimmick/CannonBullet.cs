using UnityEngine;
using System.Collections;

public class CannonBullet : MonoBehaviour {

    bool canMove = false;
    Vector3 direction = Vector3.zero;
    float speed = 0;

    public void Fire(Vector3 targetDir, float bulletSpeed)
    {
        canMove = true;
        direction = targetDir;
        speed = bulletSpeed;
    }

    void Update()
    {
        if (!canMove)
            return;

        Vector3 newPos = this.transform.position + direction * speed * Time.deltaTime;
        this.transform.position = newPos;
    }
}
