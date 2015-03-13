using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IngameHUD : MonoBehaviour {
	[SerializeField] public EasyFontTextMesh _score;
	[SerializeField] public EasyFontTextMesh _combo;
	[SerializeField] public Image _health_bar;
	[SerializeField] private FadeInOutImage _low_health_overlay;
	[SerializeField] private FadeInOutImage _low_health_flash_text;

	public void i_initialize(){
		_low_health_flash_text.set_toggle();
	}

	public void i_update(BattleGameEngine game) {
		if (game._score._health <= ScoreManager.DAMAGE_PER_HIT*10) {
			_low_health_overlay.show();
			_low_health_flash_text.gameObject.SetActive(true);
		} else {
			_low_health_overlay.hide();
			_low_health_flash_text.gameObject.SetActive(false);
		}
		_score.Text = "Score: " + game._score._score;
		_combo.Text = "Combo: " + game._score._combo;
		_health_bar.fillAmount = ((float)game._score._health)/((float)ScoreManager.MAX_HEALTH);
	}
}
