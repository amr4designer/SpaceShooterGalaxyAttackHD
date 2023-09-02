using UnityEngine;
using System.Collections;

public class TestSceneManager : MonoBehaviour {
	#region Fields & Properties
	// ----------------------------------------------------------------------------------------------------
	// Transforms
	[SerializeField]
	private Transform MainCannon;
	[SerializeField]
	private Transform LeftWing;
	[SerializeField]
	private Transform RightWing;

	// Main Cannon
	[SerializeField]
	private ParticleSystem[] MainCannonFireEffects;
	[SerializeField]
	private Light[] MainCannonFireLights;
	private int MainCannonSpeed;
	private float MainCannonOffset;

	// Engine
	[SerializeField]
	private Light[] EngineLights;

	// Wings
	private bool AreWingsSeparated = false;
	private float WingOffset;
	// ----------------------------------------------------------------------------------------------------
	#endregion
	
	#region Update Method
	// ----------------------------------------------------------------------------------------------------
	/// <summary>
	/// Updates this instance every tick.
	/// </summary>
	private void FixedUpdate () {
		// MainCannon offset
		if (this.MainCannonSpeed > 0) {
			this.MainCannonSpeed -= 1;
			
			this.MainCannonOffset += 2.5f;
			if (this.MainCannonOffset >= 6) {
				this.MainCannonSpeed = 0;
			}
		}
		if (this.MainCannonOffset > 0) {
			this.MainCannonOffset = Mathf.Max (this.MainCannonOffset - 0.30f, 0);
		}
		foreach (Light fireLight in this.MainCannonFireLights) {
			fireLight.intensity = Mathf.Pow (this.MainCannonOffset, 2) / 30.0f;
		}
		
		// Engine Lights
		float engineIntensity = 1.0f + (Mathf.PerlinNoise (Time.timeSinceLevelLoad * 10f, 0) * 1f);
		foreach (Light engineLight in this.EngineLights) {
			engineLight.intensity = engineIntensity;
		}
		
		// Animate Wings
		if (this.AreWingsSeparated) {
			this.WingOffset += 1.5f;
			if (this.WingOffset > 360) {
				this.WingOffset -= 360;
			}
			float realWingOffset = Mathf.Abs(Mathf.Sin (this.WingOffset * Mathf.Deg2Rad)) * 40;
			this.LeftWing.transform.localPosition = new Vector3 (-realWingOffset, 0, 0);
			this.RightWing.transform.localPosition = new Vector3 (realWingOffset, 0, 0);
		} else {
			if (this.LeftWing.transform.localPosition.x < 0) {
				this.LeftWing.transform.localPosition = new Vector3(this.LeftWing.transform.localPosition.x + 1f, 0, 0);
			}
			if (this.RightWing.transform.localPosition.x > 0) {
				this.RightWing.transform.localPosition = new Vector3(this.RightWing.transform.localPosition.x - 1f, 0, 0);
			}
		}
	}

	/// <summary>
	/// Updates this instance every frame.
	/// </summary>
	private void Update () {
		this.transform.Rotate (new Vector3 (0, 22.5f * Time.deltaTime, 0));
		
		this.MainCannon.localPosition = new Vector3 (0, 0, this.MainCannonOffset);
	}
	// ----------------------------------------------------------------------------------------------------
	#endregion
	
	#region Handle GUI
	// ----------------------------------------------------------------------------------------------------
	/// <summary>
	/// Handles the OnGUI event.
	/// </summary>
	private void OnGUI () {
		GUI.Box(new Rect(20, 20, 220, 64), "Ship Control");
		
		if (GUI.RepeatButton (new Rect (30, 50, 200, 20), "Fire Main Cannon") && this.MainCannonOffset < 2f) {
			this.MainCannonSpeed = 3;
			foreach (ParticleSystem fireEffect in this.MainCannonFireEffects) {
				fireEffect.Emit (10);
				fireEffect.Emit (Vector3.zero, Vector3.zero, 40, 0.1f, new Color32 (128, 128, 128, 255));
			}
		}
		
		if (GUI.Button (new Rect (30, 75, 200, 20), "Toggle Wing Separation")) {
			this.WingOffset = 0;
			this.AreWingsSeparated = !this.AreWingsSeparated;
		}
	}
	// ----------------------------------------------------------------------------------------------------
	#endregion
}
