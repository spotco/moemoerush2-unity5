using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SFXLib : MonoBehaviour {

	public static SFXLib inst;
	public void Start() {
		inst = this;
	}

	[SerializeField] public AudioClip sfx_hit;
	[SerializeField] public AudioClip sfx_hit_ok;
	[SerializeField] public AudioClip sfx_miss;
	[SerializeField] public AudioClip sfx_explosion;
	[SerializeField] public AudioClip sfx_lockon;

	public void play_sfx(AudioClip tar) {
		this.GetComponent<AudioSource>().PlayOneShot(tar);
	}
}
