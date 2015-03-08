using UnityEngine;
using System;
using System.Collections.Generic;

public class RepeatInstance : MonoBehaviour {

	[SerializeField] private RepeatProto _proto;
	[SerializeField] private int _repeat_count;
	[SerializeField] private float _test_dist;
	[NonSerialized] private List<RepeatProto> _copies = new List<RepeatProto>();

	public void i_initialize(BattleGameEngine game) {
		_proto.gameObject.SetActive(false);
		if (_test_dist == 0) _test_dist = 0.25f;

		this.fill_copies();
	}

	private bool hit_any_others(RepeatProto test) {
		foreach(RepeatProto itr in _copies) {
			if (itr.intersects(test)) return true;
		}
		return false;
	}
	
	public void i_update(BattleGameEngine game) {
		for(int i = _copies.Count-1; i >= 0; i--) {
			RepeatProto itr = _copies[i];
			itr.i_update_move();
			if (itr.should_remove()) {
				_copies.RemoveAt(i);
				Destroy(itr.gameObject);
			}
		}
		this.fill_copies();
	}

	private void fill_copies() {
		while(_copies.Count < _repeat_count) {
			GameObject itr = Util.proto_clone(_proto.gameObject);
			RepeatProto itr_proto = itr.GetComponent<RepeatProto>();
			while(this.hit_any_others(itr_proto)) {
				Util.transform_position_delta(itr.transform,new Vector3(0,0,_test_dist));
			}
			_copies.Add(itr_proto);
		}
	}
}
