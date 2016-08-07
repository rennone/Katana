using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Door : GimmickBase {

    private const string AREA_PREFIX = "Area";
    private const string AREA_NAME_SEPARATER = "-";

    public static int loadDoorID = -1;

    [SerializeField]
    int thisDoorID = 0;

    [SerializeField]
    int stageNumber = 0;

    [SerializeField]
    int nextAreaNumber = 0;

    [SerializeField]
    int nextDoorID = 0;

    bool isClose = false;

	public override void Start () {
        base.Start();

        if(loadDoorID == thisDoorID)
        {
            Katana.GameManager.Instance.Player.SetPlayerPosition(this.transform.position);
        }

        //loadDoorID = -1;
	}
	
	public override void Update () {
        base.Update();

	}

    public void Open()
    {
        if (isClose) return;

        loadDoorID = nextDoorID;
        //10未満の場合は0を前に付ける
        string stage = (stageNumber - 10) >= 0 ? stageNumber.ToString() : "0" + stageNumber.ToString();
        string number = (nextAreaNumber - 10) >= 0 ? nextAreaNumber.ToString() : "0" + nextAreaNumber.ToString();
        InputManager.Instance.CanInput = false;
        StartCoroutine(ChangeScene(stage, number));
    }

    IEnumerator ChangeScene(string stage,string number)
    {
        yield return null;
        SceneManager.LoadScene(AREA_PREFIX + stage + AREA_NAME_SEPARATER + number, LoadSceneMode.Single);
    }
}
