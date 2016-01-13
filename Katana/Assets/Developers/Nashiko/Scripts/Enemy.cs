using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
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

        animator.SetInteger(Animator.StringToHash("State"), 1);
        this.transform.Translate(Vector3.forward * Time.deltaTime);

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == TagName.Player)
        {
            var player = collision.gameObject.transform.GetComponent<PlayerController>();
            player.Damage(100);
        }
    }
}