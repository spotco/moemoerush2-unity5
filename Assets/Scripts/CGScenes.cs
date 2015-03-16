using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CGScenes : MonoBehaviour {

	[SerializeField] private Image  _black_fade;
	[SerializeField] private GameObject _box;

	[SerializeField] public GameObject _cg0;

	[SerializeField] public GameObject _cg1;
	[SerializeField] public RectTransform _cg1_bg;
	[SerializeField] public GameObject _cg1_char0_talk;
	[SerializeField] public GameObject _cg1_char1_talk;

	[SerializeField] public GameObject _cg2;
	[SerializeField] public RectTransform _cg2_bg;
	[SerializeField] public RectTransform _cg2_char0;
	[SerializeField] public GameObject _cg2_char0_talk;

	[SerializeField] public Transform _cgscene_pos;

	[SerializeField] private FadeInOutImage _press_any_key_to_start;

	private class CGScene {
		public virtual void i_initialize(CGScenes scenes){}
		public virtual void i_update(CGScenes scenes){}
		public virtual void do_remove(CGScenes scenes){}
		public virtual bool is_finished(CGScenes scenes){ return true; }
	}
	private List<CGScene> _scenes = new List<CGScene>();
	private CGScene _current_scene = null;

	private enum Mode {
		WaitStart,
		FadeIn,
		FadeOut,
		RunningScene,
		End
	}
	private Mode _current_mode;

	public void i_initialize(SceneRef sceneref) {
		sceneref._player._ovr_root_camera.transform.position = _cgscene_pos.position;

		_current_mode = Mode.WaitStart;
		_cg0.SetActive(false);
		_cg1.SetActive(false);
		_cg2.SetActive(false);
		_box.SetActive(true);

		_anim_ct = 0;
		this.set_black_fade_alpha(_anim_ct);

		_scenes.Clear();
		_scenes.Add(new CGScene0());
		_scenes.Add(new CGScene1());
		_scenes.Add(new CGScene2());

		_press_any_key_to_start.set_toggle();
	}

	private float _anim_ct = 0;
	private void set_black_fade_alpha(float val) {
		Color c = _black_fade.color;
		c.a = val;
		_black_fade.color = c;
	}

	public void i_update(SceneRef sceneref) {
		if (_current_mode == Mode.WaitStart) {
			_press_any_key_to_start.gameObject.SetActive(true);
			if ((sceneref._socket_server._left_trig_pressed && sceneref._socket_server._right_trig_pressed) || Input.GetKeyUp(KeyCode.Escape)) {
				_press_any_key_to_start.gameObject.SetActive(false);
				_current_mode = Mode.FadeIn;
			}

		} else if (_current_mode == Mode.FadeIn) {
			_anim_ct += 0.025f;
			set_black_fade_alpha(_anim_ct);
			if (_anim_ct >= 1.0f) {
				if (_current_scene != null) {
					_current_scene.do_remove(this);
				}
				if (_scenes.Count > 0) {
					_current_scene = _scenes[0];
					_current_scene.i_initialize(this);
					_scenes.RemoveAt(0);
				} else {
					_current_scene = null;
				}
				_current_mode = Mode.FadeOut;
			}
			
		} else if (_current_mode == Mode.FadeOut) {
			_anim_ct -= 0.025f;
			set_black_fade_alpha(_anim_ct);
			if (_anim_ct <= 0.0f) {
				if (_current_scene != null) {
					_current_mode = Mode.RunningScene;
				} else {
					_current_mode = Mode.End;
				}
			}

		} else if (_current_mode == Mode.RunningScene) {
			_current_scene.i_update(this);
			if (_current_scene.is_finished(this)) {
				_current_mode = Mode.FadeIn;
			}

		} else if (_current_mode == Mode.End) {
			sceneref.start_game();

		}
	}

	private class CGScene0 : CGScene {

		private Vector3 _start_pos, _end_pos;
		private float _anim_ct = 0;
		public override void i_initialize(CGScenes scenes){
			scenes._cg0.SetActive(true);
			_start_pos = new Vector3(-327,248,200);
			_end_pos = new Vector3(-176,-94,-200);
			_anim_ct = 0;
			scenes._cg0.transform.localPosition = Vector3.Lerp(_start_pos,_end_pos,_anim_ct);
			SFXLib.inst.play_sfx(SFXLib.inst.sfx_crowd);
		}
		public override void i_update(CGScenes scenes){
			_anim_ct += 0.0075f;
			scenes._cg0.transform.localPosition = Vector3.Lerp(_start_pos,_end_pos,_anim_ct);
		}
		public override bool is_finished(CGScenes scenes){ return _anim_ct >= 1.0f; }
		public override void do_remove(CGScenes scenes){
			scenes._cg0.SetActive(false);
		}
	}

	private class CGScene1 : CGScene {
		private enum Mode {
			Trans0,
			Talk0,
			Trans1,
			Talk1,
			End
		}
		private Mode _current_mode;
		private float _anim_ct;

		private Vector3 _start_pos, _end_pos;
		public override void i_initialize(CGScenes scenes){
			scenes._cg1.SetActive(true);
			scenes._cg1_char0_talk.SetActive(false);
			scenes._cg1_char1_talk.SetActive(false);
			_start_pos = new Vector3(2722,773,0);
			_end_pos = new Vector3(432,-42,0);
			_current_mode = Mode.Trans0;
			_anim_ct = 0;
			scenes._cg1_bg.transform.localPosition = Vector3.Lerp(_start_pos,_end_pos,_anim_ct);

		}
		public override void i_update(CGScenes scenes){
			if (_current_mode == Mode.Trans0) {
				_anim_ct += 0.05f;
				scenes._cg1_bg.transform.localPosition = Vector3.Lerp(_start_pos,_end_pos,_anim_ct);
				if (_anim_ct >= 1) {
					_current_mode = Mode.Talk0;
					SFXLib.inst.play_sfx(SFXLib.inst.sfx_soldier_warn);
					_anim_ct = 300;
				}
			} else if (_current_mode == Mode.Talk0) {
				scenes._cg1_char0_talk.SetActive(true);
				scenes._cg1_char0_talk.transform.localPosition = new Vector3(-97+Util.rand_range(-1,1),472+Util.rand_range(-1,1),-50);
				_anim_ct--;
				if (_anim_ct < 0) {
					_current_mode = Mode.Trans1;
					scenes._cg1_char0_talk.SetActive(false);
					_start_pos = new Vector3(432,-42,0);
					_end_pos = new Vector3(-407,-111,0);
					_anim_ct = 0;
				}
			} else if (_current_mode == Mode.Trans1) {
				_anim_ct += 0.1f;
				scenes._cg1_bg.transform.localPosition = Vector3.Lerp(_start_pos,_end_pos,_anim_ct);
				if (_anim_ct >= 1) {
					_current_mode = Mode.Talk1;
					SFXLib.inst.play_sfx(SFXLib.inst.sfx_general_speak);
					_anim_ct = 400;
				}
			} else if (_current_mode == Mode.Talk1) {
				scenes._cg1_char1_talk.SetActive(true);
				_anim_ct--;
				if (_anim_ct < 0) {
					_current_mode = Mode.End;
				}
			}

		}
		public override bool is_finished(CGScenes scenes){ return _current_mode == Mode.End; }
		public override void do_remove(CGScenes scenes){
			scenes._cg1.SetActive(false);
		}
	}

	private class CGScene2 : CGScene {

		private enum Mode {
			Trans0,
			Talk0,
			Trans1,
			End
		}
		private Mode _current_mode;
		private Vector3 _girl_start_pos, _girl_end_pos;
		private Vector3 _bg_start_pos, _bg_end_pos;
		private float _anim_ct = 0;

		public override void i_initialize(CGScenes scenes){
			scenes._cg2.SetActive(true);
			scenes._cg2_char0_talk.SetActive(false);
			_current_mode = Mode.Trans0;
			_girl_start_pos = new Vector3(989,-81,-50);
			_girl_end_pos = new Vector3(-91,-130,-50);
			_bg_start_pos = new Vector3(-388,0,0);
			_bg_end_pos = new Vector3(-35,0,0);
			_anim_ct = 0;
			scenes._cg2_char0.transform.localPosition = Vector3.Lerp(_girl_start_pos,_girl_end_pos,_anim_ct);
			scenes._cg2_bg.transform.localPosition = Vector3.Lerp(_bg_start_pos,_bg_end_pos,_anim_ct);
		}
		public override void i_update(CGScenes scenes){
			if (_current_mode == Mode.Trans0) {
				_anim_ct += 0.025f;
				scenes._cg2_char0.transform.localPosition = Vector3.Lerp(_girl_start_pos,_girl_end_pos,_anim_ct);
				scenes._cg2_bg.transform.localPosition = Vector3.Lerp(_bg_start_pos,_bg_end_pos,_anim_ct);
				if (_anim_ct >= 1) {
					_current_mode = Mode.Talk0;
					SFXLib.inst.play_sfx(SFXLib.inst.sfx_girl_late);
					_anim_ct = 400;
				}

			} else if (_current_mode == Mode.Talk0) {
				scenes._cg2_char0_talk.SetActive(true);
				_anim_ct--;
				if (_anim_ct <= 0) {
					scenes._cg2_char0_talk.SetActive(false);
					_current_mode = Mode.Trans1;
					_anim_ct = 0;
					_girl_start_pos = new Vector3(-91,-130,-50);
					_girl_end_pos = new Vector3(-829,-156,-50);
					_bg_start_pos = new Vector3(-35,0,0);
					_bg_end_pos = new Vector3(370,0,0);
				}

			} else if (_current_mode == Mode.Trans1) {
				_anim_ct += 0.025f;
				scenes._cg2_char0.transform.localPosition = Vector3.Lerp(_girl_start_pos,_girl_end_pos,_anim_ct);
				scenes._cg2_bg.transform.localPosition = Vector3.Lerp(_bg_start_pos,_bg_end_pos,_anim_ct);
				if (_anim_ct >= 1) {
					_current_mode = Mode.End;
				}
			}
		}
		public override bool is_finished(CGScenes scenes){ return _current_mode == Mode.End; }
		public override void do_remove(CGScenes scenes){
			scenes._cg2.SetActive(false);
		}
	}
}
