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
	[SerializeField] public GameObject _music;

	[SerializeField] private GameObject _repeaters_root;
	[NonSerialized] public List<RepeatInstance> _repeaters;

	[SerializeField] public GameMenu _main_menu;
	[SerializeField] public OVRCameraRig _ovr_game_camera;

	public void Start() {
		inst = this;
		_repeaters = new List<RepeatInstance>(_repeaters_root.GetComponentsInChildren<RepeatInstance>());
		this.gameObject.AddComponent<BattleGameEngine>().i_initialize(this);
	}
	
}
