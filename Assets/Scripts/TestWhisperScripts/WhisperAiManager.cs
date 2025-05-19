using iamai_core_lib;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WhisperAiManager : MonoBehaviour
{
	public static WhisperAiManager Instance { get; private set; }
	public TextMeshProUGUI outputText;
	public List<string> WhisperModelList;

	private iamai_core_lib.AI ai;
	public float[] pcmData;
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

	public async void onClick() {
		if (recording) {
			recording = false;
			MicrophoneInput.instance.StopRecording();
			//change audio source clip to pcm32
			AudioClip clip = MicrophoneInput.instance.audioSource.clip;
			int samples = clip.samples;
			int channels = clip.channels;
			pcmData = new float[samples* channels];
			clip.GetData(pcmData, 0);
			//await ai. transcribe
			string output = await ai.WhisperAsyncTranscribe(pcmData, samples);
			//set output text to transcribed text
			outputText.text = output;
		} else {
			recording = true;
			MicrophoneInput.instance.StartRecording();
		}
	}

}
