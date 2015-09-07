/***************************
Developer: Ravikiran T.A
Date: 01/09/2015
Class Summary: Camera manager is responsible for camera animations and other camera operations in the application.
***************************/

using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	
	#region Singleton
	private static CameraManager instance;
	public static CameraManager Instance{
		get{
			if(instance == null)
				instance = GameObject.FindObjectOfType<CameraManager>();
			
			return instance;
		}
	}
	#endregion
	
	#region Variables
	public GameObject animationCamera;
	public float CameraFOV;
	public float CameraFOVBasedValue;
	
	#endregion
	
	void Awake(){
		ObjectInteractionClient.objectSelected += FocusCameraOnObject;
	}
	
	void Start () {
		Debug.Log("Getting Camera FOV");
		CameraFOV = animationCamera.GetComponent<Camera>().fieldOfView;
		CameraFOVBasedValue = Mathf.Tan(Mathf.Deg2Rad * CameraFOV / 2f);
	}
	
	
	void Update () {
		
	}
	
	//Used for direct call or focusing on objects
	public void FocusCameraOnObject(GameObject selectedObject){
		Debug.Log("Focusing on object");
		//Calculate Distance to Move To
		float widthOfObject = 1.5f;
		float heightOfObject = selectedObject.GetComponent<Collider>().bounds.extents.y;
		Debug.Log("WidthOfObject" + widthOfObject);
		float distance = (widthOfObject/2) / CameraFOVBasedValue;
		iTween.MoveTo(animationCamera,new Vector3(selectedObject.transform.position.x,selectedObject.transform.position.y + heightOfObject * 1.2f ,selectedObject.transform.position.z - distance),2f);
	}
	
	//Used to recalculate camera position when room is resized
	public void RecalculateCameraOnRoomChange(GameObject WidthWall){
		//Calculate Distance to Move To
		float widthOfKitchen = RoomManager.Instance.topWall.lengthOfWall;
		float depthOfKitchen = RoomManager.Instance.leftWall.lengthOfWall;
		
		Debug.Log("WidthOfKitchen" + widthOfKitchen);
		Debug.Log("DepthOfKitchen" + depthOfKitchen);
		
		float distance = Mathf.Max((widthOfKitchen/2) / CameraFOVBasedValue,depthOfKitchen * 1.02f);
		
		
		Debug.Log("Distance:" + distance);
		
		iTween.MoveTo(animationCamera,new Vector3(WidthWall.transform.position.x,1.8288f,WidthWall.transform.position.z - distance),2f);
	}
}
