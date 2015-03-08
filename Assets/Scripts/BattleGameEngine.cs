using UnityEngine;
using System.Collections.Generic;
using System;
using BeatProcessor;
using System.IO;

public enum ControllerHand {
	Left,
	Right
};

public enum BattleGameEngineMode {
	Menu,
	Import,
	GamePrepare,
	GamePlay,
	Pause,
	End
};

public class BattleGameEngine : MonoBehaviour {
	public List<long> beats;
	public long musicLength;
	public long musicStartTime;

	[NonSerialized] public SceneRef _sceneref;
	[NonSerialized] public BattleGameEngineMode _current_mode;
	[NonSerialized] public List<BaseParticle> _particles = new List<BaseParticle>();

	private SocketServer _socket_server;

	public void i_initialize(SceneRef sceneref) {
		_sceneref = sceneref;
		_sceneref._ui.i_initialize(this);
		foreach(RepeatInstance repinst_itr in _sceneref._repeaters) {
			repinst_itr.i_initialize(this);
		}
		_current_mode = BattleGameEngineMode.Menu;

		_socket_server = this.gameObject.AddComponent<SocketServer>();
		_socket_server.i_initialize(this);

		_sceneref._wii_model.i_initialize();
		this.beats = new List<long> ();

		// Open Main Menu
		_sceneref._ui._main_menu.gameObject.SetActive (true);
		_sceneref._player.gameObject.SetActive(false);
	}
	
	public void Update() {
		if (_current_mode == BattleGameEngineMode.GamePlay) {
			_sceneref._player.i_update (this);
			_sceneref._enemies.i_update (this);
			_sceneref._ui.i_update (this);
			update_particles ();

			foreach (RepeatInstance repinst_itr in _sceneref._repeaters) {
					repinst_itr.i_update (this);
			}
			if(_sceneref._ui._score_manager.isPlayerDead() 
			   || DateTime.Now.ToFileTime() - musicStartTime >= (musicLength + 1000L) * 10000L){
				on_key_end();
			}
		} else if(_current_mode == BattleGameEngineMode.GamePrepare){
			_sceneref._ui._count_down.i_update();
			_sceneref._enemies.i_update (this);
		}

		if (Input.GetKeyUp (KeyCode.S)) {
			on_key_start ();
		} else if (Input.GetKeyUp (KeyCode.I)) {
			on_key_import ();		
		} else if (Input.GetKeyUp (KeyCode.O)) {
			on_key_open_filepicker ();		
		} else if (Input.GetKeyUp (KeyCode.M)) {
			on_key_menu();		
		} else if (Input.GetKeyUp(KeyCode.E)) {
			on_key_end();
		} else if (Input.GetKeyUp(KeyCode.T)){
			on_key_trick();
		}
	}

	public void player_shoot(int hand_id) {
		if (_sceneref._wii_model.is_left_hand_id(hand_id)) {
			_sceneref._player._left_beam.shoot();
			_sceneref._enemies.shoot(this,ControllerHand.Left);

		} else if (_sceneref._wii_model.is_right_hand_id(hand_id)) {
			_sceneref._player._right_beam.shoot();
			_sceneref._enemies.shoot(this,ControllerHand.Right);
		}
	}



	public BaseParticle add_particle(string name) {
		BaseParticle particle = ((GameObject)Instantiate(Resources.Load(name))).GetComponent<BaseParticle>();
		particle.i_initialize(this);
		particle.gameObject.transform.parent = _sceneref._particle_root.transform;
		_particles.Add(particle);
		return particle;
	}

	private void update_particles() {
		for (int i_particle = _particles.Count-1; i_particle >= 0; i_particle--) {
			BaseParticle itr_particle = _particles[i_particle];
			itr_particle.i_update(this);
			if (itr_particle.should_remove(this)) {
				itr_particle.do_remove(this);
				itr_particle.gameObject.transform.parent = null;
				Destroy(itr_particle.gameObject);
				_particles.RemoveAt(i_particle);
			}
		}
	}

	public void on_key_start(){
		if (_current_mode == BattleGameEngineMode.Menu) {
			if(beats.Count == 0){
				// TODO: Message: Please import music!
				Debug.Log ("Please Import Music!!");
			}else{
				_sceneref._ui._main_menu.gameObject.SetActive(false);
			}
		} else if (_current_mode == BattleGameEngineMode.End) {
			_sceneref._ui._score_menu.gameObject.SetActive(false);
		}
		_current_mode = BattleGameEngineMode.GamePrepare;
		_sceneref._ui._score_manager.gameObject.SetActive(true);
		_sceneref._ui._count_down.i_initialize(this);
		_sceneref._ui._score_manager.i_initialize();
		_sceneref._player.gameObject.SetActive(true);
		_sceneref._enemies.gameObject.SetActive(true);
		_sceneref._player.i_initialize(this);
		_sceneref._enemies.i_initialize(this);
		this.musicStartTime = 0;
	}

	public void on_key_import(){
		if (_current_mode == BattleGameEngineMode.Menu) {
			_current_mode = BattleGameEngineMode.Import;
			// TODO: display "importing..."
			Debug.Log("Importing...");
			_sceneref._ui._main_menu.gameObject.SetActive(false);
			_sceneref._ui._import_menu.gameObject.SetActive(true);
		}
	}

	public void on_key_open_filepicker(){
		/*
		if (_current_mode == BattleGameEngineMode.Import) {
			String filename = EditorUtility.OpenFilePanel("Choose .wav file", "", "wav");
			AudioSource audio = _sceneref._music.GetComponent<AudioSource>();
			WWW www = new WWW("file:///" + filename);
			audio.clip = www.audioClip;
			try{
				beats = new List<long>();
				if(filename.EndsWith(".wav")){
					musicLength = BeatDetector.outputBeats(filename, 1, beats);
				}else{
					Debug.Log ("Only support .wav files");
					_sceneref._ui._import_menu.wrongFileTypeMessage();
					return;
				}
			}catch(FileNotFoundException e){
				Debug.Log ("File Not Found: " + filename);
				_sceneref._ui._import_menu.fileNotFoundMessage();
				return;
			}
			Debug.Log("Import Succeed!");
			_sceneref._ui._import_menu.successMessage();
		}
		*/
		_filepicker_open = true;
	}

	private bool _filepicker_open = false;
	public void OnGUI() {
		//Debug.Log ("test");
		//if (_filepicker_open) {
			UniFileBrowser.use.OpenFileWindow(OpenFile);
		//}
	}

	void OpenFile (string pathToFile) {
		Debug.Log ("file");
	}

	public void on_key_menu(){
		if (_current_mode == BattleGameEngineMode.Import) {
			_sceneref._ui._import_menu.gameObject.SetActive(false);
		} else if (_current_mode == BattleGameEngineMode.End) {
			_sceneref._ui._score_menu.gameObject.SetActive(false);
		} else if (_current_mode == BattleGameEngineMode.Pause) {

		}
		_current_mode = BattleGameEngineMode.Menu;
		_sceneref._ui._main_menu.gameObject.SetActive(true);
	}

	public void on_key_end(){
		if(_current_mode == BattleGameEngineMode.GamePlay){
			_current_mode = BattleGameEngineMode.End;
			_sceneref._player.gameObject.SetActive(false);
			_sceneref._music.GetComponent<AudioSource>().Stop();

			// Destroy all enemy objects
			_sceneref._enemies.destroy_all_enemies (_sceneref._ui);
			_sceneref._enemies.gameObject.SetActive(false);
			int finalScore = _sceneref._ui._score_manager.getScore();
			_sceneref._ui._score_manager.gameObject.SetActive(false);
			_sceneref._ui._score_menu.gameObject.SetActive(true);
			_sceneref._ui._score_menu.setScore(finalScore);
		}else if(_current_mode == BattleGameEngineMode.Pause){

		}
	}

	public void on_key_trick(){
		_sceneref._ui._score_manager.hitSuccess ();
	}
}


public class BaseParticle : MonoBehaviour {
	public virtual void i_initialize(BattleGameEngine game) {}
	public virtual void i_update(BattleGameEngine game) {}
	public virtual bool should_remove(BattleGameEngine game) { return true; }
	public virtual void do_remove(BattleGameEngine game) {}
	
	public BaseParticle set_position(Vector3 pos) {
		this.gameObject.transform.position = pos;
		return this;
	}
	
}

