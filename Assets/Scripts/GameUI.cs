using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;

[RequireComponent(typeof(BoxCollider))]
public class GameUI : MonoBehaviour {
	[SerializeField] public EnemyFloatingTargetingUI _proto_target_reticule;
	[SerializeField] public PlayerTargetReticuleUI _proto_player_target_reticule;
	[SerializeField] public ScoreManager _score_manager;
	[SerializeField] public ScoreMenu _score_menu;
	[SerializeField] public CountDown _count_down;

	public PlayerTargetReticuleUI _left_hand_target;
	public PlayerTargetReticuleUI _right_hand_target;

	public void i_initialize(BattleGameEngine game) {
		_proto_target_reticule.gameObject.SetActive(false);
		_proto_player_target_reticule.gameObject.SetActive(false);
		this.GetComponent<Canvas>().planeDistance = this.transform.localPosition.z;
	
		_left_hand_target = Util.proto_clone(_proto_player_target_reticule.gameObject).GetComponent<PlayerTargetReticuleUI>();
		_right_hand_target = Util.proto_clone(_proto_player_target_reticule.gameObject).GetComponent<PlayerTargetReticuleUI>();

		_score_manager.i_initialize();
		_count_down.i_initialize(game);
	}
	
	private Dictionary<int,EnemyFloatingTargetingUI> _objid_to_targetingui = new Dictionary<int, EnemyFloatingTargetingUI>();
	private List<int> _objsids_to_remove = new List<int>();
	public void i_update(BattleGameEngine game) {
		update_enemy_targeting_uis(game);
		update_player_targeting_uis(game);

	}

	public EnemyFloatingTargetingUI get_ui_for_enemy(BaseEnemy tar) {
		if (!_objid_to_targetingui.ContainsKey(tar.gameObject.GetInstanceID())) return null;
		return _objid_to_targetingui[tar.gameObject.GetInstanceID()];
	}

	private void update_player_targeting_uis(BattleGameEngine game) {
		_left_hand_target.i_update(game,new Ray(game._sceneref._player._left_beam.transform.position,Util.vec_sub(game._sceneref._player._left_beam.transform.position,game._sceneref._player._left_arm.transform.position).normalized));
		_right_hand_target.i_update(game,new Ray(game._sceneref._player._right_beam.transform.position,Util.vec_sub(game._sceneref._player._right_beam.transform.position,game._sceneref._player._right_arm.transform.position).normalized));
	}

	private void update_enemy_targeting_uis(BattleGameEngine game) {
		foreach(EnemyFloatingTargetingUI fui in _objid_to_targetingui.Values) {
			fui._active = false;
		}
		
		foreach(BaseEnemy itr_enemy in game._sceneref._enemies.list()) {
			if (_objid_to_targetingui.ContainsKey(itr_enemy.gameObject.GetInstanceID())) {
				EnemyFloatingTargetingUI fui = _objid_to_targetingui[itr_enemy.gameObject.GetInstanceID()];
				
				Ray cast = new Ray(game._sceneref._player._ovr_eye_center.transform.position,Util.vec_sub(itr_enemy.get_center(),game._sceneref._player._ovr_eye_center.transform.position));
				RaycastHit hit_info;
				bool hit_found = this.GetComponent<BoxCollider>().Raycast(cast,out hit_info,100);
				
				if (hit_found && itr_enemy.targetable()) {
					fui.gameObject.transform.position = hit_info.point;
					fui.gameObject.SetActive(true);
				} else {
					fui.gameObject.SetActive(false);
				}
			}
			if (!_objid_to_targetingui.ContainsKey(itr_enemy.gameObject.GetInstanceID())) {
				_objid_to_targetingui[itr_enemy.gameObject.GetInstanceID()] = Util.proto_clone(_proto_target_reticule.gameObject).GetComponent<EnemyFloatingTargetingUI>().i_initialize(itr_enemy);
			}
			_objid_to_targetingui[itr_enemy.gameObject.GetInstanceID()].i_update(itr_enemy,game);
		}
		
		foreach(int key in _objid_to_targetingui.Keys) {
			if (!_objid_to_targetingui[key]._active) _objid_to_targetingui[key].fadeout();
			if (_objid_to_targetingui[key].should_remove()) _objsids_to_remove.Add(key);
		}
		foreach(int key in _objsids_to_remove) {
			Destroy(_objid_to_targetingui[key].gameObject);
			_objid_to_targetingui.Remove(key);
		}
		_objsids_to_remove.Clear();
	}

	public void destroy_all_enemies_ui(){
		_objsids_to_remove = new List<int> ();
		_objid_to_targetingui = new Dictionary<int, EnemyFloatingTargetingUI> ();
	}
}
