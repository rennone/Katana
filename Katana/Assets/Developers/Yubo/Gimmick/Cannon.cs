using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour {

    public CannonBullet bulletPrefab;
    public Transform bulletPos;
    public float bulletMergin = 2f;
    public float bulletSpeed = 10f;
    public float searchDistance = 20f;

    [SerializeField]
    private Transform target;

    private bool canFire = true;

    void Start()
    {

    }

    void Update()
    {
        float dist = (target.position - this.transform.position).magnitude;
        if (dist < searchDistance)
        {
            this.transform.LookAt(target.position);

            if (!canFire)
                return;

            canFire = false;
            CannonBullet bullet = Instantiate(bulletPrefab, bulletPos.position, Quaternion.identity) as CannonBullet;
            Vector3 targetDir = (target.position - this.transform.position).normalized;
            bullet.Fire(targetDir,bulletSpeed);
            Invoke("SetCanFire", bulletMergin);
        }
    }

    void SetCanFire()
    {
        canFire = true;
    }
}
