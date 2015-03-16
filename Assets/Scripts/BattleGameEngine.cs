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
	TransitionToEnd,
	GameEnd
};

public class BattleGameEngine : MonoBehaviour {
	[NonSerialized] public SceneRef _sceneref;
	[NonSerialized] public BattleGameEngineMode _current_mode;
	[NonSerialized] public List<BaseParticle> _particles = new List<BaseParticle>();
	[NonSerialized] public ScoreManager _score = new ScoreManager();

	public void i_initialize(SceneRef sceneref) {
		_score.i_initialize();
		_sceneref = sceneref;
		_sceneref._ui.i_initialize(this);

		_current_mode = BattleGameEngineMode.PreIntroTransitionStart;
		_sceneref._player.i_initialize(this);

		prep_into_transition();

		_left_hand_fire_count = FIRE_COUNT_MAX;
		_right_hand_fire_count = FIRE_COUNT_MAX;
	}

	private float _anim_theta = 0;
	private float _anim_dist = 0;
	private Vector3 _anim_initial_pos;
	private Quaternion _anim_initial_rotation;
	private void prep_into_transition() {
		_sceneref._player._ovr_root_camera.transform.position = _sceneref._player._ovrcamera_start_anchor.transform.position;
		_sceneref._player._ovr_root_camera.transform.LookAt(_sceneref._player._ovrcamera_end_anchor);
		_anim_dist = Util.vec_dist(
			new Vector3(_sceneref._player.transform.position.x,_sceneref._player.transform.position.y),
		    new Vector3(_sceneref._player._ovr_root_camera.transform.position.x,_sceneref._player._ovr_root_camera.transform.position.y)
		);


	}


	public void i_update() {
		if (_current_mode == BattleGameEngineMode.PreIntroTransitionStart) {
			_sceneref._ui.i_update (this);
			_anim_theta += 0.0025f;
			Vector3 neu_pos = new Vector3(
				Mathf.Cos(_anim_theta)*_anim_dist + _sceneref._player.transform.position.x,
				_sceneref._player._ovr_root_camera.transform.position.y,
				Mathf.Sin(_anim_theta)*_anim_dist + _sceneref._player.transform.position.z
			);
			_sceneref._player._ovr_root_camera.transform.position = neu_pos;
			_sceneref._player._ovr_root_camera.transform.LookAt(_sceneref._player._ovrcamera_end_anchor);
			if (Input.GetKeyUp(KeyCode.Escape) || (_sceneref._socket_server._left_trig_pressed && _sceneref._socket_server._right_trig_pressed)) {
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
			if (_anim_theta == 0)SFXLib.inst.play_sfx(SFXLib.inst.sfx_ready);
			_anim_theta += 0.015f;
			_sceneref._player._ovr_root_camera.transform.position = Util.sin_lerp_vec(_anim_initial_pos,_sceneref._player._ovrcamera_end_anchor.transform.position,_anim_theta);
			_sceneref._player._ovr_root_camera.transform.rotation = Quaternion.Slerp(_anim_initial_rotation,_sceneref._player._ovrcamera_end_anchor.transform.rotation,Util.sin_lerp(0,1,_anim_theta));
			if (_anim_theta < 0.3f) {
				_sceneref._ui.show_countdown_ui(3);
				_sceneref._player.set_headless(false);
				if (_anim_theta+0.015f > 0.3f)SFXLib.inst.play_sfx(SFXLib.inst.sfx_ready);

			} else if (_anim_theta < 0.6f) {
				_sceneref._ui.show_countdown_ui(2);
				_sceneref._player.set_headless(false);
				if (_anim_theta+0.015f > 0.6f)SFXLib.inst.play_sfx(SFXLib.inst.sfx_ready);

			} else {
				_sceneref._ui.show_countdown_ui(1);
				if (_anim_theta < 0.85f) {
					_sceneref._player.set_headless(false);
				} else {
					_sceneref._player.set_headless(true);
				}
			}
			foreach (RepeatInstance repinst_itr in _sceneref._repeaters) {
				repinst_itr.i_update (this);
			}


			if (_anim_theta >= 1) {
				_sceneref._ui.show_countdown_ui(0);
				SFXLib.inst.play_sfx(SFXLib.inst.sfx_go);
				_current_mode = BattleGameEngineMode.GamePlay;
				_sceneref._enemies.i_initialize(this);
				if (_sceneref._music != null) _sceneref._music.Play();
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
			update_fire_count();

			if (_score._health <= 0 || _sceneref._enemies.is_enemies_finished()) {
				prep_transition_to_end();
			}

		} else if (_current_mode == BattleGameEngineMode.TransitionToEnd) {
			_sceneref._ui.i_update (this);
			Vector3 tar_pos = new Vector3(
				Mathf.Cos(_anim_theta)*_anim_dist + _sceneref._player.transform.position.x,
				_sceneref._player._ovrcamera_gameover_anchor.transform.position.y,
				Mathf.Sin(_anim_theta)*_anim_dist + _sceneref._player.transform.position.z
			);
			Quaternion pre = _sceneref._player._ovr_root_camera.transform.rotation;
			_sceneref._player._ovr_root_camera.transform.LookAt(_sceneref._player._ovrcamera_end_anchor);
			Quaternion lookat = _sceneref._player._ovr_root_camera.transform.rotation;
			_sceneref._player._ovr_root_camera.transform.rotation = pre;

			_sceneref._player._ovr_root_camera.transform.position = Util.sin_lerp_vec(_anim_initial_pos,tar_pos,_anim_theta);
			_sceneref._player._ovr_root_camera.transform.rotation = Quaternion.Slerp(_anim_initial_rotation,lookat,Util.sin_lerp(0,1,_anim_theta));
			_anim_theta += 0.025f;
			if (_anim_theta >= 1) {
				_current_mode = BattleGameEngineMode.GameEnd;
				if (_score._health > 0) {
					SFXLib.inst.play_sfx(SFXLib.inst.sfx_game_end_voice);
				} else {
					SFXLib.inst.play_sfx(SFXLib.inst.sfx_girl_sad);
				}
			}

		} else if (_current_mode == BattleGameEngineMode.GameEnd) {
			_anim_theta += 0.0025f;
			_sceneref._ui.i_update (this);
			Vector3 neu_pos = new Vector3(
				Mathf.Cos(_anim_theta)*_anim_dist + _sceneref._player.transform.position.x,
				_sceneref._player._ovr_root_camera.transform.position.y,
				Mathf.Sin(_anim_theta)*_anim_dist + _sceneref._player.transform.position.z
			);
			_sceneref._player._ovr_root_camera.transform.position = neu_pos;
			_sceneref._player._ovr_root_camera.transform.LookAt(_sceneref._player._ovrcamera_end_anchor);
			if (Input.GetKeyUp(KeyCode.Escape)) {
				_sceneref.restart();
			}
		}
	}
	
	public void prep_transition_to_end() {
		if (_sceneref._music != null) _sceneref._music.Stop();
		_sceneref._player.set_headless(false);
		_sceneref._enemies.clear_enemies();
		if (_score._health > 0) {
			_sceneref._player.play_anim(PlayerCharacter.ANIM_CHEER);
		} else {
			_sceneref._player.play_anim(PlayerCharacter.ANIM_DIE,false,0.5f);
		}

		_anim_dist = Util.vec_dist(
			_sceneref._player.transform.position,
			_sceneref._player._ovrcamera_gameover_anchor.transform.position
		);
		_anim_initial_pos = _sceneref._player._ovr_root_camera.transform.position;
		_anim_initial_rotation = _sceneref._player._ovr_root_camera.transform.rotation;
		_anim_theta = 0;
		_current_mode = BattleGameEngineMode.TransitionToEnd;
		SFXLib.inst.play_sfx(SFXLib.inst.sfx_end_jingle);

		Vector3 ppos = _sceneref._player.transform.localPosition;
		ppos.y = 0.46f;
		_sceneref._player.transform.localPosition = ppos;
	}
	
	public int _left_hand_fire_count = 0;
	public int _right_hand_fire_count = 0;
	public const int FIRE_COUNT_MAX = 5;
	public int _last_fired_time_left = 0;
	public int _last_fired_time_right = 0;
	private void update_fire_count() {
		_last_fired_time_left++;
		_last_fired_time_right++;
		if (_last_fired_time_left > 10) {
			_left_hand_fire_count = Math.Min(_left_hand_fire_count+1,FIRE_COUNT_MAX);
		}
		if (_last_fired_time_right > 10) {
			_right_hand_fire_count = Math.Min(_right_hand_fire_count+1,FIRE_COUNT_MAX);
		}
	}
	
	public void player_shoot(int hand_id) {
		if (_current_mode == BattleGameEngineMode.GamePlay) {
			if (_sceneref._wii_model.is_left_hand_id(hand_id)) {
				_last_fired_time_left = 0;
				if (_left_hand_fire_count > 0) {
					_left_hand_fire_count = _left_hand_fire_count-1;
					_sceneref._player._left_beam.shoot();
					_sceneref._enemies.shoot(this,ControllerHand.Left);
					_sceneref._player.fire_arm(ControllerHand.Left);
					SFXLib.inst.play_sfx(SFXLib.inst.sfx_shoot);
				} else {
					SFXLib.inst.play_sfx(SFXLib.inst.sfx_buzz);
				}
				
			} else if (_sceneref._wii_model.is_right_hand_id(hand_id)) {
				_last_fired_time_right = 0;
				if (_right_hand_fire_count > 0) {
					_right_hand_fire_count = _right_hand_fire_count-1;
					_sceneref._player._right_beam.shoot();
					_sceneref._enemies.shoot(this,ControllerHand.Right);
					_sceneref._player.fire_arm(ControllerHand.Right);
					SFXLib.inst.play_sfx(SFXLib.inst.sfx_shoot);
				} else {
					SFXLib.inst.play_sfx(SFXLib.inst.sfx_buzz);
				}
			}
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

public class ScoreManager {	
	public int _score;
	public int _health;
	public int _combo;
	public const int MAX_HEALTH = 100;
	public const int DAMAGE_PER_HIT = 5;
	
	public void i_initialize(){
		_score = 0;
		_combo = 0;
		_health = MAX_HEALTH;
	}
	
	public void hit_success(BattleGameEngine game){
		int combo_mult;
		if (_combo > 60) {
			combo_mult = 4;
		} else if (_combo > 40) {
			combo_mult = 3;
		} else if (_combo > 20) {
			combo_mult = 2;
		} else {
			combo_mult = 1;
		}

		_score += 20 * combo_mult;
		_health += 1 * combo_mult;
		_combo++;

		if (_combo % 10==0 && _combo>0) {
			game._sceneref._ui.show_combo(_combo);
		}

		if (_health >= 100) {
			_health = 100;
		}
	}
	
	public void hit_failure(BattleGameEngine game){
		_combo = 0;
		_health -= DAMAGE_PER_HIT;
		if (_health < 0) _health = 0;
	}
	
	public bool is_dead(){
		return (_health <= 0);
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

