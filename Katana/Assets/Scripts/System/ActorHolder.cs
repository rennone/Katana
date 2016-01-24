using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActorHolder : MonoBehaviour
{

    // このラインを越えたら強制的に破壊される高さ
    [SerializeField] 
    private int borderHeightToDeath = -200;


    public PlayerController Player { get; private set; }

    // アクターのインスタンスリスト
    // key   : tagName
    // value : Actorリスト 
    private Dictionary<string, HashSet<ActorController>> actorList_ = new Dictionary<string, HashSet<ActorController>>();

    void SetPlayer(PlayerController player)
    {
        if(Player != null)
            Destroy(Player);
        Player = player;
    }


    public void RegisterActor(ActorController actor)
    {
        if(!actorList_.ContainsKey(actor.tag))
            actorList_[actor.tag] = new HashSet<ActorController>();

        actorList_[actor.tag].Add(actor);

        // Playerだけは特別に保存しておく(アクセスが多いので)
        if(actor.tag == TagName.Player)
            SetPlayer(actor.gameObject.GetComponent<PlayerController>());
    }

    public void RemoveActor(ActorController actor)
    {
        if(!actorList_.ContainsKey(actor.tag))
            return;

        actorList_[actor.tag].Remove(actor);
    }
	
	// Update is called once per frame
	void Update () 
    {
	    foreach (var actorSet in actorList_)
	    {
	        foreach (var actor in actorSet.Value)
	        {
	            if(actor.transform.position.y <= borderHeightToDeath)
                    Destroy(actor.gameObject);
	        }
	    }
	}
}
