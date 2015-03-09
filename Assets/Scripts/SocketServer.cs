using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using UnityEngine;

/*
JSONObject obj = new JSONObject();
					obj.Add("action","rumble");
					obj.Add("id",Convert.ToInt32(jason.GetNumber("id")));
					obj.Add("duration",0.001f);
					enqueue_msg_to_send(obj.ToString());

*/

public class SocketServer : MonoBehaviour {
	private SceneRef _sceneref;
	private CommunicatorServer _socket;
	private OnNextUpdater _on_next_update = new OnNextUpdater();

	public void i_initialize(SceneRef sceneref) {
		_sceneref = sceneref;

		_socket = new CommunicatorServer(7001,100,(string val)=>{
			try {
				JSONObject jason = JSONObject.Parse(val);
				string type = jason.GetString("t");
				_on_next_update.CallOnNextUpdate(()=>{
					if (type == "b") {
						string button = jason.GetString("b");
						int id = Convert.ToInt32(jason.GetNumber("id"));
						if (button == "B") {
							_sceneref.game().player_shoot(id);
						}
						
					} else if (type == "m") {
						_sceneref._wii_model.wmp_report(jason);
					} else if (type == "c") {
						_sceneref._wii_model.wiimote_connect(jason);
					} else if (type == "d") {
						_sceneref._wii_model.wiimote_disconnect(jason);
					} else if (type == "a") {
						_sceneref._wii_model.accel_report(jason);
					} else if (type == "i") {
						_sceneref._wii_model.ir_report(jason);
					}
				});
			} catch {
				Debug.LogError("MALFORMED JSON");
			}
		}, ()=> {
			Debug.Log ("unity connect message sent");
			JSONObject obj = new JSONObject();
			obj.Add("action","unity_connect");
			enqueue_msg_to_send(obj.ToString());
		});
	}
	
	void OnApplicationQuit() {
		JSONObject obj = new JSONObject();
		obj.Add("action","unity_disconnect");
		enqueue_msg_to_send(obj.ToString());
		_socket.OnApplicationQuit();
	}
	
	public void enqueue_msg_to_send(string msg) { 
		_socket.enqueue_msg_to_send(msg);
	}

	void Update () {
		if (Input.GetKey(KeyCode.F1)) {
			JSONObject obj = new JSONObject();
			obj.Add("action","find_wiimote");
			this.enqueue_msg_to_send(obj.ToString());
		}
		_on_next_update.UpdateTick();
	}
}
