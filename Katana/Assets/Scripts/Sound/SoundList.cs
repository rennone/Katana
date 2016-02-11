using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundList : ScriptableObject {
    public List<SoundData> SoundDatas = new List<SoundData>();

}

public enum SoundType{
	NONE,
	SE ,
	BGM ,
	JINGLE ,
	VOICE,
	SYSTEM,
}

[System.Serializable]
public class SoundData
{
	public string			SoundDataKey;		//SoundKey
	public AudioClip 	  	Sound;				//AudioClip
    [Tooltip("AudioMixerのグループ分け")]
    public SoundType		Type;
    [Tooltip("同時発音数制限")]
	public int				SyncCount = 4;		//同時発音数制限
    [Range(0f,1f), Tooltip("音量")]
	public float 			Volume;				//AudioのVolumeレベル(0-1)
    [Tooltip("音が届く距離")]
    public float			Distance = 20;		//オーディオの発音距離レベル
    [Tooltip("フェードする速さ")]
    public AudioRolloffMode RollOff;			//発音の可聴領域の対応
    [Tooltip("ドップラー効果の強さ")]
    public float			DopplerLevel =1;    //ドップラーレベル
    [Range(0.1f, 3f), Tooltip("音の再生速度")]
    public float            pitch = 1;      //ピッチ
}