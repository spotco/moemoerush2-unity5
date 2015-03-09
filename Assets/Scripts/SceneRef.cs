using UnityEngine;
using System;
using System.Collections.Generic;

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

		this.gameObject.AddComponent<BattleGameEngine>().i_initialize(this);
		this._main_menu.i_initialize(this);
		this.set_mode_visible();
	}

	private void set_mode_visible() {
		if (_mode == SceneMode.GameMenu) {
			_main_menu.gameObject.SetActive(true);
			_ovr_game_camera.gameObject.SetActive(false);

		} else if (_mode == SceneMode.GameEngine) {
			_main_menu.gameObject.SetActive(false);
			_ovr_game_camera.gameObject.SetActive(true);
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
