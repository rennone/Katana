using UnityEngine;
using System.Collections;

public class CannonBullet : MonoBehaviour {

    bool canMove = false;
    Vector3 direction = Vector3.zero;
    float speed = 0;
    Vector3 firstPos = Vector3.zero;
    float expDist = 0;

    public void Fire(Vector3 targetDir, float bulletSpeed,float explosionDistance)
    {
        canMove = true;
        direction = targetDir;
        speed = bulletSpeed;
        firstPos = this.transform.position;
        expDist = explosionDistance;
    }

    void Update()
    {
        if (!canMove)
            return;

        Vector3 newPos = this.transform.position + direction * speed * Time.deltaTime;
        this.transform.position = newPos;
        if((newPos - firstPos).magnitude > expDist)
        {
            this.gameObject.SetActive(false);
        } 
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("MainCharacter"))
        {
            this.gameObject.SetActive(false);
        }
    }
}
