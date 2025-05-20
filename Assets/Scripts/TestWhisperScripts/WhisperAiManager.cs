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
	private AudioSource audioPlayer;
	private iamai_core_lib.AI ai;
	private float[] pcmData;
	private int samples;
	bool recording = false;

	private void Awake() {
		Instance = this;
		ai = new iamai_core_lib.AI(WhisperModelList[0], 8);
		
	}

	private async void Start() {
		try {
			await ai.Activate();
			Debug.Log("Ai Activated");
		} catch (Exception e) {
			Debug.LogError(e);
		}
		audioPlayer = GetComponent<AudioSource>();
	}

	public void onClick() {
		if (recording) {
			recording = false;
			AudioClip clip = MicrophoneInput.instance.audioSource.clip;
			MicrophoneInput.instance.StopRecording();
			//change audio source clip to pcm32
			samples = clip.samples;
			int channels = clip.channels;
			pcmData = new float[samples*channels];
			clip.GetData(pcmData, 0);
			//await ai. transcribe
			pcmData = convertToMonoChannel(pcmData, channels);
		} else {
			recording = true;
			MicrophoneInput.instance.StartRecording();
		}
	}

	public float[] convertToMonoChannel(float[] currentPCMData, int channels) {// Downmix to mono if channels > 1
		if (channels > 1) {
			float[] monoData = new float[samples];
			for (int i = 0; i < samples; i++) {
				float sum = 0f;
				for (int c = 0; c < channels; c++) {
					sum += pcmData[i * channels + c];
				}
				monoData[i] = sum / channels; // average channels
			}
			return monoData;
		}
		return currentPCMData;
	}

	public async void Transcribe() {
		string output = await ai.WhisperAsyncTranscribe(pcmData, samples);
		//set output text to transcribed text
		Debug.Log(pcmData[0]);
		Debug.Log(output);
		outputText.text = output;
	}

	public void playClip() {
		audioPlayer.clip = MicrophoneInput.instance.audioSource.clip;
		audioPlayer.Play();
	}
}