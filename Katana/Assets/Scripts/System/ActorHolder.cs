using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActorHolder : MonoBehaviour
{

    // このラインを越えたら強制的に破壊される高さ
    [SerializeField] 
    private int borderHeightToDeath = -1000;


    // アクターのインスタンスリスト
    // key   : tagName
    // value : Actorリスト 
    private Dictionary<string, HashSet<IActor>> actorList_ = new Dictionary<string, HashSet<IActor>>();


    public void RegisterActor(IActor actor)
    {
        if(!actorList_.ContainsKey(actor.tag))
            actorList_[actor.tag] = new HashSet<IActor>();

        actorList_[actor.tag].Add(actor);
    }

    public void RemoveActor(IActor actor)
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
