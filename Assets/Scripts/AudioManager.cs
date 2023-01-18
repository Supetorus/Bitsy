using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	[SerializeField] List<AudioClip> stealthClips;
	[SerializeField] List<AudioClip> detectedClips;
	[SerializeField] AudioMixer audioMixer;

	[SerializeField] AudioSource playerAudio;
	private int stealthIndex;
	private int detectedIndex;

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
					PlayDetected();
				}
				else
				{
					PlayStealth();
				}
			}
		}
	}


	void Start()
	{

	}

	void Update()
	{
		if (!playerAudio.isPlaying)
		{
			if (PlayerDetected)
			{
				detectedIndex = detectedIndex++ % detectedClips.Count;
				PlayDetected();
			}
			else
			{
				stealthIndex = stealthIndex++ % stealthClips.Count;
				PlayStealth();
			}
		}
	}

	void PlayStealth()
	{
		playerAudio.Stop();
		playerAudio.clip = stealthClips[stealthIndex];
		playerAudio.Play();
	}

	void PlayDetected()
	{
		playerAudio.Stop();
		playerAudio.clip = detectedClips[detectedIndex];
		playerAudio.Play();
	}
}
