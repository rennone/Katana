using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugScrollController : MonoBehaviour 
{    
    [SerializeField]
	public RectTransform prefab = null;

	void Start () 
	{
		for(int i=0; i<0; i++)
		{
			var item = GameObject.Instantiate(prefab) as RectTransform;
			item.SetParent(transform, false);

			var text = item.GetComponentInChildren<Text>();
			text.text = "item:" + i.ToString();
		}
	}
}