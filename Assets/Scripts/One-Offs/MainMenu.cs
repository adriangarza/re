using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public CanvasGroup pressStart;

	public GameObject cloud;
	public GameObject cloud2;
	public float cloudSpeed;
	
	void Start() {
		StartCoroutine(ToggleOpacity());
	}

	IEnumerator ToggleOpacity() {
		yield return new WaitForSeconds(.7f);
		pressStart.alpha = (pressStart.alpha == 1) ? 0 : 1;
		StartCoroutine(ToggleOpacity());
	}

	void Update() {
		cloud.transform.Translate(cloudSpeed, 0, 0);
		cloud2.transform.Translate(cloudSpeed, 0, 0);

		if (Input.GetKeyDown(KeyCode.Space)) {
			//load the main game asynchronously to prevent loading stutters
			StartCoroutine(StartGame());
		}
	}

	IEnumerator StartGame() {
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("main");
		while (!asyncLoad.isDone) {
			yield return null;
		}
	}
}
