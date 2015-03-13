using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(EasyFontTextMesh))]
public class FadeInOutImage : MonoBehaviour {
	private enum Mode {
		Show, Hide
	};
	private Mode _current_mode;
	private EasyFontTextMesh _img;

	private float _current_alpha = 0;
	private float _current_scale = 1;
	private float _tar_alpha = 0;
	private float _tar_scale = 1;

	void Start () {
		_img = this.GetComponent<EasyFontTextMesh>();
		_current_mode = Mode.Hide;
		this.update_alpha(0);
	}

	bool _do_toggle = false;
	int _toggle_ct = 0;
	public void set_toggle() {
		_do_toggle = true;
		_current_alpha = 1;
		_tar_alpha = 1;
		_tar_scale = 1;
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
	}

	void update_alpha(float val) {
		_img.set_alpha(val);
	}
	void update_scale(float val) {
		this.transform.localScale = Util.valv(val);
	}
	
	public void show() {
		if (_current_mode != Mode.Show) {
			_current_alpha = 0.0f;
			_current_scale = 1.5f;
		}
		_current_mode = Mode.Show;
		_tar_alpha = 1.0f;
		_tar_scale = 1.0f;
	}
	public void hide() {
		_current_mode = Mode.Hide;
		_tar_alpha = 0.0f;
		_tar_scale = 1.5f;
	}
}
