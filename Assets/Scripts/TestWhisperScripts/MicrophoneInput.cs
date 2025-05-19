using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneInput : MonoBehaviour {

	public static MicrophoneInput instance { get; private set; }

	public bool playOnStart = true;

	public AudioSource audioSource;
	private bool isRecording = false;
	private string microphoneName;

	
	void Start() {
		audioSource = GetComponent<AudioSource>();

		if (Microphone.devices.Length > 0) {
			microphoneName = Microphone.devices[0]; // Use the default microphone
			if (playOnStart) {
				StartRecording();
			}
		} else {
			Debug.Log("No microphone found!");
		}
	}

	public void StartRecording() {
		if (!isRecording) {
			// Record for a set amount of time (e.g., 5 seconds)
			audioSource.clip = Microphone.Start(microphoneName, true, 5, 44100);
			audioSource.Play();
			isRecording = true;
			Debug.Log("Recording started!");
		}
	}

	public void StopRecording() {
		if (isRecording) {
			Microphone.End(microphoneName); // Stop recording
			audioSource.Stop();
			isRecording = false;
			Debug.Log("Recording stopped!");
		}
	}
}