using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLose : MonoBehaviour {
	public GameObject LevelObject;
	private LevelManager _Level;
	void Start() {
		if (LevelObject == null) {
			Debug.LogError("where is level manager, yo?");
			return;
		}
		_Level = LevelObject.GetComponent<LevelManager>();
	}
	void OnTriggerEnter2D(Collider2D ball) {
		if (ball.gameObject.tag != "ball") return;
		_Level.RegisterLoseBall();		
	}

}
