using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(SphereCollider))]
public class EnemyFloatingTargetingUI : MonoBehaviour {
	
	private enum EnemyFloatingTargetingUIMode {
		FadeIn,
		Idle,
		FadeOut
	};
	private EnemyFloatingTargetingUIMode _current_mode;
	
	[SerializeField] private RectTransform _reticule_transform;
	[SerializeField] private Image _reticule_image;
	
	private float _reticule_anim_t = 0; //1 out, 0 in
	private float _retic_target_alpha = 0;
	public bool _active = true;
	
	private float _max_scale = 1.5f, _min_scale = 0.1f, _min_dist = 5.0f, _max_dist = 80.0f;
	
	public EnemyFloatingTargetingUI i_initialize(BaseEnemy itr_enemy) {
		_current_mode = EnemyFloatingTargetingUIMode.FadeIn;
		_reticule_anim_t = 1.0f;
		_retic_target_alpha = 0.35f;
		this.update_reticule_in_anim();
		return this;
	}
	
	private void update_reticule_in_anim() {
		_reticule_transform.localScale = Util.valv((1.0f+1.0f*_reticule_anim_t)*this.dist_scf());
		Color neu_retic_color = _reticule_image.color;
		neu_retic_color.a = (1-_reticule_anim_t)*_retic_target_alpha;
		_reticule_image.color = neu_retic_color;
	}
	
	public void i_update(BaseEnemy itr_enemy, BattleGameEngine game) {
		this.transform.localScale = Util.valv(this.dist_scf(itr_enemy,game));
		_active = true;

		if (_left_hand_hit || _right_hand_hit) {
			_retic_target_alpha = 0.95f;
		} else {
			_retic_target_alpha = 0.75f;
		}
		Color neu_retic_color = _reticule_image.color;
		neu_retic_color.a = _retic_target_alpha;
		_reticule_image.color = neu_retic_color;

	}

	public bool _left_hand_hit = false;
	public bool _right_hand_hit = false;
	private void set_hand_hit(ControllerHand hand, bool val) {
		if (hand == ControllerHand.Left) {
			_left_hand_hit = val;
		} else {
			_right_hand_hit = val;
		}
	}

	void OnTriggerEnter(Collider collision) {
		if (collision.gameObject.GetComponent<PlayerTargetReticuleUI>() != null) set_hand_hit(collision.gameObject.GetComponent<PlayerTargetReticuleUI>()._hand,true);
	}

	void OnTriggerExit(Collider collision) {
		if (collision.gameObject.GetComponent<PlayerTargetReticuleUI>() != null) set_hand_hit(collision.gameObject.GetComponent<PlayerTargetReticuleUI>()._hand,false);
	}

	public SphereCollider get_collider() {
		return this.GetComponent<SphereCollider>();
	}

	private float _last_dist_scf = 0;
	private float dist_scf() { return _last_dist_scf; }
	private float dist_scf(BaseEnemy itr_enemy, BattleGameEngine game) {
		float dist = Util.vec_dist(game._sceneref._player._ovr_eye_center.transform.position,itr_enemy.get_center());
		dist = Mathf.Clamp(dist,_min_dist,_max_dist);
		float val = (_max_scale-_min_scale) * (1-(dist-_min_dist)/(_max_dist-_min_dist)) + _min_scale;
		_last_dist_scf = val;
		return val;
	}
	
	public void fadeout() {
		_current_mode = EnemyFloatingTargetingUIMode.FadeOut;
	}
	
	public bool should_remove() {
		return (_current_mode == EnemyFloatingTargetingUIMode.FadeOut) && (_reticule_anim_t >= 1);
	}
	
	public void Update() {
		if (_current_mode == EnemyFloatingTargetingUIMode.FadeIn) {
			_reticule_anim_t = Mathf.Clamp(_reticule_anim_t-0.2f,0,1);
			if (_reticule_anim_t <= 0) _current_mode = EnemyFloatingTargetingUIMode.Idle;
			update_reticule_in_anim();
			
		} else if (_current_mode == EnemyFloatingTargetingUIMode.Idle) {
			
		} else if (_current_mode == EnemyFloatingTargetingUIMode.FadeOut) {
			_reticule_anim_t = Mathf.Clamp(_reticule_anim_t+0.2f,0,1);
			update_reticule_in_anim();
		}
		_reticule_transform.Rotate(new Vector3(0,0,5));
	}
	
}
