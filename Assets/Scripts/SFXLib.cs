using UnityEngine;
using System.Collections.Generic;
using System;

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
	[SerializeField] public AudioClip sfx_buzz;
	[SerializeField] public AudioClip sfx_shoot;
	[SerializeField] public AudioClip sfx_ready;
	[SerializeField] public AudioClip sfx_go;
	[SerializeField] public AudioClip sfx_cheer;
	[SerializeField] public AudioClip sfx_end_jingle;
	[SerializeField] public AudioClip sfx_itai;
	[SerializeField] public AudioClip sfx_game_end_voice;

	public void play_sfx(AudioClip tar) {
		this.GetComponent<AudioSource>().PlayOneShot(tar);
	}

	private struct EnqueuedSfx {
		public AudioClip _audio;
		public long _time;
	}
	private List<EnqueuedSfx> _frame_enqueued_sfx = new List<EnqueuedSfx>();
	private List<EnqueuedSfx> _enqueued_sfx = new List<EnqueuedSfx>();
	public void enqueue_sfx(AudioClip tar) {
		_frame_enqueued_sfx.Add(new EnqueuedSfx() {
			_audio = tar,
			_time = _frame_enqueued_sfx.Count * 5
		});
	}

	public void Update() {
		_enqueued_sfx.InsertRange(0,_frame_enqueued_sfx);
		_frame_enqueued_sfx.Clear();

		for (int i = _enqueued_sfx.Count-1; i >= 0; i--) {
			EnqueuedSfx itr = _enqueued_sfx[i];
			itr._time--;
			if (itr._time <= 0) {
				this.play_sfx(itr._audio);
				_enqueued_sfx.RemoveAt(i);
			}
		}
	}
}
