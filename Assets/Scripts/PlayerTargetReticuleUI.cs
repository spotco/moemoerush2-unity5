using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class PlayerTargetReticuleUI : MonoBehaviour {
	
	void Start () {
	
	}

	public void i_update(BattleGameEngine game, Ray dir) {
		RaycastHit hit_info;
		bool hit_found = game._sceneref._ui.GetComponent<Collider>().Raycast(dir,out hit_info,100);
		if (hit_found) {
			this.gameObject.SetActive(true);
			this.transform.position = hit_info.point;

		} else {
			this.gameObject.SetActive(false);
		}
	
	}

	void Update () {
		this.transform.Rotate(new Vector3(0,0,-5));
	}

	public SphereCollider get_collider() {
		return this.GetComponent<SphereCollider>();
	}
}
