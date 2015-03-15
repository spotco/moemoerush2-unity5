using UnityEngine;
using System.Collections;
using System;

public class AssaultPlatformEnemy : BaseEnemy {

	[SerializeField] private Animation _anim;
	[SerializeField] private GameObject _light;
	private int _light_flash_ct = 0;
	[SerializeField] private GameObject _body_anchor;

	public override void i_update(BattleGameEngine game) {
		float pos_y = this.transform.position.y;
		Vector3 tar_pos = Vector3.Lerp(_start_position,game._sceneref._player._explosion_anchor.transform.position,this.t());
		tar_pos.y = pos_y;
		this.transform.position = tar_pos;
		this.transform.LookAt(game._sceneref._player.transform.position);

		_light_flash_ct++;
		float target_duration = (1-this.t())*15+2;
		if (_light_flash_ct > target_duration) {
			_light.SetActive(!_light.activeSelf);
			_light_flash_ct = 0;
		}

		if (_end_time - DateTime.Now.ToFileTime() <= EnemyManager.HIT_TIME * EnemyManager.MS_TO_100NS && !_has_played_lockon_sound) {
			_has_played_lockon_sound = true;
			SFXLib.inst.play_sfx(SFXLib.inst.sfx_lockon);
		}

		_anim["Walk"].speed = 2.5f;
		_anim.playAutomatically = true;
		_anim.wrapMode = WrapMode.Loop;
		_anim.CrossFade("Walk");
	}
	
	public override void do_remove_killed(BattleGameEngine game) {
		game.add_particle(ParticleSystemWrapperParticle.EXPLOSION).set_position(this.transform.position);
		base.do_remove_killed(game);
		
	}
	public override void do_remove_hit_player(BattleGameEngine game) {
		game.add_particle(ParticleSystemWrapperParticle.EXPLOSION).set_position(game._sceneref._player._explosion_anchor.transform.position);
		base.do_remove_hit_player(game);
		
	}

	public override Vector3 get_center() {
		return _body_anchor.transform.position;
	}
	
}
