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
	PreIntroTransitionStart,
	IntroTransition,
	GamePlay,
	End
};

/*
TODO -- 
Prepmenu to game start (+ovr) transition
ground type enemy
Death anim and menu
game end anim and menu
combo and low health effects
animated hand
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
		_current_mode = BattleGameEngineMode.PreIntroTransitionStart;
		_sceneref._player.i_initialize(this);

		prep_into_transition();
	}

	private float _anim_theta = 0;
	private float _anim_dist = 0;
	private Vector3 _anim_initial_pos;
	private Quaternion _anim_initial_rotation;
	private void prep_into_transition() {
		_sceneref._player._ovr_root_camera.transform.position = _sceneref._player._ovrcamera_start_anchor.transform.position;
		_sceneref._player._ovr_root_camera.transform.LookAt(_sceneref._player._ovrcamera_end_anchor);
		_anim_dist = Util.vec_dist(Vector3.zero,new Vector3(_sceneref._player._ovr_root_camera.transform.position.x,0,_sceneref._player._ovr_root_camera.transform.position.y));


	}


	public void i_update() {
		if (_current_mode == BattleGameEngineMode.PreIntroTransitionStart) {
			_sceneref._ui.i_update (this);
			_anim_theta += 0.01f;
			Vector3 neu_pos = new Vector3(Mathf.Cos(_anim_theta)*_anim_dist,_sceneref._player._ovr_root_camera.transform.position.y,Mathf.Sin(_anim_theta)*_anim_dist);
			_sceneref._player._ovr_root_camera.transform.position = neu_pos;
			_sceneref._player._ovr_root_camera.transform.LookAt(_sceneref._player._ovrcamera_end_anchor);
			if (Input.GetKeyUp(KeyCode.Space)) {
				_current_mode = BattleGameEngineMode.IntroTransition;
				_anim_initial_pos = _sceneref._player._ovr_root_camera.transform.position;
				_anim_initial_rotation = _sceneref._player._ovr_root_camera.transform.rotation;
				_anim_theta = 0;
			}
			foreach (RepeatInstance repinst_itr in _sceneref._repeaters) {
				repinst_itr.i_update (this);
			}
			_sceneref._player.set_headless(false);

		} else if (_current_mode == BattleGameEngineMode.IntroTransition) {
			_sceneref._ui.i_update (this);
			_anim_theta += 0.0075f;
			_sceneref._player._ovr_root_camera.transform.position = Util.sin_lerp_vec(_anim_initial_pos,_sceneref._player._ovrcamera_end_anchor.transform.position,_anim_theta);
			_sceneref._player._ovr_root_camera.transform.rotation = Quaternion.Slerp(_anim_initial_rotation,_sceneref._player._ovrcamera_end_anchor.transform.rotation,Util.sin_lerp(0,1,_anim_theta));
			if (_anim_theta < 0.3f) {
				_sceneref._ui.show_countdown_ui(3);
			} else if (_anim_theta < 0.6f) {
				_sceneref._ui.show_countdown_ui(2);
			} else {
				_sceneref._ui.show_countdown_ui(1);
			}
			foreach (RepeatInstance repinst_itr in _sceneref._repeaters) {
				repinst_itr.i_update (this);
			}
			_sceneref._player.set_headless(false);

			if (_anim_theta >= 1) {
				_sceneref._ui.show_countdown_ui(0);
				_current_mode = BattleGameEngineMode.GamePlay;
				_sceneref._enemies.i_initialize(this);
				//_sceneref._music.Play();
			}
		} else if (_current_mode == BattleGameEngineMode.GamePlay) {
			_sceneref._player.i_update (this);
			_sceneref._enemies.i_update (this);
			_sceneref._ui.i_update (this);
			update_particles ();
			
			foreach (RepeatInstance repinst_itr in _sceneref._repeaters) {
				repinst_itr.i_update (this);
			}
			_sceneref._player.set_headless(true);
			
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

