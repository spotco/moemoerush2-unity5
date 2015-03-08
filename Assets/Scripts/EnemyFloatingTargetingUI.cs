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
		_retic_target_alpha = _reticule_image.color.a;
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
		
		if (Util.sphere_collider_intersect(game._sceneref._ui._left_hand_target.GetComponent<SphereCollider>(),this.GetComponent<SphereCollider>()) || 
		    Util.sphere_collider_intersect(game._sceneref._ui._right_hand_target.GetComponent<SphereCollider>(),this.GetComponent<SphereCollider>()) ) {
			_reticule_image.color = new Color(1.0f,0.8f,0.8f,0.85f);
		} else {
			_reticule_image.color = new Color(1.0f,0.0f,0.0f,0.85f);
		}
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
