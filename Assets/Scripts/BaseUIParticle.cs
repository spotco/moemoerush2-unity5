using UnityEngine;
using System.Collections;

public class BaseUIParticle : MonoBehaviour {
	public virtual void i_initialize(GameUI ui) {}
	public virtual void i_update(GameUI ui) {}
	public virtual bool should_remove(GameUI ui) { return true; }
	public virtual void do_remove(GameUI ui) {}

	public RectTransform rect() { return this.GetComponent<RectTransform>(); }
}
