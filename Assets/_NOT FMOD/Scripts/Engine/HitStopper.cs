using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class HitStopper : MonoBehaviour {

	[SerializeField] CinemachineVirtualCamera cam;
	[SerializeField] float zoomSpeed = 1f;

	float defaultZoom;
	bool hitstopActive = false;

	static HitStopper instance;

	public static HitStopper Instance {
		get {
			if (instance == null) {
				Debug.LogWarning ("Attempted to access GameSession instance with no available instance.");//This shouldn't happen
			}
			return instance;
		}
	}

	void Awake() {
		if (instance != null && instance != this) {
			Destroy (gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
	}

	void Start() {
		defaultZoom = cam.m_Lens.OrthographicSize;
	}

	public void StartHitStop(float zoomMulti, float timeScale, float duration) {
		if (!hitstopActive) {
			StartCoroutine (HitStop (zoomMulti, timeScale, duration));
		}
	}

	IEnumerator HitStop(float zoomMulti, float timeScale, float duration) {
		hitstopActive = true;
		float halfDuration = duration / 2;
		float startTime = Time.time;
		float finishTime = Time.time + duration;
		float midTime = Time.time + halfDuration;
		float closestZoom = defaultZoom * zoomMulti;
		//Debug.Log ("Start time: " + startTime + ", Finish time: " + finishTime + ", Mid time: " + midTime + ", duration: " + duration + ", half duration: " + halfDuration);
		while (Time.time < midTime) {
			float currentT = (Time.time - startTime) / halfDuration;
			cam.m_Lens.OrthographicSize = Mathf.Lerp (defaultZoom, closestZoom, currentT);
			Time.timeScale = Mathf.Lerp (1f, timeScale, currentT);
			yield return null;
			//Debug.Log (Time.time + " " + midTime);
		}
		while (Time.time < finishTime) {
			float currentT = (Time.time - midTime) / halfDuration;
			cam.m_Lens.OrthographicSize = Mathf.Lerp (closestZoom, defaultZoom, currentT);
			Time.timeScale = Mathf.Lerp (timeScale, 1f, currentT);
			yield return null;
		}
		Time.timeScale = 1f;
		cam.m_Lens.OrthographicSize = defaultZoom;
		hitstopActive = false;
	}
}
