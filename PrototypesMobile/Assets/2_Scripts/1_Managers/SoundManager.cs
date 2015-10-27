namespace TheVandals
{
	using UnityEngine;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	
	public class SoundManager : Singleton<SoundManager>
	{
		private AudioSource[] audioSource;
		[SerializeField]
		private AudioClip[] audioClip;

		#region Events
		void OnEnable()
		{
			EventManager.initialise += Init;
		}		
		
		void OnDisable()
		{
			EventManager.initialise -= Init;
		}
		
		private void Init()
		{			
			audioSource = GetComponents<AudioSource>();
			audioSource[0].clip = audioClip[0];
			audioSource[1].clip = audioClip[1];
			audioSource[0].Play();
		}
		#endregion

		#region Public

		public void PlayClip(int index)
		{
			audioSource[index].Play();
		}

		#endregion
	}
}
