using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
	[SerializeField] List<AudioClip> stealthClips;
	[SerializeField] List<AudioClip> detectedClips;
	[SerializeField] AudioMixer audioMixer;

	[SerializeField] AudioSource playerAudio;
	private int stealthIndex = 0;
	private int detectedIndex = 0;

	public bool playerDetected;
	public bool PlayerDetected
	{
		get { return playerDetected; }
		set
		{
			if (playerDetected != value)
			{
				playerDetected = value;
				if (value)
				{
					//PlayDetected();
				}
				else
				{
					//PlayStealth();
				}
			}
		}
	}

	private void OnEnable()
	{
		DetectionLevel.onFull += PlayDetected;
		//DetectionLevel.onEmpty += PlayStealth;
	}

	private void OnDisable()
	{
		DetectionLevel.onFull -= PlayDetected;
		DetectionLevel.onEmpty -= PlayStealth;
	}

	void Start()
	{
		detectedIndex = Random.Range(0,detectedClips.Count);
		stealthIndex = Random.Range(0,stealthClips.Count);
	}

	void Update()
	{
		if (!playerAudio.isPlaying)
		{
			if (PlayerDetected)
			{
				PlayDetected();
				detectedIndex = (int)Mathf.Repeat(detectedIndex + 1, detectedClips.Count - 1);
			}
			else
			{
				PlayStealth();
				stealthIndex = (int)Mathf.Repeat(stealthIndex + 1, stealthClips.Count - 1);
			}
		}
	}

	void PlayStealth()
	{
		stealthIndex = Random.Range(0, stealthClips.Count);
		if (playerAudio.clip == stealthClips[stealthIndex] && playerAudio.isPlaying) return;
		DetectionLevel.onFull += PlayDetected;
		DetectionLevel.onEmpty -= PlayStealth;
		//playerAudio.Stop();
		playerAudio.clip = stealthClips[stealthIndex];
		playerAudio.Play();
	}

	void PlayDetected()
	{
		if (playerAudio.clip == detectedClips[detectedIndex] && playerAudio.isPlaying) return;
		DetectionLevel.onFull -= PlayDetected;
		DetectionLevel.onEmpty += PlayStealth;
		//playerAudio.Stop();
		playerAudio.clip = detectedClips[detectedIndex];
		playerAudio.Play();
	}
}
