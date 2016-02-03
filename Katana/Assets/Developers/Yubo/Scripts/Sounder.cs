using UnityEngine;
using System.Collections;

public class Sounder : MonoBehaviour {

    void Start()
    {
        SoundManager.I.PlayBGM(SoundKey.BGM_COMBAT);
    }

	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SoundManager.I.PlaySound(this.transform, SoundKey.SE_JUMP01);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            //SoundManager.I.CrossFadeBGM(SoundKey.BGM_STAGE_KUMA_GO, 3f);
        }
	}
}
