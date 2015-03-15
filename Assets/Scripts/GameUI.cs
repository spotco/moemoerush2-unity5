using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;

[RequireComponent(typeof(BoxCollider))]
public class GameUI : MonoBehaviour {
	[SerializeField] public EnemyFloatingTargetingUI _proto_target_reticule;
	[SerializeField] public PlayerTargetReticuleUI _proto_player_target_reticule;
	[SerializeField] private FadeInOutImage _countdown_3;
	[SerializeField] private FadeInOutImage _countdown_2;
	[SerializeField] private FadeInOutImage _countdown_1;
	[SerializeField] private FadeInOutImage _press_any_key_to_start_flash;
	[SerializeField] private FadeInOutImage _start_text;
	[SerializeField] private FadeInOutImage _combo;
	
	[SerializeField] public IngameHUD _hud;
	[SerializeField] public GameObject _gameover_hud;
	[SerializeField] public EasyFontTextMesh _gameover_score;

	[NonSerialized] public PlayerTargetReticuleUI _left_hand_target;
	[NonSerialized] public PlayerTargetReticuleUI _right_hand_target;

	public void i_initialize(BattleGameEngine game) {
		_proto_target_reticule.gameObject.SetActive(false);
		_proto_player_target_reticule.gameObject.SetActive(false);
		this.GetComponent<Canvas>().planeDistance = this.transform.localPosition.z;
	
		_left_hand_target = Util.proto_clone(_proto_player_target_reticule.gameObject).GetComponent<PlayerTargetReticuleUI>();
		_right_hand_target = Util.proto_clone(_proto_player_target_reticule.gameObject).GetComponent<PlayerTargetReticuleUI>();
		_left_hand_target.i_initialize(ControllerHand.Left);
		_right_hand_target.i_initialize(ControllerHand.Right);

		_hud.i_initialize();
		_press_any_key_to_start_flash.gameObject.SetActive(true);
		_press_any_key_to_start_flash.set_toggle();
	}
	
	private Dictionary<int,EnemyFloatingTargetingUI> _objid_to_targetingui = new Dictionary<int, EnemyFloatingTargetingUI>();
	private List<int> _objsids_to_remove = new List<int>();
	public void i_update(BattleGameEngine game) {
		_left_hand_target.GetComponent<Image>().fillAmount = ((float)game._left_hand_fire_count)/((float)BattleGameEngine.FIRE_COUNT_MAX);
		_right_hand_target.GetComponent<Image>().fillAmount = ((float)game._right_hand_fire_count)/((float)BattleGameEngine.FIRE_COUNT_MAX);
		if (game._current_mode == BattleGameEngineMode.GamePlay) {
			_hud.gameObject.SetActive(true);
			_hud.i_update(game);
			_left_hand_target.gameObject.SetActive(true);
			_right_hand_target.gameObject.SetActive(true);
			_gameover_hud.SetActive(false);

		} else if (game._current_mode == BattleGameEngineMode.GameEnd) {
			_gameover_score.Text = string.Format("Score: "+game._score._score);
			_gameover_hud.SetActive(true);
			_hud.gameObject.SetActive(false);
			_left_hand_target.gameObject.SetActive(false);
			_right_hand_target.gameObject.SetActive(false);

		} else {
			_gameover_hud.SetActive(false);
			_hud.gameObject.SetActive(false);
			_left_hand_target.gameObject.SetActive(false);
			_right_hand_target.gameObject.SetActive(false);
		}
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
					Vector3 fui_localpos = fui.gameObject.transform.localPosition;
					fui_localpos.z = fui.dist_scf_z_offset();
					fui.gameObject.transform.localPosition = fui_localpos;
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
		start_text_update();
	}

	public void destroy_all_enemies_ui(){
		_objsids_to_remove = new List<int> ();
		_objid_to_targetingui = new Dictionary<int, EnemyFloatingTargetingUI> ();
	}

	public void show_countdown_ui(int tar) {
		_press_any_key_to_start_flash.gameObject.SetActive(false);
		if (tar == 3) {
			_countdown_3.show();
			_countdown_2.hide();
			_countdown_1.hide();
		} else if (tar == 2) {
			_countdown_3.hide();
			_countdown_2.show();
			_countdown_1.hide();
		} else if (tar == 1) {
			_countdown_3.hide();
			_countdown_2.hide();
			_countdown_1.show();
		} else {
			_countdown_3.hide();
			_countdown_2.hide();
			_countdown_1.hide();
			_start_text.show();
			_start_text_show_ct = 50;
		}
	}

	private int _start_text_show_ct = 0;
	private void start_text_update() {
		_start_text_show_ct = Mathf.Max(_start_text_show_ct-1,0);
		if (_start_text_show_ct <= 0) {
			_start_text.hide();
		}
	}

	public void show_combo(int val) {
		SFXLib.inst.play_sfx(SFXLib.inst.sfx_cheer);
		_combo.GetComponent<EasyFontTextMesh>().Text = string.Format("{0} Combo!",val);
		_combo.show(50);
	}
}
