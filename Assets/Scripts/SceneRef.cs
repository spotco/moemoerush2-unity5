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

	[NonSerialized] public SocketServer _socket_server;

	[NonSerialized] public WavReader _wav_reader;

	public enum SceneMode {
		GameMenu,
		GameEngine
	}
	[NonSerialized] public SceneMode _mode;

	public void Start() {
		inst = this;
		_mode = SceneMode.GameMenu;
		_repeaters = new List<RepeatInstance>(_repeaters_root.GetComponentsInChildren<RepeatInstance>());

		_socket_server = this.gameObject.AddComponent<SocketServer>();
		_socket_server.i_initialize(this);
		_wii_model.i_initialize();


		this._main_menu.i_initialize(this);
		this.set_mode_visible();


		if (true) {
			WavReader test = new WavReader("/Users/spotco/moemoerush/test.wav");
			test.readWav();
			this._music.clip = test.getAudioClip();
			test.getAudioClip();
			test.getBeatTimings();
			this._wav_reader = test;
			this._mode = SceneRef.SceneMode.GameEngine;
		}
	}

	private void set_mode_visible() {
		if (_mode == SceneMode.GameMenu) {
			_main_menu.gameObject.SetActive(true);
			_ovr_game_camera.gameObject.SetActive(false);
			if (this.game() != null) {
				Destroy(this.game());
			}

		} else if (_mode == SceneMode.GameEngine) {
			_main_menu.gameObject.SetActive(false);
			_ovr_game_camera.gameObject.SetActive(true);
			if (this.game() == null) {
				this.gameObject.AddComponent<BattleGameEngine>().i_initialize(this);
			}
		}
	}

	public void Update() {
		this.set_mode_visible();
		if (_mode == SceneMode.GameMenu) {
			_main_menu.i_update();
			
		} else if (_mode == SceneMode.GameEngine) {
			this.GetComponent<BattleGameEngine>().i_update();

		}
	}

	public BattleGameEngine game() { return this.GetComponent<BattleGameEngine>(); }
	
}
