using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {


	// Update is called once per frame
	void Update () {
		transform.Translate(new  Vector3(0,0,0.4f*Time.deltaTime));
	}
}
