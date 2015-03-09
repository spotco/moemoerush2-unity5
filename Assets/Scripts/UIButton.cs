using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(BoxCollider))]
public class UIButton : MonoBehaviour {

	private GameObject _cursor;
	private System.Action _callback;

	public void i_initialize(GameObject cursor, System.Action callback) {
		_cursor = cursor;
		_callback = callback;
	}

	private float _target_scale = 1.0f;
	void Update () {
		if (_cursor == null) return;
		if (_cursor.GetComponent<BoxCollider>().bounds.Intersects(this.GetComponent<BoxCollider>().bounds)) {
			_target_scale = 1.5f;
			if (Input.GetMouseButtonUp(0)) {
				_callback();
			}
		} else {
			_target_scale = 1.0f;
		}
		_target_scale = Util.drp(this.transform.localScale.x,_target_scale,0.5f);
		this.transform.localScale = Util.valv(_target_scale);
	}
}
