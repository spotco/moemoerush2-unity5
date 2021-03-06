using UnityEngine;
using System;
using System.Collections.Generic;
using BeatProcessor;

public class SceneRef : MonoBehaviour {

	public static SceneRef inst;

	[SerializeField] public GameUI _ui;
	[SerializeField] public GameObject _particle_root;
	[SerializeField] public PlayerCharacter _player;
	[SerializeField] public WiiModel _wii_model;
	[SerializeField] public EnemyManager _enemies;
	[SerializeField] public AudioSource _music;

	[SerializeField] private GameObject _repeaters_root;
	[NonSerialized] public List<RepeatInstance> _repeaters;

	[SerializeField] public GameMenu _main_menu;
	[SerializeField] public OVRCameraRig _ovr_game_camera;

	[SerializeField] public CGScenes _cgscenes;

	[NonSerialized] public SocketServer _socket_server;

	[NonSerialized] public WavReader _wav_reader;

	public enum SceneMode {
		GameMenu,
		GameEngine,
		CGScenes
	}
	[NonSerialized] public SceneMode _mode;

	public void Start() {
		inst = this;
		_mode = SceneMode.GameMenu;
		_repeaters = new List<RepeatInstance>(_repeaters_root.GetComponentsInChildren<RepeatInstance>());
		foreach(RepeatInstance repinst_itr in _repeaters) {
			repinst_itr.i_initialize();
		}

		_socket_server = this.gameObject.AddComponent<SocketServer>();
		_socket_server.i_initialize(this);
		_wii_model.i_initialize();

		this.gameObject.AddComponent<BattleGameEngine>();


		this._main_menu.i_initialize(this);
		this.set_mode_visible();


		if (true) {
			/*
			WavReader test = new WavReader("/Users/spotco/moemoerush/test.wav");
			test.readWav();
			this._music.clip = test.getAudioClip();
			test.getAudioClip();
			test.getBeatTimings();
			this._wav_reader = test;
			start_game();

			start_cgscenes();
			*/

		}
	}

	private void set_mode_visible() {
		if (_mode == SceneMode.GameMenu) {
			_main_menu.gameObject.SetActive(true);
			_ovr_game_camera.gameObject.SetActive(false);
			_ui.gameObject.SetActive(false);
			_cgscenes.gameObject.SetActive(false);

		} else if (_mode == SceneMode.GameEngine) {
			_main_menu.gameObject.SetActive(false);
			_ui.gameObject.SetActive(true);
			_ovr_game_camera.gameObject.SetActive(true);
			_cgscenes.gameObject.SetActive(false);

		} else if (_mode == SceneMode.CGScenes) {
			_main_menu.gameObject.SetActive(false);
			_ui.gameObject.SetActive(false);
			_ovr_game_camera.gameObject.SetActive(true);
			_cgscenes.gameObject.SetActive(true);

		}
	}

	public void start_cgscenes() {
		_cgscenes.gameObject.SetActive(true);
		_cgscenes.i_initialize(this);
		_mode = SceneMode.CGScenes;
	}

	public void start_game() {
		_cgscenes.gameObject.SetActive(false);
		if (game() != null) Destroy(game());
		this.gameObject.AddComponent<BattleGameEngine>().i_initialize(this);
		_mode = SceneMode.GameEngine;
	}

	public void Update() {
		this.set_mode_visible();
		if (_mode == SceneMode.GameMenu) {
			_main_menu.i_update();
			
		} else if (_mode == SceneMode.GameEngine) {
			game().i_update();

		} else if (_mode == SceneMode.CGScenes) {
			_cgscenes.i_update(this);
		}

		/*
		if (Input.GetKeyUp(KeyCode.P)) {
			this.restart();
		}
		*/
	}

	public BattleGameEngine game() { return this.GetComponent<BattleGameEngine>(); }

	public void restart() {
		if (_music != null) _music.Stop();
		_enemies.clear_enemies();
		_main_menu.i_initialize(this);
		_mode = SceneMode.GameMenu;
		this.set_mode_visible();
	}
	
}
