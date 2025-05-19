using iamai_core_lib;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WhisperAiManager : MonoBehaviour
{
	public static WhisperAiManager Instance { get; private set; }

	public List<string> WhisperModelList;

	private iamai_core_lib.AI ai;

	bool recording = false;

	private void Awake() {
		Instance = this;
		ai = new iamai_core_lib.AI(WhisperModelList[0], 8);
	}

	private async void Start() {
		try {
			await ai.Activate();
		} catch (Exception e) {
			Debug.LogError(e);
		}
	}

	public void onClick() {
		if (recording) {
			recording = false;
			MicrophoneInput.instance.StopRecording();
			//change audio source clip to pcm32
			// await ai. transcribe
			//set output text to transcribed text
		} else {
			recording = true;
			MicrophoneInput.instance.StartRecording();
		}
	}

}
