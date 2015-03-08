using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ImportMenu : MonoBehaviour {
	[SerializeField] public GameObject _message;
	private const string IMPORT_TEXT = "Please press \"O\" to pick a mp3/wav file";
	private const long errorDisplayInterval = 5000L;
	private long errorDisplayTime = 0;

	public void wrongFileTypeMessage(){
		this.displayErrorMessage ("Only support *.mp3/*.wav files. (*´∀`*)");
	}

	public void successMessage() {
		this.displayErrorMessage ("Import Success! Ｏ(≧▽≦)Ｏ");
	}

	public void fileNotFoundMessage(){
		this.displayErrorMessage ("File Not Found! (ﾉ｀Д´)ﾉ");
	}

	private void displayErrorMessage(string text){
		_message.GetComponent<Text> ().text = text;
		errorDisplayTime = DateTime.Now.ToFileTime ();
	}

	void Update(){
		if (errorDisplayTime != 0) {
			long interval = DateTime.Now.ToFileTime() - errorDisplayTime;
			if(interval >= errorDisplayInterval * 10000L){
				_message.GetComponent<Text>().text = IMPORT_TEXT;
				errorDisplayTime = 0;
			}
		}
	}
}
