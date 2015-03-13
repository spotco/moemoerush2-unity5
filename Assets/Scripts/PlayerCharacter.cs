using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour {

	public static string ANIM_WINK = "wink_00";
	public static string ANIM_WALK = "walk_00";
	public static string ANIM_THROWN = "thrown_20_p";
	public static string ANIM_SPRINT = "sprint_00";
	public static string ANIM_SPECIAL1 = "special_20_p";
	public static string ANIM_SPECIAL2 = "special_21";
	public static string ANIM_KICK = "kick_21";
	public static string ANIM_SIDESTEP_LEFT = "sidestep_10_p";
	public static string ANIM_SIDESTEP_RIGHT = "sidestep_11_p";
	public static string ANIM_SALUTE = "salute_01";
	public static string ANIM_JUMP_1 = "jump_10_a";
	public static string ANIM_JUMP_2 = "jump_10_b";
	public static string ANIM_JUMP_3 = "jump_10_c";
	public static string ANIM_IDLE = "idle_00";
	public static string ANIM_ROLL = "forwardroll_10_p";

	[SerializeField] private GameObject _anim_body_root;
	[SerializeField] private GameObject _headless_body_root;

	[SerializeField] public GameObject _ovr_root_camera;
	[SerializeField] public GameObject _ovr_eye_center;

	[SerializeField] public GameObject _left_arm;
	[SerializeField] public GameObject _right_arm;

	[SerializeField] public PlayerBeam _left_beam;
	[SerializeField] public PlayerBeam _right_beam;

	[SerializeField] private GameObject _head_camera;

	[SerializeField] public GameObject _explosion_anchor;
	[SerializeField] public Transform _ovrcamera_start_anchor;
	[SerializeField] public Transform _ovrcamera_end_anchor;

	public void i_initialize(BattleGameEngine game) {
		_anim_body_root.SetActive(false);
		_headless_body_root.SetActive(true);
		_left_beam.gameObject.SetActive(true);
		_right_beam.gameObject.SetActive(true);
		this.initialize_head_bob();

		play_anim(ANIM_SPRINT);
	}

	public void i_update(BattleGameEngine game) {
		float tmp;
		Vector3 whand_left = game._sceneref._wii_model._left_hand.transform.localRotation.eulerAngles;
		tmp = whand_left.x;
		whand_left.y += 90;
		whand_left.x = -whand_left.z;
		whand_left.z = tmp;

		Vector3 whand_right = game._sceneref._wii_model._right_hand.transform.localRotation.eulerAngles;
		whand_right.y -= 90;
		tmp = whand_right.x;
		whand_right.x = whand_right.z;
		whand_right.z = -tmp;

		Util.transform_set_euler_local(_left_arm.transform,whand_left);
		Util.transform_set_euler_local(_right_arm.transform,whand_right);

		this.update_head_bob(10.0f);

		this.gameObject.transform.position = Util.vec_add(_ovr_eye_center.transform.position,_ovr_offset);
	}
	[SerializeField] private Vector3 _ovr_offset;

	private float _scale = 0.3f;
	public void update_scale(float val) {
		_scale = Mathf.Clamp(val,0.3f,5.0f);
		this.transform.localScale = Util.valv(_scale);
	}

	private Vector3 _head_camera_parent_initial_pos;
	private float _head_bob_theta = 0;
	private void initialize_head_bob() {
		_head_camera_parent_initial_pos = _head_camera.transform.parent.localPosition;
	}

	private void update_head_bob(float dtheta) {
		_head_bob_theta += dtheta;
		_head_camera.transform.parent.localPosition = Util.vec_add(_head_camera_parent_initial_pos,new Vector3(0,0.035f * Mathf.Abs(Mathf.Sin(_head_bob_theta * Util.deg2rad)),0));
	}

	public void set_headless(bool val) {
		if (val) {
			_anim_body_root.gameObject.SetActive(false);
			_headless_body_root.gameObject.SetActive(true);
		} else {
			_anim_body_root.gameObject.SetActive(true);
			_headless_body_root.gameObject.SetActive(false);
		}
	}
	public void play_anim(string val) {
		_anim_body_root.GetComponent<Animation>().Play(val);
	}
	
}
