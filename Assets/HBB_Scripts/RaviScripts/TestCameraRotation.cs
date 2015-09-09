using UnityEngine;
using System.Collections;

public class TestCameraRotation : MonoBehaviour {
	
	public Vector2 startPoint;
	public Vector2 endPoint;
	
	public enum cameraState{
		Stationary,
		Rotating,
		Panning
	};
	public cameraState camState;
	
	
	void Start () {
		
	}
	
	IEnumerator CheckMouseInput(){
		while(true){
		
		}
	}
	
	void Rotate(){

	}
}
