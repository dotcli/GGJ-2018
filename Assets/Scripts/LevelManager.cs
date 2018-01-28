using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	public GameObject vehicle;
	public GameObject[] levels;
	private int levelIndex = 0;
	private GameObject activeLevel;
	private GameObject _vehicle;
	Camera cam;
	Vector3 vehicleScreenPos = Vector3.zero;

	// Use this for initialization
	void Start () {
		cam = Camera.main;
		spawnLevel(0);
		spawnVehicle();
	}
	
	// Update is called once per frame
	void Update () {
		keepVehicleInScene();
	}

	private void destroyActiveLevel() {
		if (activeLevel == null) return;
		Destroy(activeLevel);
	}
	private void spawnLevel(int index) {
		activeLevel = Instantiate(levels[index]);
	}
	private void gotoNextLevel() {
		destroyActiveLevel();
		levelIndex ++;
		if (levelIndex >= levels.Length) levelIndex = 0; // TODO show info about having beat the game
		spawnLevel(levelIndex);
	}
	private void spawnVehicle() {
		_vehicle = Instantiate(vehicle, activeLevel.transform.Find("Spawn Point").position, Quaternion.identity);
	}
	private void destroyVehicle() {
		if (_vehicle == null) return;
		Destroy(_vehicle);
	}
	private void keepVehicleInScene() {
		if (_vehicle == null) return;
		vehicleScreenPos = cam.WorldToScreenPoint(_vehicle.transform.position);
		float x = (float)vehicleScreenPos.x / Screen.width;
		float y = (float)vehicleScreenPos.y / Screen.height;
		if (x > 1 || x < 0 || y > 1 || y < 0) {
			destroyVehicle();
			spawnVehicle();
		}
		// TODO determine victory
	}
}
