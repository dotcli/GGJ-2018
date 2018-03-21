using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMovement : MonoBehaviour {

	public float Amount = 1f;
	public float Freq = 1f;
	private float timer = 0f;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		Debug.Log((1 - Mathf.Cos(timer * Freq)) * Amount);
		transform.localPosition = new Vector3(0f, (1 - Mathf.Cos(timer * Freq)) * Amount, 0f);
	}
}
