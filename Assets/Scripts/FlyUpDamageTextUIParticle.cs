using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlyUpDamageTextUIParticle : BaseUIParticle {
	public static string DAMAGE_TEXT = "UIParticles/FlyUpDamageTextUIParticle";
	
	[SerializeField] public Text _text;
	[SerializeField] public Gradient _gradient;
	[SerializeField] public Color _enemy_damage_color_top;
	[SerializeField] public Color _enemy_damage_color_bottom;
	[SerializeField] public Color _miss_color_top;
	[SerializeField] public Color _miss_color_bottom;
	[SerializeField] public Color _enemy_crit_color_top;
	[SerializeField] public Color _enemy_crit_color_bottom;
	
	public enum Mode {
		EnemyCrit,
		EnemyDamage,
		Miss
	};
	
	public override void i_initialize(GameUI game) {
	}
	
	private Vector3 _initial_pos, _final_pos;
	private static int CT_MAX = 50;
	private int _ct = 0;
	private float _scale_mult = 1.0f;
	private Mode _mode;
	public FlyUpDamageTextUIParticle start(Vector3 pos,int damage, Mode mode) { return start (pos,string.Format("{0}",damage),mode); }
	public FlyUpDamageTextUIParticle start(Vector3 pos,string text, Mode mode) {
		_text.text = text;
		_mode = mode;
		if (_mode == Mode.EnemyDamage) {
			_gradient.topColor = _enemy_damage_color_top;
			_gradient.bottomColor = _enemy_damage_color_bottom;
		} else if (_mode == Mode.Miss) {
			_gradient.topColor = _miss_color_top;
			_gradient.bottomColor = _miss_color_bottom;
		} else if (_mode == Mode.EnemyCrit) {
			_gradient.topColor = _enemy_crit_color_top;
			_gradient.bottomColor = _enemy_crit_color_bottom;
			_scale_mult = 1.5f;
		}
		_initial_pos = pos;
		_final_pos = Util.vec_delta(pos,0,100.0f);
		this.transform.position = _initial_pos;

		_ct = 0;
		
		return this;
	}
	
	public override void i_update(GameUI game) {
		float pct = ((float)_ct)/CT_MAX;
		this.transform.position = Vector3.Lerp(_initial_pos,_final_pos,pct);
		if (_mode == Mode.EnemyCrit) {
			if (pct < 0.35f) {
				float phase_pct = pct/0.35f;
				this.set_alpha(1);
				this.transform.localScale = Util.valv(_scale_mult*(0.75f*(1-phase_pct) + 1.0f));
				
			} else {
				float phase_pct = (pct-0.15f)/0.85f;
				this.set_alpha(1-phase_pct);
				this.transform.localScale = Util.valv(_scale_mult*(0.25f+0.75f*(1-phase_pct)));
			}
		} else {
			this.transform.localScale = Util.valv(0.5f+(1-pct)*0.5f);
			this.set_alpha(1-pct);
		}
		_ct++;
	}

	private void set_alpha(float val) {
		Color top = _gradient.topColor;
		Color bot = _gradient.bottomColor;
		top.a = val;
		bot.a = val;
		_gradient.topColor = top;
		_gradient.bottomColor = bot;
		_text.SetVerticesDirty();
	}

	public override bool should_remove(GameUI game) { 
		return _ct > CT_MAX; 
	}
	public override void do_remove(GameUI game) {}
}
