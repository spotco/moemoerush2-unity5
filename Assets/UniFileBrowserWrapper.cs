using UnityEngine;
using System.Collections;
using System;

public class UniFileBrowserWrapper : MonoBehaviour {

	private enum UniFileBrowserWrapperMode {
		Open,
		Closed
	}
	private UniFileBrowserWrapperMode _current_mode = UniFileBrowserWrapperMode.Closed;
	private System.Action<string> _callback;

	public void pick_file(System.Action<string> callback) {
		_current_mode = UniFileBrowserWrapperMode.Open;
		_callback = callback;
	}

	public void OnGUI() {
		if (_current_mode == UniFileBrowserWrapperMode.Open) {
			UniFileBrowser.use.OpenFileWindow(OpenFile);
		}
	}

	void OpenFile (string pathToFile) {
		if (_callback != null) {
			_callback(pathToFile);
		}
		_callback = null;
		_current_mode = UniFileBrowserWrapperMode.Closed;
	}

}
