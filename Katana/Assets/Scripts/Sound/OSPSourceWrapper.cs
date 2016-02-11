using UnityEngine;
using System.Collections;
using DG.Tweening;
public class OSPSourceWrapper : MonoBehaviour {

	[SerializeField]
	private Transform _targetTransform;
	public Transform TargetTransform{
		get{ return _targetTransform;}
		set{ 
			_targetTransform = value;
		}
	}

	private float _volume;
	public  float volume{
		get{ return _volume;}
		set{ 
			_audioSource.volume = value;
			_volume = value;
		}
	}

	private float _time;
	public  float time{
		get{ return _time;}
	}

	[SerializeField]
	private AudioSource _audioSource;

	// Use this for initialization
	void Update() {
		//トレースする対象のTransformがある場合は、対象が消えるor Deactiveでサウンドを停止する
		if (_isTrace) {
			if (_targetTransform == null || !_targetTransform.gameObject.activeSelf) 
			{
				_audioSource.Stop();
			}
			else
			{
				this.transform.position = _targetTransform.position;
			}
		}
	}

	public bool isPlaying
	{
		get{ return _audioSource.isPlaying;}
	}

	public bool isPlayingClip(SoundKey soundKey) {
		if (_audioSource.isPlaying)
		{
			if (_audioSource.clip == SoundManager.Instance.GetClip(soundKey)) {
				return true;
			}
		}
		return false;
	}

	private bool _isTrace;
	public bool isTrace
	{
		get{ return _isTrace;}
		set{ _isTrace = value;}
	}

	public AudioClip clip
	{
		get{ return _audioSource.clip; }
	}

	public AudioSource audioSource
	{
		get{ return _audioSource; }
	}

	public void Stop(){
		_audioSource.loop = false;
		_audioSource.Stop ();
	}

    public void Stop(float fade ) {
        _audioSource.loop = false;
        _audioSource.DOFade(0.0f, fade).OnComplete(() =>
        {
            _audioSource.Stop();
        });
    }


	public void Play(){
        _audioSource.Play();
	}

	public void PlayDelayed(float delay){
		_time = Time.realtimeSinceStartup;
		_audioSource.PlayDelayed (delay);
	}

	/// <summary>
	/// Sets the spatial blend.
	/// 0.0 makes the sound full 2D, 1.0 makes it full 3D.
	/// </summary>
	/// <param name="blend">Blend.</param>
	public void setSpatialBlend(float blend) {
		_audioSource.spatialBlend = blend;
	}

	/// <summary>
	/// Pitchの値を途中から変更する。
	/// </summary>
	public float pitch
	{
        get{ return _audioSource.pitch; }
		set { _audioSource.pitch = value; }
	}

}
