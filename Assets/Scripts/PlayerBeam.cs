using UnityEngine;
using System.Collections;

public class PlayerBeam : MonoBehaviour {
	[SerializeField] private SpriteRenderer _flare;

	private float _a = 0;
	public void Start() {
		set_alpha(0);
		_a = 0;
	}

	public void Update() {
		_a = Mathf.Clamp(_a-0.1f,0.0f,1.0f);
		set_alpha(_a);
	}

	private void set_alpha(float val) {
		this.GetComponent<LineRenderer>().SetColors(
			new Color(1.0f,1.0f,1.0f,val),
			new Color(1.0f,1.0f,1.0f,val)
		);
		Color c = _flare.color;
		c.a = val;
		_flare.color = c;
	}

	public void shoot() {
		_a = 1.0f;
		set_alpha(_a);
	}

}
