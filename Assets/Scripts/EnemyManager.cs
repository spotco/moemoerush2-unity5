using UnityEngine;
using System.Collections.Generic;
using System;

public class EnemyManager : MonoBehaviour {
	private List<long> beats;
	private int currentIndex;
	private long gameStartTime;
	private const long INVULN_TIME = 4000L;
	private const long HIT_TIME = 2000L;

	[SerializeField] MissileEnemy _missile_enemy_proto;
	[SerializeField] private GameObject _spawn_points_air;

	private List<BaseEnemy> _enemies = new List<BaseEnemy>();
	public List<BaseEnemy> list() { return _enemies; }

	public void i_initialize(BattleGameEngine game) {
		_missile_enemy_proto.gameObject.SetActive(false);
		beats = game.beats;
		currentIndex = 0;
		gameStartTime = DateTime.Now.ToFileTime();
	}

	public void i_update(BattleGameEngine game) {
		if(currentIndex < beats.Count && beats[currentIndex] * 10000L <= DateTime.Now.ToFileTime() - gameStartTime){
		//if (Input.GetKeyUp(KeyCode.Space) || Util.rand_range(0,100) < 2) {
			MissileEnemy neu_enemy = Util.proto_clone(_missile_enemy_proto.gameObject).GetComponent<MissileEnemy>();
			Vector3 spawn_pos = _spawn_points_air.transform.GetChild(Util.int_random(0,_spawn_points_air.transform.childCount)).position;
			neu_enemy.i_initialize(game, spawn_pos, INVULN_TIME * 10000L, HIT_TIME * 10000L);
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
		game._sceneref._ui._score_manager.hitSuccess ();
		_enemies.RemoveAt(_enemies.IndexOf(itr_enemy));
		Destroy(itr_enemy.gameObject);
	}

	public void hit_player(BattleGameEngine game, BaseEnemy itr_enemy, long time){
		itr_enemy.do_remove_hit_player(game);
		game._sceneref._ui._score_manager.hitFailure ();
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
	public virtual void i_update(BattleGameEngine game) {
		this.transform.position = Vector3.Lerp(_start_position,game._sceneref._player.transform.position,this.t());
		this.transform.LookAt(game._sceneref._player.transform.position);
	}
	public virtual bool should_remove(BattleGameEngine game) {
		return DateTime.Now.ToFileTime() >= _end_time;
	}
	public virtual void do_remove_killed(BattleGameEngine game) {}
	public virtual void do_remove_hit_player(BattleGameEngine game) {}

	public virtual Vector3 get_center() {
		return this.transform.position;
	}
}



