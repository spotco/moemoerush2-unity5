using UnityEngine;
using System.Collections;
using System;

public class CountDown : MonoBehaviour {
	[SerializeField] private GameObject three;
	[SerializeField] private GameObject two;
	[SerializeField] private GameObject one;

	private const long PREPARE_INTERVAL = 6000L; // prepare for 6 sec.
	private long prepareTime;
	private BattleGameEngine _game;
	private int currentState;

	public void i_initialize(BattleGameEngine _game){
		prepareTime = DateTime.Now.ToFileTime ();
		this._game = _game;
		currentState = 4;
		three.gameObject.SetActive(false);
		two.gameObject.SetActive(false);
		one.gameObject.SetActive(false);
		this.gameObject.SetActive (true);
	}

	public void i_update(){
		long preparing = DateTime.Now.ToFileTime() - prepareTime;
		if (currentState == 4) {
			three.gameObject.SetActive(true);
			currentState = 3;
		}else if(preparing >= PREPARE_INTERVAL * 10000L / 3.0 && currentState == 3){
			three.gameObject.SetActive(false);
			two.gameObject.SetActive(true);
			currentState = 2;
		}else if(preparing >= PREPARE_INTERVAL * 10000L / 3.0 * 2 && currentState == 2) {
			two.gameObject.SetActive(false);
			one.gameObject.SetActive(true);
			currentState = 1;
		}else if(preparing >= PREPARE_INTERVAL * 10000L && currentState == 1){
			// TODO: play the music
			Debug.Log ("Start");

			AudioSource audio = _game._sceneref._music.GetComponent<AudioSource>();
			//while (!audio.clip.isReadyToPlay) {}

			audio.Play ();

			one.gameObject.SetActive(false);
			currentState = 0;
			_game._current_mode = BattleGameEngineMode.GamePlay;
			_game._sceneref._ui._count_down.gameObject.SetActive(false);
			prepareTime = 0;
			_game.musicStartTime = DateTime.Now.ToFileTime();
		}
	}
}
