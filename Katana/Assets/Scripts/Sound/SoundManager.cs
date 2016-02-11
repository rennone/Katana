using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

/// <summary>
/// OSP wrapper.
/// </summary>
public class SoundManager : Katana.Singleton<SoundManager>
{

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

	private AudioMixer _audioMixer;
	private readonly Dictionary<SoundType, AudioMixerGroup> _audioMixerGroupSet = new Dictionary<SoundType, AudioMixerGroup> ();
	private readonly Dictionary<string, AudioMixerSnapshot> _audioMixerSnapShotSet = new Dictionary<string, AudioMixerSnapshot> ();

    public bool IsStopAll;
    [SerializeField]
	private GameObject _audioSourcePrefab;
	[SerializeField]
	private AudioSource[] _bgmPoints;    //フェードを行うため、二つ用意する必要がある
    [SerializeField]
    private bool _useDoppler;

    private List<SoundData> SoundDataList;
    private readonly List<OSPSourceWrapper> _audioTracks = new List<OSPSourceWrapper>();

    private readonly Dictionary<SoundKey, AudioClip> _audioClipSets = new Dictionary<SoundKey, AudioClip>();
	private readonly Dictionary<SoundKey, SoundData> _soundDataSets = new Dictionary<SoundKey, SoundData>();

	private static readonly string DATASET_PATH = "Data/SoundList";

	private bool _isPause = false;

    private int _bgmSourceIndex = 0;

    protected override void OnInitialize()
    {
        _audioMixer = Resources.Load("Sounds/MainAudioMixer") as AudioMixer;
        AudioMixerGroup[] audioMixerGroups = _audioMixer.FindMatchingGroups("/" + MIXER_GROUP_MASTER);
        _audioMixerGroupSet[SoundType.JINGLE] = audioMixerGroups.First(c => c.name == MIXER_GROUP_JINGLE);
        _audioMixerGroupSet[SoundType.SE] = audioMixerGroups.First(c => c.name == MIXER_GROUP_SE);
        _audioMixerGroupSet[SoundType.BGM] = audioMixerGroups.First(c => c.name == MIXER_GROUP_BGM);
        _audioMixerGroupSet[SoundType.VOICE] = audioMixerGroups.First(c => c.name == MIXER_GROUP_VOICE);
        _audioMixerGroupSet[SoundType.SYSTEM] = audioMixerGroups.First(c => c.name == MIXER_GROUP_SYSTEM);

        _audioMixerSnapShotSet[SNAPSHOT_MASTER] = _audioMixer.FindSnapshot(SNAPSHOT_MASTER);
        _audioMixerSnapShotSet[SNAPSHOT_PLAYING_JINGLE] = _audioMixer.FindSnapshot(SNAPSHOT_PLAYING_JINGLE);
        _audioMixerSnapShotSet[SNAPSHOT_PAUSING] = _audioMixer.FindSnapshot(SNAPSHOT_PAUSING);

        for (int index = 0; index < _bgmPoints.Length; index++)
        {
            _bgmPoints[index].outputAudioMixerGroup = _audioMixerGroupSet[SoundType.BGM];
        }

        SoundDataList = Resources.Load<SoundList>(DATASET_PATH).SoundDatas;
        foreach (SoundData s in SoundDataList)
        {
            if (s.Sound == null || s.SoundDataKey == null)
            {
                continue;
            }

            SoundKey key = (SoundKey)System.Enum.Parse(typeof(SoundKey), s.SoundDataKey);
            _soundDataSets.Add(key, s);
            _audioClipSets.Add(key, s.Sound);
        }

        for (int index = 0; index < MAX_TRACK; index++)
        {
            GameObject audioInstance = Instantiate(_audioSourcePrefab, this.transform.position, Quaternion.identity) as GameObject;
            audioInstance.transform.parent = this.transform;
            _audioTracks.Add(audioInstance.GetComponent<OSPSourceWrapper>());
        }

        Mute(false);
    }

    void OnLevelWasLoaded(int index) {
        IsStopAll = false;
        ResetMixerSnapShot();

        foreach (var bgm in _bgmPoints)
        {
            bgm.Stop();
        }
    }

	void Update(){
#if UNITY_EDITOR && OSP_DEBUG
		debugInfo();
#endif

		if (_isPause) {
			if (Time.timeScale != 0.0f) {
				PausingSound (false);
				_isPause = false;
			}
		} else {
			if( Time.timeScale == 0.0f ) {
				PausingSound (true);
				_isPause = true;
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
		return _audioClipSets[soundKey];
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
        if (IsStopAll) {
            return null;
        }

		SoundData soundData = _soundDataSets[soundKey];
		if (soundData == null) {
			Debug.LogError (soundKey + " file not found.");
			return null;
		}

        int playing = 0;
        for ( int index =0; index < _audioTracks.Count; index++)
        {
            if (_audioTracks[index].isPlaying)
            {
                playing++;
            }
        }

		if (playing >= MAX_TRACK) {
			Debug.LogWarning (soundKey + " has exceed to global audio limit");
			return null;
		}

		// 直前に同じ音が鳴っていたら終了(エコー防止)
		if(InSuppressingTime(soundData)){
			Debug.LogWarning( "Suppressing Time exceed.");
			return null;
		}

		int playingCount = _audioTracks.Count (c => c.clip == soundData.Sound && c.isPlaying);

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

		OSPSourceWrapper osps = _audioTracks.First( c=> !c.isPlaying );

		AudioSource audio = osps.audioSource;
		osps.volume = soundData.Volume;
		audio.rolloffMode = soundData.RollOff;
		audio.dopplerLevel =  _useDoppler ? soundData.DopplerLevel : 0;

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
		
		AudioMixerGroup amg = _audioMixerGroupSet[soundData.Type];
		audio.outputAudioMixerGroup = amg;
		
		if(audio.loop){
			Debug.Log ("Play Loop Sound : " + soundKey.ToString());
		}
		osps.PlayDelayed (delay);
		return osps;
	}

	private bool _mixerReset = false;
    /// <summary>
    /// ジングル再生中には、SnapShot切り替えによるダッキングを行う。
    /// </summary>
    /// <returns>The check playing jingle.</returns>
    /// <param name="source">Audio.</param>

    void _CheckPlayingJingle(OSPSourceWrapper source)
    {

        _audioMixer.TransitionToSnapshots(
            new AudioMixerSnapshot[] { _audioMixerSnapShotSet[SNAPSHOT_MASTER], _audioMixerSnapShotSet[SNAPSHOT_PLAYING_JINGLE] },
            new float[] { 0, 1 },
            SNAPSHOT_TRANSITION_TIME
        );

        Wait.InvokeAfterSeconds(source.clip.length, () =>
        {

            if (_mixerReset)
            {
                _mixerReset = false;
                return;
            }
            else
            {
                _audioMixer.TransitionToSnapshots(
                    new AudioMixerSnapshot[] { _audioMixerSnapShotSet[SNAPSHOT_MASTER], _audioMixerSnapShotSet[SNAPSHOT_PLAYING_JINGLE] },
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
		_audioMixer.TransitionToSnapshots (
			new AudioMixerSnapshot[]{_audioMixerSnapShotSet[SNAPSHOT_MASTER] , _audioMixerSnapShotSet[SNAPSHOT_PLAYING_JINGLE] },
			new float[]{ 1,0},
			SNAPSHOT_TRANSITION_TIME
		);
		_mixerReset = true;
	}

	public void PausingSound(bool pause) {
		Debug.Log ("PausingSound:" + pause);
		if (pause) {
			_audioMixerSnapShotSet[SNAPSHOT_PAUSING].TransitionTo( 0.0f );
		} else {
			_audioMixerSnapShotSet[SNAPSHOT_MASTER].TransitionTo( 0.0f );
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
		
		osps.GetComponent<AudioSource>().DOFade(0.0f, fade).OnComplete(osps.Stop);
	}

	public void PlayBgm( SoundKey soundKey , float fadeTime = 0.5f)
	{
        SoundData soundData = _soundDataSets[soundKey];

        //foreach (SoundData sd in SoundDataList) {
		//	Debug.Log(soundKey.ToString() + ":" + sd.SoundDataKey);
		//}

		if (soundData == null)
		{
			Debug.LogWarning( this.GetInstanceID() + ":" + soundKey.ToString() + " was not found.");
			return;
		}

		float targetVolume = soundData.Volume;
        for (int index = 0; index < _bgmPoints.Length; index++)
        {
            DOTween.Kill(_bgmPoints[index] );
        }

	    _bgmPoints[_bgmSourceIndex].spatialBlend = 0.0f;  // bgmは2D音源(発信源に依存しない)
        _bgmPoints[_bgmSourceIndex].clip = soundData.Sound;
        _bgmPoints[_bgmSourceIndex].volume = 0.0f;
        _bgmPoints[_bgmSourceIndex].Play ();
        _bgmPoints[_bgmSourceIndex].DOFade (targetVolume, fadeTime);
        _bgmPoints[_bgmSourceIndex].loop = true;
	}

    public void StopAll(float fade = 1.0f) {
        IsStopAll = true;
        for (int index = 0; index < _audioTracks.Count; index++) {
            if (_audioTracks[index].isPlaying)
            {
                _audioTracks[index].Stop(fade);
            }
        }
    }

    //public void SetBGMPosition( Vector3 leftPosition , Vector3 rightPosition )
    //{
    //    for (int index = 0; index < BGMPoints.Length; index++)
    //    {
    //        BGMPoints[index].transform.position = leftPosition;
    //    }
    //}

    public void CrossFadeBgm(SoundKey soundKey, float fadeTime = 0.5f)
    {
        SoundData soundData = _soundDataSets[soundKey];

        float targetVolume = soundData.Volume;
        int oldBgmSourceIndex = _bgmSourceIndex;

        _bgmSourceIndex++;
        _bgmSourceIndex = _bgmSourceIndex % 2;

        _bgmPoints[_bgmSourceIndex].clip = soundData.Sound;
        _bgmPoints[_bgmSourceIndex].volume = 0.0f;

        _bgmPoints[_bgmSourceIndex].transform.parent = this.transform;

        _bgmPoints[_bgmSourceIndex].Play();

        DOTween.Kill(_bgmPoints[_bgmSourceIndex]);

        DOTween.Kill(_bgmPoints[oldBgmSourceIndex]);

        _bgmPoints[_bgmSourceIndex].DOFade(targetVolume, fadeTime);

        _bgmPoints[oldBgmSourceIndex].DOFade(0.0f, fadeTime).OnComplete(() => { _bgmPoints[oldBgmSourceIndex].Stop(); });
    }

    private bool _isStopping;
	public void StopBgm(float fadeTime = 1.0f)
	{
        _isStopping = true;
        for (int index = 0; index <2; index++)
        {
            _bgmPoints[index].DOFade(0.0f, fadeTime).OnComplete(() => { _bgmPoints[index].Pause(); });
        }
	}

	// エコーを起こすような音が鳴っていないかチェックします 
	private bool InSuppressingTime(SoundData data){
		if (data.Sound == null) {
			return true;
		}

		// OSPSourceWrapperで見ている値と合わせる 
		float nowTime = Time.realtimeSinceStartup;
		foreach (OSPSourceWrapper osps in _audioTracks) {
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
	private void DebugInfo(){
		foreach (OSPSourceWrapper osp in _audioTracks) {
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
