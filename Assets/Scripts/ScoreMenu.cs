using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ScoreMenu : MonoBehaviour {
	[SerializeField] public GameObject _score_text;
	private int score;

	public void setScore(int score){
		this.score = score;
		_score_text.GetComponent<Text> ().text = "Score: " + score;
	}

}
