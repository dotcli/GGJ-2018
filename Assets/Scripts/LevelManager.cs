using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	public Transform vehicle;
	public Transform spawnPoint;
	Camera cam;
	Vector3 vehicleScreenPos;

	// Use this for initialization
	void Start () {
		cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		keepVehicleInScene();
	}

	private void keepVehicleInScene() {
		vehicleScreenPos = cam.WorldToScreenPoint(vehicle.position);
		float x = (float)vehicleScreenPos.x / Screen.width;
		float y = (float)vehicleScreenPos.y / Screen.height;
		Debug.Log(x + " " + y);
		if (x > 1 || x < 0 || y > 1 || y < 0) {
			vehicle.position = spawnPoint.position;
			Debug.Log("RESET");
		}
	}
}
