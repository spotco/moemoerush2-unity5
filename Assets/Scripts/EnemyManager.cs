using UnityEngine;
using System.Collections.Generic;
using System;

public class EnemyManager : MonoBehaviour {
	private List<long> beats;
	private int currentIndex;
	private long gameStartTime;

	[SerializeField] MissileEnemy _missile_enemy_proto;
	[SerializeField] private GameObject _spawn_points_air;

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
		if(currentIndex < beats.Count && 
		   (beats[currentIndex] * MS_TO_100NS)-INVULN_TIME <= DateTime.Now.ToFileTime() - gameStartTime){

			MissileEnemy neu_enemy = Util.proto_clone(_missile_enemy_proto.gameObject).GetComponent<MissileEnemy>();
			Vector3 spawn_pos = _spawn_points_air.transform.GetChild(Util.int_random(0,_spawn_points_air.transform.childCount)).position;
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
		if (hand == ControllerHand.Left) target_collider = game._sceneref._ui._left_hand_target.get_collider();
		if (hand == ControllerHand.Right) target_collider = game._sceneref._ui._right_hand_target.get_collider();

		for(int i_enemy = _enemies.Count-1; i_enemy >= 0; i_enemy--) {
			BaseEnemy itr_enemy = _enemies[i_enemy];
			if (!itr_enemy.targetable()) continue;
			EnemyFloatingTargetingUI itr_fui = game._sceneref._ui.get_ui_for_enemy(itr_enemy);
			if (itr_fui == null) {
				Debug.LogError ("fui for enemy null");
				continue;
			}

			if (Util.sphere_collider_intersect(target_collider,itr_fui.get_collider())) {
				this.hit_enemy(game, itr_enemy,DateTime.Now.ToFileTime());
			}
		}
	}

	public void hit_enemy(BattleGameEngine game, BaseEnemy itr_enemy, long time) {
		itr_enemy.do_remove_killed(game);
		_enemies.RemoveAt(_enemies.IndexOf(itr_enemy));
		Destroy(itr_enemy.gameObject);
	}

	public void hit_player(BattleGameEngine game, BaseEnemy itr_enemy, long time){
		itr_enemy.do_remove_hit_player(game);
		_enemies.RemoveAt(_enemies.IndexOf(itr_enemy));
		Destroy(itr_enemy.gameObject);
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

	private bool _has_played_lockon_sound = false;
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
	}
	public virtual void do_remove_hit_player(BattleGameEngine game) {
		SFXLib.inst.play_sfx(SFXLib.inst.sfx_explosion);
	}

	public virtual Vector3 get_center() {
		return this.transform.position;
	}
}



