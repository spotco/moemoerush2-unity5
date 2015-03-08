using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemWrapperParticle : BaseParticle {
	public static string EXPLOSION = "Particles/explosion";

	public override void i_initialize(BattleGameEngine game) {
	}
	public override void i_update(BattleGameEngine game) {
	}
	public override bool should_remove(BattleGameEngine game) { 
		return !this.GetComponent<ParticleSystem>().IsAlive(); 
	}
	public override void do_remove(BattleGameEngine game) {}
}
