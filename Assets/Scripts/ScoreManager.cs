using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class ScoreManager : MonoBehaviour {
	[SerializeField] public GameObject _score;
	[SerializeField] public GameObject _health_bar;
	
	private int score;
	private int health;
	
	public void i_initialize(){
		score = 0;
		health = 100;
		this.gameObject.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		_score.GetComponent<EasyFontTextMesh> ().Text = "Score: " + score;
		_health_bar.GetComponent<Image> ().fillAmount = health / 100.0f;
	}
	
	public void hitSuccess(){
		score += 20;
		health += 1;
		if (health >= 100) {
			health = 100;
		}
	}
	
	public void hitFailure(){
		health -= 10;
		if (health <= 0) {
			health = 0;
		}
	}

	public bool isPlayerDead(){
		return (health <= 0);
	}

	public int getScore(){
		return this.score;
	}
}
