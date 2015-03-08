using UnityEngine;
using System.Collections;

public class FaceToCamera : MonoBehaviour {

	[SerializeField] GameObject obj;

	void Update () {
		if (float.IsNaN(obj.transform.position.x)) return;
		if (obj != null) this.transform.LookAt(obj.transform.position);
	}
}
