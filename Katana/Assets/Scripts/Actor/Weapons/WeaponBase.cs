using UnityEngine;
using System.Collections;

public class WeaponBase : Katana.AMonoBehaviour 
{
    [SerializeField]
    Collider _collider;     //あたり判定用コライダ

	// Use this for initialization
	protected override sealed void Awake () 
    {
        _collider.isTrigger = true;

        //最初は武器を切る
        SetActive(false);
	}
	
	// Update is called once per frame
    protected override sealed void Update()
    {

	}

    public virtual void SetActive(bool enable)
    {
        _collider.enabled = enable;
        enabled = enable;
    }

    public virtual void SetVisible(bool enable)
    {
    }

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Weapon Collide");
    }
}
