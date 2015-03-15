using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeInOutImage : MonoBehaviour {
	private enum Mode {
		Show, Hide
	};
	private Mode _current_mode;
	private EasyFontTextMesh _text;
	private Image _img;

	private float _current_alpha = 0;
	private float _current_scale = 1;
	private float _tar_alpha = 0;
	private float _tar_scale = 1;
	[SerializeField] private float _scale_mult;
	public float get_scale_mult() {
		if (_scale_mult == 0) {
			return 1;
		} else {
			return _scale_mult;
		}
	}

	void Start () {
		_current_scale *= get_scale_mult();
		_text = this.GetComponent<EasyFontTextMesh>();
		_img = this.GetComponent<Image>();
		_current_mode = Mode.Hide;
		this.update_alpha(0);
	}

	bool _do_toggle = false;
	int _toggle_ct = 0;
	public void set_toggle() {
		_do_toggle = true;
		_current_alpha = 1;
		_tar_alpha = 1;
		_tar_scale = 1 * get_scale_mult();
	}

	void Update () {
		if (_do_toggle) {
			_toggle_ct++;
			if (_toggle_ct % 20 == 0) {
				this.GetComponent<MeshRenderer>().enabled = !this.GetComponent<MeshRenderer>().enabled;
			}
		}

		_current_alpha = Util.drp(_current_alpha,_tar_alpha,0.25f);
		_current_scale = Util.drp(_current_scale,_tar_scale,0.25f);
		this.update_alpha(_current_alpha);
		this.update_scale(_current_scale);
		if (_hold_ct > 0) {
			_hold_ct--;
			if (_hold_ct <= 0) {
				this.hide();
			}
		}
	}

	void update_alpha(float val) {
		if (_text != null) _text.set_alpha(val);
		if (_img != null) _img.color = new Color(1,1,1,val);
	}
	void update_scale(float val) {
		this.transform.localScale = Util.valv(val);
	}

	private int _hold_ct = -1;
	public void show(int hold_ct = -1) {
		if (_current_mode != Mode.Show) {
			_current_alpha = 0.0f;
			_current_scale = 1.5f * get_scale_mult();
		}
		_hold_ct = hold_ct;
		_current_mode = Mode.Show;
		_tar_alpha = 1.0f;
		_tar_scale = 1.0f * get_scale_mult();
	}
	public void hide() {
		_current_mode = Mode.Hide;
		_tar_alpha = 0.0f;
		_tar_scale = 1.5f * get_scale_mult();
	}
}
