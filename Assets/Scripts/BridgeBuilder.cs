using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

/**
 * Detects Arms joint positions
 * Create and position bridge segments
 */
public class BridgeBuilder : MonoBehaviour {

	// prefab for bridge
	public Transform bridgeSegment;

	public float bridgeThickness = 0.3f;

	public GameObject BodySourceManager;
	private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();

	private BodySourceManager _BodyManager;

	// We only care about arm joints
	// TODO remove this if unused
	// private Kinect.JointType[] _joints = {
	// 	Kinect.JointType.HandTipLeft,
	// 	Kinect.JointType.HandLeft,
	// 	Kinect.JointType.WristLeft,
	// 	Kinect.JointType.ElbowLeft,
	// 	Kinect.JointType.ShoulderLeft,
	// 	Kinect.JointType.SpineShoulder,
	// 	Kinect.JointType.ShoulderRight,
	// 	Kinect.JointType.ElbowRight,
	// 	Kinect.JointType.WristRight,
	// 	Kinect.JointType.HandRight,
	// 	Kinect.JointType.HandTipRight
	// };
	private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
	{
		{ Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
		{ Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
		{ Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
		{ Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
		{ Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
		
		{ Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
		{ Kinect.JointType.HandRight, Kinect.JointType.WristRight },
		{ Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
		{ Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
		{ Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
	};

	void Start() {
		if (BodySourceManager == null) return;
		_BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
	}

	/*
	 * Updates body tracking data
	 * Updates segment position
	 */
	void Update () {
		if (BodySourceManager == null) return;
		if (_BodyManager == null) return;
		Kinect.Body[] data = _BodyManager.GetData();
		if (data == null) return;
		List<ulong> trackedIds = new List<ulong>();
		foreach(var body in data) {
			if (body == null) continue;
			if (!body.IsTracked) continue;
			trackedIds.Add(body.TrackingId);
		}
		List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

		// delete untracked bodies
		foreach(ulong trackingId in knownIds){
			if (!trackedIds.Contains(trackingId)) {
				Destroy(_Bodies[trackingId]);
				_Bodies.Remove(trackingId);
			}
		}
		// update bridges
		foreach(var body in data) {
			if (body == null) continue;
			if (!body.IsTracked) continue; // TODO is this really necessary? prev logic checks already
			if (!_Bodies.ContainsKey(body.TrackingId)) _Bodies[body.TrackingId] = CreateBodyBridge(body.TrackingId);
			RefreshBodyBridge(body, _Bodies[body.TrackingId]);
		}
	}

	private GameObject CreateBodyBridge(ulong id) {
		GameObject bodyBridge = new GameObject("Bridge:"+id);
		foreach(KeyValuePair<Kinect.JointType, Kinect.JointType> bone in _BoneMap) {
			Transform segment = SpawnSegment();
			segment.name = bone.ToString();
			segment.transform.parent = bodyBridge.transform;
		}
		return bodyBridge;
	}

	private void RefreshBodyBridge(Kinect.Body body, GameObject bodyBridge) {
		foreach(KeyValuePair<Kinect.JointType, Kinect.JointType> bone in _BoneMap) {
			Vector3 sourceJoint = GetVector3FromJoint2D(body.Joints[bone.Key]);
			Vector3 targetJoint = GetVector3FromJoint2D(body.Joints[bone.Value]);
			Transform segmentObj = bodyBridge.transform.Find(bone.ToString());
			// calculate position using the avg of two joints
			segmentObj.localPosition = (sourceJoint + targetJoint) / 2.0f;
			// calculate scale using the distance between two joints
			Vector3 boneDirection = (targetJoint - sourceJoint);
			float boneLength = boneDirection.magnitude;
			segmentObj.localScale = new Vector3(boneLength, bridgeThickness, 1);
			// calculate rotation using the angle between two joints
			float angle = Vector3.SignedAngle(boneDirection, Vector3.right, Vector3.back);
			segmentObj.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
	}

	private Transform SpawnSegment() {
		return Instantiate(bridgeSegment, transform.position, transform.rotation);
	}
	private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
	{
		return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
	}
	// get the joint positions, but with z set to 0, for a 2D render
	private static Vector3 GetVector3FromJoint2D(Kinect.Joint joint)
	{
		return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, 0);
	}
}
