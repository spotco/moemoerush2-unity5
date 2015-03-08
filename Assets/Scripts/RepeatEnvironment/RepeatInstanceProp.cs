using UnityEngine;
using System.Collections.Generic;
using System;

public class RepeatInstanceProp : MonoBehaviour {

	[SerializeField] public GameObject _repeat_every_object;
	[SerializeField] public int _repeat_every_count;
	[SerializeField] public List<GameObject> _props;

	public static int _ct = 0;

	public void Start() {
		if (!this.name.Contains("Clone")) return;
		if (RepeatInstanceProp._ct % _repeat_every_count == 0) {
			_repeat_every_object.SetActive(true);
			foreach(GameObject itr in _props) {
				itr.transform.parent = null;
				Destroy(itr);
			}
			_props.Clear();
		} else {
			_repeat_every_object.SetActive(false);
			GameObject tar = null;
			float rnd = Util.rand_range(0,100);
			if (rnd < 10) {
				tar = _props[0];
			} else if (rnd < 20) {
				tar = _props[1];
			} else if (rnd < 40) {
				tar = _props[2];
			} else if (rnd < 60) {
				tar = _props[3];
			}
			if (tar != null) {
				tar.SetActive(true);
			}


		}


		RepeatInstanceProp._ct++;
	}

}
