using UnityEngine;
using System.Collections;
using System;

public class MissileEnemy : BaseEnemy {

	[SerializeField] private GameObject _light;
	private int _light_flash_ct = 0;
	
	public override void i_update(BattleGameEngine game) {
		base.i_update(game);
		_light_flash_ct++;
		float target_duration = (1-this.t())*15+2;
		if (_light_flash_ct > target_duration) {
			_light.SetActive(!_light.activeSelf);
			_light_flash_ct = 0;
		}
	}

	public override void do_remove_killed(BattleGameEngine game) {
		game.add_particle(ParticleSystemWrapperParticle.EXPLOSION).set_position(this.transform.position);

	}
	public override void do_remove_hit_player(BattleGameEngine game) {
		game.add_particle(ParticleSystemWrapperParticle.EXPLOSION).set_position(game._sceneref._player._explosion_anchor.transform.position);

	}

}
