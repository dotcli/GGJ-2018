using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	public GameObject vehicle;
	public float levelWaitTime = 10.0f;
	public float endWaitTime = 10.0f;
	public float vehicleFrequency = 2.0f;
	public int vehicleAmount = 20;
	public GameObject[] levels;
	private int levelIndex = 0;
	private GameObject activeLevel;
	private List<GameObject> _vehicles = new List<GameObject>();
	Camera cam;
	Vector3 vehicleScreenPos = Vector3.zero;
	float timer = 0;

	private int winBall = 0;
	private int loseBall = 0;
	public string StrScore = "";
	private enum LevelState {
		Wait,
		Ongoing,
		Ended,
	}
	LevelState state;

	// Use this for initialization
	void Start () {
		cam = Camera.main;
		spawnLevel(0);
		state = LevelState.Wait;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		switch (state)
		{
			case LevelState.Wait:
				if (timer > levelWaitTime) {
					startLevel();
					timer = 0;
				}
				break;
			case LevelState.Ongoing:
				if (timer > vehicleFrequency) {
					if (_vehicles.Count < vehicleAmount) {
						spawnVehicle();
					}
					timer = 0;
				}
				// NOTE to move on from here, a trigger keeps track of score
				break;
			case LevelState.Ended:
				if (timer > endWaitTime) {
					gotoNextLevel();
					timer = 0;
				}
				break;
			default:
				Debug.LogWarning("Unknown state " + state);
				break;
		}
		// Debug.Log(timer);
	}

	private void destroyActiveLevel() {
		if (activeLevel == null) return;
		Destroy(activeLevel);
	}
	private void spawnLevel(int index) {
		activeLevel = Instantiate(levels[index]);
		resetScore();
	}
	private void resetScore() {
		winBall = 0;
		loseBall = 0;
		StrScore = "";
	}
	private void gotoNextLevel() {
		destroyAllVehicles();
		destroyActiveLevel();
		levelIndex ++;
		if (levelIndex >= levels.Length) levelIndex = 0; // TODO show info about having beat the game
		spawnLevel(levelIndex);
		state = LevelState.Wait;
	}
	// start the level after initial wait's over
	private void startLevel() {
		state = LevelState.Ongoing;
	}
	private void spawnVehicle() {
		GameObject _vehicle = Instantiate(vehicle, activeLevel.transform.Find("Spawn Point").position, Quaternion.identity);
		// Debug.Log("vehicle spawned at " + _vehicle.transform.position, _vehicle);
		_vehicles.Add(_vehicle);
	}
	private void destroyAllVehicles() {
		foreach (GameObject v in _vehicles) {
			Destroy(v);
		}
		_vehicles.Clear();
	}
	private void checkVehicleSuccess() {
		// TODO determine victory

		// TODO old code for ref
		// if (_vehicle == null) return;
		// vehicleScreenPos = cam.WorldToScreenPoint(_vehicle.transform.position);
		// float x = (float)vehicleScreenPos.x / Screen.width;
		// float y = (float)vehicleScreenPos.y / Screen.height;
		// if (x > 1 || x < 0 || y > 1 || y < 0) {
		// 	destroyVehicle();
		// 	spawnVehicle();
		// }
	}

	public void RegisterWinBall() {
		Debug.Log("Good ball");
		winBall += 1;
		checkLevelCompletion();
	}
	public void RegisterLoseBall() {
		Debug.Log("Bad ball");
		loseBall += 1;
		checkLevelCompletion();
	}
	private void checkLevelCompletion() {
		if (winBall + loseBall < vehicleAmount) return;
		StrScore = winBall + "/" + vehicleAmount;
		Debug.Log("Score: " + StrScore);
		state = LevelState.Ended;
	}
}
