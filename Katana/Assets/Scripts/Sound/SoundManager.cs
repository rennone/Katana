using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
/// <summary>
/// OSP wrapper.
/// </summary>
public class SoundManager : Singleton<SoundManager> {

	private static readonly int MAX_TRACK = 20;

	private static readonly string MIXER_GROUP_MASTER = "Master";
	private static readonly string MIXER_GROUP_SE = "SEGroup";
	private static readonly string MIXER_GROUP_BGM = "BGMGroup";
	private static readonly string MIXER_GROUP_VOICE = "VoiceGroup";
	private static readonly string MIXER_GROUP_JINGLE = "JingleGroup";
	private static readonly string MIXER_GROUP_SYSTEM = "SystemGroup";
	private static readonly float SUPPRESSING_TIME = 0.1f;

	//AudioMixersナップショットの定義
	private static readonly string SNAPSHOT_MASTER		   = "Snapshot";
	private static readonly string SNAPSHOT_PLAYING_JINGLE = "Snapshot_PlayingJingle";
	private static readonly string SNAPSHOT_PAUSING = "Snapshot_Pausing";
	private static readonly float  SNAPSHOT_TRANSITION_TIME = 0.5f;

	private AudioMixer audioMixer;
	private Dictionary<SoundType, AudioMixerGroup> AudioMixerGroupSet = new Dictionary<SoundType, AudioMixerGroup> ();
	private Dictionary<string, AudioMixerSnapshot> AudioMixerSnapShotSet = new Dictionary<string, AudioMixerSnapshot> ();

    public bool isStopAll = false;
    [SerializeField]
	private GameObject AudioSourcePrefab;
	[SerializeField]
	private AudioSource[] BGMPoints;    //フェードを行うため、二つ用意する必要がある
    [SerializeField]
    private bool useDoppler;

    private List<SoundData> SoundDataList;
    private List<OSPSourceWrapper> AudioTracks = new List<OSPSourceWrapper>();

    private Dictionary<SoundKey, AudioClip> audioClipSets = new Dictionary<SoundKey, AudioClip>();
	private Dictionary<SoundKey, SoundData> soundDataSets = new Dictionary<SoundKey, SoundData>();

	private static readonly string DATASET_PATH = "Data/SoundList";

	private bool isPause = false;

    private int bgmSourceIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        audioMixer = Resources.Load("Sounds/MainAudioMixer") as AudioMixer;
        AudioMixerGroup[] audioMixerGroups = audioMixer.FindMatchingGroups("/" + MIXER_GROUP_MASTER);
        AudioMixerGroupSet[SoundType.JINGLE] = audioMixerGroups.First(c => c.name == MIXER_GROUP_JINGLE);
        AudioMixerGroupSet[SoundType.SE] = audioMixerGroups.First(c => c.name == MIXER_GROUP_SE);
        AudioMixerGroupSet[SoundType.BGM] = audioMixerGroups.First(c => c.name == MIXER_GROUP_BGM);
        AudioMixerGroupSet[SoundType.VOICE] = audioMixerGroups.First(c => c.name == MIXER_GROUP_VOICE);
        AudioMixerGroupSet[SoundType.SYSTEM] = audioMixerGroups.First(c => c.name == MIXER_GROUP_SYSTEM);

        AudioMixerSnapShotSet[SNAPSHOT_MASTER] = audioMixer.FindSnapshot(SNAPSHOT_MASTER);
        AudioMixerSnapShotSet[SNAPSHOT_PLAYING_JINGLE] = audioMixer.FindSnapshot(SNAPSHOT_PLAYING_JINGLE);
        AudioMixerSnapShotSet[SNAPSHOT_PAUSING] = audioMixer.FindSnapshot(SNAPSHOT_PAUSING);

        for (int index = 0; index < BGMPoints.Length; index++)
        {
            BGMPoints[index].outputAudioMixerGroup = AudioMixerGroupSet[SoundType.BGM];
        }

        SoundDataList = Resources.Load<SoundList>(DATASET_PATH).SoundDatas;
        foreach (SoundData s in SoundDataList)
        {
            if (s.Sound == null || s.SoundDataKey == null)
            {
                continue;
            }

            SoundKey key = (SoundKey)System.Enum.Parse(typeof(SoundKey), s.SoundDataKey);
            soundDataSets.Add(key, s);
            audioClipSets.Add(key, s.Sound);
        }

        for (int index = 0; index < MAX_TRACK; index++)
        {
            GameObject audioInstance = Instantiate(AudioSourcePrefab, this.transform.position, Quaternion.identity) as GameObject;
            audioInstance.transform.parent = this.transform;
            AudioTracks.Add(audioInstance.GetComponent<OSPSourceWrapper>());
        }

        Mute(false);
    }

    void OnLevelWasLoaded(int index) {
        isStopAll = false;
        ResetMixerSnapShot();

        foreach (var bgm in BGMPoints)
        {
            bgm.Stop();
        }
    }

	void Update(){
#if UNITY_EDITOR && OSP_DEBUG
		debugInfo();
#endif

		if (isPause) {
			if (Time.timeScale != 0.0f) {
				PausingSound (false);
				isPause = false;
			}
		} else {
			if( Time.timeScale == 0.0f ) {
				PausingSound (true);
				isPause = true;
			}
		}
	}

	public void Mute(bool flag){
		if (flag) {
			AudioListener.volume = 0.0f;
		} 
		else 
		{
			AudioListener.volume = 1.0f;
		}
	}

	public AudioClip GetClip( SoundKey soundKey ) {
		return audioClipSets[soundKey];
	}

	public OSPSourceWrapper PlaySound(Transform target , string key , float delay = 0, bool loop = false){
		SoundKey soundKey = (SoundKey)System.Enum.Parse (typeof(SoundKey), key);
		OSPSourceWrapper osp = this.PlaySound_impl (target , soundKey , delay, loop);
		return osp;
	}
	
	public OSPSourceWrapper PlaySound(Vector3 targetPosition , string key , float delay = 0, bool loop = false){
		SoundKey soundKey = (SoundKey)System.Enum.Parse (typeof(SoundKey), key);
		OSPSourceWrapper osp = this.PlaySound_impl (targetPosition, soundKey , delay, loop);
		return osp;
	}

	
	public OSPSourceWrapper PlaySound(Transform target , SoundKey key , float delay = 0, bool loop = false){
		OSPSourceWrapper osp = this.PlaySound_impl (target , key , delay, loop);
		return osp;
	}

	public OSPSourceWrapper PlaySound(Vector3 targetPosition , SoundKey key , float delay = 0, bool loop = false){
		OSPSourceWrapper osp = this.PlaySound_impl (targetPosition, key , delay, loop);
		return osp;
	}

	private OSPSourceWrapper PlaySound_impl(System.Object target , SoundKey soundKey , float delay = 0, bool loop = false , bool force = false ){
        //Debug.Log(soundDataSets.Count);
        if (isStopAll) {
            return null;
        }

		SoundData soundData = soundDataSets[soundKey];
		if (soundData == null) {
			Debug.LogError (soundKey + " file not found.");
			return null;
		}

        int playing = 0;
        for ( int index =0; index < AudioTracks.Count; index++)
        {
            if (AudioTracks[index].isPlaying)
            {
                playing++;
            }
        }

		if (playing >= MAX_TRACK) {
			Debug.LogWarning (soundKey + " has exceed to global audio limit");
			return null;
		}

		// 直前に同じ音が鳴っていたら終了(エコー防止)
		if(inSuppressingTime(soundData)){
			Debug.LogWarning( "Suppressing Time exceed.");
			return null;
		}

		int playingCount = AudioTracks.Count (c => c.clip == soundData.Sound && c.isPlaying);

		if (playingCount >= soundData.SyncCount) 
		{
            return null;
		}

		bool isTransform = false;
		Vector3 targetPosition = Vector3.zero;
		Transform targetTransform = null;
		if (target is Vector3) {
			targetPosition = (Vector3)target;
		} else {
			isTransform = true;
			targetTransform = target as Transform;
		}

		OSPSourceWrapper osps = AudioTracks.First( c=> !c.isPlaying );

		AudioSource audio = osps.audioSource;
		osps.volume = soundData.Volume;
		audio.rolloffMode = soundData.RollOff;
		audio.dopplerLevel =  useDoppler ? soundData.DopplerLevel : 0;

		audio.maxDistance = soundData.Distance;
		audio.clip = soundData.Sound;
		audio.loop = loop;
		audio.pitch = soundData.pitch;

		if (soundData.Type == SoundType.JINGLE) {
			osps.setSpatialBlend (0.0f);
            _CheckPlayingJingle(osps);
		} else {
			if (soundData.Type == SoundType.SYSTEM) {
				osps.setSpatialBlend(0.0f);
			}
			else
			{
				osps.setSpatialBlend(1.0f);
			}
		}

		if(isTransform)
		{
			osps.transform.position = targetTransform.position;
			osps.TargetTransform = targetTransform;
		}
		else
		{
			osps.transform.position = targetPosition;
            osps.TargetTransform = null;
		}
		osps.isTrace = isTransform;
		
		AudioMixerGroup amg = AudioMixerGroupSet[soundData.Type];
		audio.outputAudioMixerGroup = amg;
		
		if(audio.loop){
			Debug.Log ("Play Loop Sound : " + soundKey.ToString());
		}
		osps.PlayDelayed (delay);
		return osps;
	}

	private bool mixerReset = false;
    /// <summary>
    /// ジングル再生中には、SnapShot切り替えによるダッキングを行う。
    /// </summary>
    /// <returns>The check playing jingle.</returns>
    /// <param name="audio">Audio.</param>

    void _CheckPlayingJingle(OSPSourceWrapper audio)
    {

        audioMixer.TransitionToSnapshots(
            new AudioMixerSnapshot[] { AudioMixerSnapShotSet[SNAPSHOT_MASTER], AudioMixerSnapShotSet[SNAPSHOT_PLAYING_JINGLE] },
            new float[] { 0, 1 },
            SNAPSHOT_TRANSITION_TIME
        );

        Wait.InvokeAfterSeconds(audio.clip.length, () =>
        {

            if (mixerReset)
            {
                mixerReset = false;
                return;
            }
            else
            {
                audioMixer.TransitionToSnapshots(
                    new AudioMixerSnapshot[] { AudioMixerSnapShotSet[SNAPSHOT_MASTER], AudioMixerSnapShotSet[SNAPSHOT_PLAYING_JINGLE] },
                    new float[] { 1, 0 },
                    SNAPSHOT_TRANSITION_TIME
                );
            }
        });

    }

	/// <summary>
	/// MASTERのMIXERSNAPSHOTにResetをかける
	/// </summary>
	public void ResetMixerSnapShot(){
		audioMixer.TransitionToSnapshots (
			new AudioMixerSnapshot[]{AudioMixerSnapShotSet[SNAPSHOT_MASTER] , AudioMixerSnapShotSet[SNAPSHOT_PLAYING_JINGLE] },
			new float[]{ 1,0},
			SNAPSHOT_TRANSITION_TIME
		);
		mixerReset = true;
	}

	public void PausingSound(bool pause) {
		Debug.Log ("PausingSound:" + pause);
		if (pause) {
			AudioMixerSnapShotSet[SNAPSHOT_PAUSING].TransitionTo( 0.0f );
		} else {
			AudioMixerSnapShotSet[SNAPSHOT_MASTER].TransitionTo( 0.0f );
		}
	}

	public void StopSound( OSPSourceWrapper osps , float fade = 0.3f ){
		if (osps == null) 
		{
			Debug.LogWarning("osps is Null.");
			return;
		}

		if (osps.isPlaying) 
		{
			Debug.Log("is Playing?" + osps.isPlaying);
		}

		Debug.Log ("Stop Sound : " + osps.clip.name);
		
		osps.GetComponent<AudioSource>().DOFade(0.0f, fade).OnComplete(()=>{
			osps.Stop();
		});
	}

	public void PlayBGM( SoundKey soundKey , float fadeTime = 0.5f)
	{
        SoundData soundData = soundDataSets[soundKey];

        foreach (SoundData sd in SoundDataList) {
		//	Debug.Log(soundKey.ToString() + ":" + sd.SoundDataKey);
		}

		if (soundData == null)
		{
			Debug.LogWarning( this.GetInstanceID() + ":" + soundKey.ToString() + " was not found.");
			return;
		}

		float targetVolume = soundData.Volume;
        for (int index = 0; index < BGMPoints.Length; index++)
        {
            DOTween.Kill(BGMPoints[index] );
        }

        BGMPoints[bgmSourceIndex].clip = soundData.Sound;
        BGMPoints[bgmSourceIndex].volume = 0.0f;
        BGMPoints[bgmSourceIndex].Play ();
        BGMPoints[bgmSourceIndex].DOFade (targetVolume, fadeTime);
        BGMPoints[bgmSourceIndex].loop = true;
	}

    public void StopAll(float fade = 1.0f) {
        isStopAll = true;
        for (int index = 0; index < AudioTracks.Count; index++) {
            if (AudioTracks[index].isPlaying)
            {
                AudioTracks[index].Stop(fade);
            }
        }
    }

	public void SetBGMPosition( Vector3 leftPosition , Vector3 rightPosition )
	{
        for (int index = 0; index < BGMPoints.Length; index++)
        {
            BGMPoints[index].transform.position = leftPosition;
        }

	}

    public void CrossFadeBGM(SoundKey soundKey, float fadeTime = 0.5f)
    {
        SoundData soundData = soundDataSets[soundKey];

        float targetVolume = soundData.Volume;
        int oldBgmSourceIndex = bgmSourceIndex;

        bgmSourceIndex++;
        bgmSourceIndex = bgmSourceIndex % 2;

        BGMPoints[bgmSourceIndex].clip = soundData.Sound;
        BGMPoints[bgmSourceIndex].volume = 0.0f;

        BGMPoints[bgmSourceIndex].transform.parent = this.transform;

        BGMPoints[bgmSourceIndex].Play();

        DOTween.Kill(BGMPoints[bgmSourceIndex]);

        DOTween.Kill(BGMPoints[oldBgmSourceIndex]);

        BGMPoints[bgmSourceIndex].DOFade(targetVolume, fadeTime);

        BGMPoints[oldBgmSourceIndex].DOFade(0.0f, fadeTime).OnComplete(() => { BGMPoints[oldBgmSourceIndex].Stop(); });
    }

    private bool isStopping = false;
	public void StopBGM(float fadeTime = 1.0f)
	{
        isStopping = true;
        for (int index = 0; index <2; index++)
        {
            BGMPoints[index].DOFade(0.0f, fadeTime).OnComplete(() => { BGMPoints[index].Pause(); });
        }
	}

	// エコーを起こすような音が鳴っていないかチェックします 
	private bool inSuppressingTime(SoundData data){
		if (data.Sound == null) {
			return true;
		}

		// OSPSourceWrapperで見ている値と合わせる 
		float nowTime = Time.realtimeSinceStartup;
		foreach (OSPSourceWrapper osps in AudioTracks) {
			if(data.Sound.name.Equals( osps.name) ){
				if(nowTime-osps.time < SUPPRESSING_TIME){
					//Debug.Log("nowTime-osps.time="+(nowTime-osps.time).ToString());
					return true;
				}
			}
		}
		return false;
	}

	//--- for debug 
	private void debugInfo(){
		foreach (OSPSourceWrapper osp in AudioTracks) {
			if(osp.clip!=null){
				if(osp.isPlaying){
					osp.name = osp.clip.name;
				}else{
					osp.name = string.Format("__{0}__" , osp.clip.name);
				}
			}else{
				osp.name = "__null__";
			}
		}
	}
	
}
