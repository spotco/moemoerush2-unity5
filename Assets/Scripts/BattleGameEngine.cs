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
	GamePrepare,
	GamePlay,
	End
};

/*
TODO -- 
Prepmenu to game start (+ovr) transition
Death anim and menu
ground type enemy
game end anim and menu

score flyouts
hit screenshake and pause

wiimote ir pointing algorithm
*/
public class BattleGameEngine : MonoBehaviour {
	[NonSerialized] public SceneRef _sceneref;
	[NonSerialized] public BattleGameEngineMode _current_mode;
	[NonSerialized] public List<BaseParticle> _particles = new List<BaseParticle>();

	public void i_initialize(SceneRef sceneref) {
		_sceneref = sceneref;
		_sceneref._ui.i_initialize(this);
		foreach(RepeatInstance repinst_itr in _sceneref._repeaters) {
			repinst_itr.i_initialize(this);
		}
		_current_mode = BattleGameEngineMode.GamePrepare;
		_sceneref._player.i_initialize(this);

	}
	
	public void i_update() {
		if (_current_mode == BattleGameEngineMode.GamePlay) {
			_sceneref._player.i_update (this);
			_sceneref._enemies.i_update (this);
			_sceneref._ui.i_update (this);
			update_particles ();

			foreach (RepeatInstance repinst_itr in _sceneref._repeaters) {
					repinst_itr.i_update (this);
			}
			if(_sceneref._ui._score_manager.isPlayerDead()){
				game_over();
			}
		} else if(_current_mode == BattleGameEngineMode.GamePrepare){
			_sceneref._ui._count_down.i_update();

			if (Input.GetKeyUp(KeyCode.Escape)) {
				_sceneref._enemies.i_initialize(this);
				_sceneref._music.Play();
				_current_mode = BattleGameEngineMode.GamePlay;
			}
		}
	}

	public void game_over() {
		//Debug.LogError("game over");
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

