using UnityEngine;
using System.Collections;
using Katana;

public class Cannon : MonoBehaviour {

    public CannonBullet bulletPrefab;
    public Transform bulletPos;
    public float bulletMergin = 2f;
    public float bulletSpeed = 10f;
    public float searchDistance = 20f;
    public float explosionDistance = 50f;

    int cacheCount = 10;

    private bool canFire = true;
    private CannonBullet[] bullets;

    int nowBulletCount = 0;

    void Start()
    {
        bullets = new CannonBullet[cacheCount];
        for(int i = 0; i < cacheCount; i++)
        {
            bullets[i] = Instantiate(bulletPrefab) as CannonBullet;
            bullets[i].gameObject.SetActive(false);
        }
    }

    void Update()
    {
        var target = GameManager.I.Player.transform;
        float dist = (target.position - this.transform.position).magnitude;
        if (dist < searchDistance)
        {
            this.transform.LookAt(target.position);

            if (!canFire)
                return;

            canFire = false;
            CannonBullet bullet = GetBullet();
            bullet.transform.position = bulletPos.position;
            Vector3 targetDir = (target.position - this.transform.position).normalized;
            bullet.Fire(targetDir,bulletSpeed,explosionDistance);
            Invoke("SetCanFire", bulletMergin);
        }
    }

    void SetCanFire()
    {
        canFire = true;
    }

    CannonBullet GetBullet()
    {
        nowBulletCount += 1;
        if (nowBulletCount >= cacheCount)
            nowBulletCount = 0;

        bullets[nowBulletCount].gameObject.SetActive(true);
        return bullets[nowBulletCount];
    }
}
