using UnityEngine;
using System.Collections;

public class BossEnemy : MonoBehaviour
{
    private Animator animator;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.Find("MainCharacter");
        var diff = player.transform.position - this.transform.position;

        if (diff.magnitude > 0.01) {
            this.transform.rotation = Quaternion.LookRotation(diff);
        }

        animator.SetInteger(Animator.StringToHash("State"), 4);
        this.transform.Translate(Vector3.forward * Time.deltaTime);

    }

    void OnCollisionEnter(Collision collision)
    {
        collision.other.SendMessage("DecreaseHP" , 100);
    }
}