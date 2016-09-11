using UnityEngine;
using System.Collections;

public class Sounder : MonoBehaviour {

    void Start()
    {
        SoundManager.Instance.PlayBgm(SoundKey.ESPIONAGE_MOVING);
    }

	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SoundManager.Instance.PlaySound(this.transform, SoundKey.SE_JUMP01);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            //SoundManager.I.CrossFadeBGM(SoundKey.BGM_STAGE_KUMA_GO, 3f);
        }
	}
}
