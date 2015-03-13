using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class RepeatProto : MonoBehaviour {


	public void Start() {
	}

	public bool intersects(RepeatProto other) {
		return this.GetComponent<BoxCollider>().bounds.Intersects(other.GetComponent<BoxCollider>().bounds);
	}

	public void i_update_move() {
		Util.transform_position_delta(this.transform,new Vector3(0,0,-0.05f));
	}

	[SerializeField] private float _remove_back_dist;
	public bool should_remove() {
		float val = _remove_back_dist;
		if (val == 0) val = this.GetComponent<BoxCollider>().size.z;
		return this.transform.position.z < -val;
	}
}
