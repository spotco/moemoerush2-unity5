using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IngameHUD : MonoBehaviour {
	[SerializeField] public GameObject _score;
	[SerializeField] public GameObject _health_bar;
	
	public void i_initialize(){
	}

	public void i_update(BattleGameEngine game) {
		_score.GetComponent<EasyFontTextMesh> ().Text = "Score: " + 0;
		_health_bar.GetComponent<Image> ().fillAmount = 50.0f / 100.0f;
	}
}
