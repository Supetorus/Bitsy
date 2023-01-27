using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{
	[SerializeField] AudioSource audioSource;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

	public void Play()
	{
		if (!audioSource.isPlaying) audioSource.Play();
	}
}
