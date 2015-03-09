using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.IO;
using BeatProcessor;

public class GameMenu : MonoBehaviour {

	[NonSerialized] public SceneRef _sceneref;

	[SerializeField] public Image _cursor;
	[SerializeField] private Collider _ui_collider;
	[SerializeField] private GameObject _camera_anchor;

	[SerializeField] private UIButton _import_button;
	[SerializeField] private UniFileBrowserWrapper _file_browser;

	[SerializeField] private GameObject _home_menu;
	[SerializeField] private GameObject _prep_menu;
	[SerializeField] private GameObject _loading_menu;

	[SerializeField] private Text _file_desc;
	[SerializeField] private Text _connect_desc;

	private OnNextUpdater _on_next_update = new OnNextUpdater();

	public enum GameMenuMode {
		HomeMenu,
		FilePicker,
		PrepMenu,
		Loading
	}
	[NonSerialized] private GameMenuMode _current_mode;


	public void i_initialize(SceneRef sceneref) {
		_sceneref = sceneref;
		_import_button.i_initialize(_cursor.gameObject,()=>{
			_current_mode = GameMenuMode.FilePicker;
			_file_browser.pick_file((string filepath)=>{
				_current_mode = GameMenuMode.Loading;
				this.i_update();
				_on_next_update.CallOnNextUpdate(()=>{
					_on_next_update.CallOnNextUpdate(()=>{
						WavReader test = new WavReader(filepath);
						
						test.readWav();
						_sceneref._music.clip = test.getAudioClip();
						test.getBeatTimings();
						
						_sceneref._wav_reader = test;
						
						_file_desc.text = string.Format("file:\n{0}",filepath);
						_current_mode = GameMenuMode.PrepMenu;
					});
				});
			});
		});
		_current_mode = GameMenuMode.HomeMenu;
	}

	public void i_update() {
		_on_next_update.UpdateTick();
		if (_current_mode == GameMenuMode.FilePicker) {
			Cursor.visible = true;
			_ui_collider.gameObject.SetActive(false);

		} else {
			Cursor.visible = false;
			_ui_collider.gameObject.SetActive(true);

			Vector3 mouse_project = Util.vec_sub(_ui_collider.transform.position,_camera_anchor.transform.position);
			mouse_project.x += (Input.mousePosition.x - Screen.width/2.0f)/(Screen.width/2.0f);
			mouse_project.y += (Input.mousePosition.y - Screen.height/2.0f)/(Screen.height/2.0f);
			mouse_project.Normalize();
			
			
			Ray dir = new Ray(_camera_anchor.transform.position,mouse_project);
			RaycastHit hit_info;
			bool hit_found = _ui_collider.Raycast(dir,out hit_info,100);
			if (hit_found) {
				_cursor.transform.position = hit_info.point;
				
			}
			
			if (_current_mode == GameMenuMode.HomeMenu) {
				_home_menu.SetActive(true);
				_prep_menu.SetActive(false);
				_loading_menu.SetActive(false);

			} else if (_current_mode == GameMenuMode.PrepMenu) {
				_home_menu.SetActive(false);
				_prep_menu.SetActive(true);
				_loading_menu.SetActive(false);

				if (Input.GetKeyUp(KeyCode.Space)) {
					_sceneref._mode = SceneRef.SceneMode.GameEngine;
				}
			} else if (_current_mode == GameMenuMode.Loading) {
				_home_menu.SetActive(false);
				_prep_menu.SetActive(false);
				_loading_menu.SetActive(true);
			}
		}

		_connect_desc.text = string.Format("rift connected ({0})\nwiimote (connected:{1} / calibrated:{2})",Ovr.Hmd.Detect(),_sceneref._wii_model.connected_count(),_sceneref._wii_model.calibrated_count());
	}
}
