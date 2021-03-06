﻿using UnityEngine;
using System.Collections.Generic;
using System;

public class EnemyManager : MonoBehaviour {
	private List<long> beats;
	private int currentIndex;
	private long gameStartTime;

	public bool is_enemies_finished() {
		if (beats == null) return false;
		return currentIndex >= beats.Count && _enemies.Count == 0;
	}
	public void clear_enemies() {
		foreach(BaseEnemy itr in _enemies) {
			Destroy(itr.gameObject);
		}
		_enemies.Clear();
	}

	[SerializeField] MissileEnemy _missile_enemy_proto;
	[SerializeField] HoverScoutEnemy _hoverscout_enemy_proto;
	[SerializeField] AssaultPlatformEnemy _assaultplatform_enemy_proto;
	[SerializeField] RobotSoldierEnemy _robotsoldier_enemy_proto;

	[SerializeField] private GameObject _spawn_points_air;
	[SerializeField] private GameObject _spawn_points_ground;

	private List<BaseEnemy> _enemies = new List<BaseEnemy>();
	public List<BaseEnemy> list() { return _enemies; }

	public void i_initialize(BattleGameEngine game) {
		_missile_enemy_proto.gameObject.SetActive(false);
		if (game._sceneref._wav_reader != null) {
			beats = game._sceneref._wav_reader.getBeatTimings();
		} else {
			beats = new List<long>();
		}

		currentIndex = 0;
		gameStartTime = DateTime.Now.ToFileTime();
	}

	public const long INVULN_TIME = 4000l;
	public const long HIT_TIME = 1500l;
	public const long MS_TO_100NS = 10000l;
	
	public void i_update(BattleGameEngine game) {
		if(currentIndex < beats.Count && (beats[currentIndex] * MS_TO_100NS)-INVULN_TIME <= DateTime.Now.ToFileTime() - gameStartTime){

			BaseEnemy neu_enemy;
			Vector3 spawn_pos;
			if (Util.int_random(0,4) != 0) {
				if (Util.int_random(0,2) == 0) {
					neu_enemy = Util.proto_clone(_missile_enemy_proto.gameObject).GetComponent<MissileEnemy>();
					spawn_pos = _spawn_points_air.transform.GetChild(Util.int_random(0,_spawn_points_air.transform.childCount)).position;
				} else {
					neu_enemy = Util.proto_clone(_hoverscout_enemy_proto.gameObject).GetComponent<HoverScoutEnemy>();
					spawn_pos = _spawn_points_air.transform.GetChild(Util.int_random(0,_spawn_points_air.transform.childCount)).position;
				}


			} else {
				if (Util.int_random(0,2) == 0) {
					neu_enemy = Util.proto_clone(_assaultplatform_enemy_proto.gameObject).GetComponent<AssaultPlatformEnemy>();
					spawn_pos = _spawn_points_ground.transform.GetChild(Util.int_random(0,_spawn_points_ground.transform.childCount)).position;
				} else {
					neu_enemy = Util.proto_clone(_robotsoldier_enemy_proto.gameObject).GetComponent<RobotSoldierEnemy>();
					spawn_pos = _spawn_points_ground.transform.GetChild(Util.int_random(0,_spawn_points_ground.transform.childCount)).position;
				}

			}
			neu_enemy.i_initialize(game, spawn_pos, INVULN_TIME * MS_TO_100NS, HIT_TIME * MS_TO_100NS);
			_enemies.Add(neu_enemy);

			currentIndex++;
		}

		for(int i_enemy = _enemies.Count-1; i_enemy >= 0; i_enemy--) {
			BaseEnemy itr_enemy = _enemies[i_enemy];
			itr_enemy.i_update(game);
			if (itr_enemy.should_remove(game)) {
				hit_player(game, itr_enemy, DateTime.Now.ToFileTime());
			}
		}
	}

	public void shoot(BattleGameEngine game, ControllerHand hand) {
		SphereCollider target_collider = null;

		for(int i_enemy = _enemies.Count-1; i_enemy >= 0; i_enemy--) {
			BaseEnemy itr_enemy = _enemies[i_enemy];
			if (!itr_enemy.targetable()) continue;
			EnemyFloatingTargetingUI itr_fui = game._sceneref._ui.get_ui_for_enemy(itr_enemy);
			if (itr_fui == null) {
				Debug.LogError ("fui for enemy null");
				continue;
			}

			if ((hand == ControllerHand.Left && itr_fui._left_hand_hit) || (hand == ControllerHand.Right && itr_fui._right_hand_hit)) {
				this.hit_enemy(game, itr_enemy,DateTime.Now.ToFileTime());
			}
		}
	}

	public void hit_enemy(BattleGameEngine game, BaseEnemy itr_enemy, long time) {
		itr_enemy.do_remove_killed(game);
		_enemies.RemoveAt(_enemies.IndexOf(itr_enemy));
		Destroy(itr_enemy.gameObject);

		game._score.hit_success(game);
	}

	public void hit_player(BattleGameEngine game, BaseEnemy itr_enemy, long time){
		itr_enemy.do_remove_hit_player(game);
		_enemies.RemoveAt(_enemies.IndexOf(itr_enemy));
		Destroy(itr_enemy.gameObject);
		game._score.hit_failure(game);
		SFXLib.inst.play_sfx(SFXLib.inst.sfx_itai);
	}

	public void destroy_all_enemies(GameUI _ui){
		foreach(BaseEnemy enemy in _enemies){
			EnemyFloatingTargetingUI fui = _ui.get_ui_for_enemy(enemy);
			Destroy (fui.gameObject);
			Destroy (enemy.gameObject);
		}
		_ui.destroy_all_enemies_ui ();
		_enemies = new List<BaseEnemy> ();
	}
}

public class BaseEnemy : MonoBehaviour {
	protected long _initial_time;
	protected long _invuln_end_time;
	protected long _end_time;
	protected Vector3 _start_position;

	public virtual void i_initialize(BattleGameEngine game, Vector3 start_position,long invuln_time, long hit_time) {
		_initial_time = DateTime.Now.ToFileTime();
		_invuln_end_time = _initial_time + invuln_time;
		_end_time = _invuln_end_time + hit_time;
		_start_position = start_position;
		this.transform.position = _start_position;
	}
	public float t() {
		float t = (DateTime.Now.ToFileTime()-_initial_time)/((float)(_end_time-_initial_time));
		return t;
	}
	public bool targetable() {
		return DateTime.Now.ToFileTime() > _invuln_end_time;
	}

	protected bool _has_played_lockon_sound = false;
	public virtual void i_update(BattleGameEngine game) {
		this.transform.position = Vector3.Lerp(_start_position,game._sceneref._player._explosion_anchor.transform.position,this.t());
		this.transform.LookAt(game._sceneref._player.transform.position);

		if (_end_time - DateTime.Now.ToFileTime() <= EnemyManager.HIT_TIME * EnemyManager.MS_TO_100NS && !_has_played_lockon_sound) {
			_has_played_lockon_sound = true;
			SFXLib.inst.play_sfx(SFXLib.inst.sfx_lockon);
		}
	}
	public virtual bool should_remove(BattleGameEngine game) {
		return DateTime.Now.ToFileTime() >= _end_time;
	}
	public virtual void do_remove_killed(BattleGameEngine game) {
		SFXLib.inst.enqueue_sfx(SFXLib.inst.sfx_hit);
		game._sceneref._player.shake(6,0.06f);
	}
	public virtual void do_remove_hit_player(BattleGameEngine game) {
		SFXLib.inst.play_sfx(SFXLib.inst.sfx_explosion);
		game._sceneref._player.shake(12,0.1f);
	}

	public virtual Vector3 get_center() {
		return this.transform.position;
	}
}



