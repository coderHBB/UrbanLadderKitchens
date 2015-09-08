/***************************
Developer: Ravikiran T.A
Date: 07/09/2015
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
	[Header("Developer Set Values")]
	public float CameraFOV;
	[Tooltip("This value is used internally as Mathf.Tan(Mathf.Deg2Rad * CameraFOV / 2f)")]
	public float CameraFOVBasedValue;
	
	[Tooltip("How far in front of the zoomed object should the camera stop")]
	[Range(0.5f,10f)]public float distanceFromObjectOnZoom = 3f;
	
	[Tooltip("How high in front of the zoomed object should the camera stop")]
	[Range(0.5f,2f)]public float cameraHeightWithObjectOnZoom = 1f;
	
	[Tooltip("What angle should the camera view rotate with respect to selected object.This value is measured with respect to the front facing direction of the selected object")]
	[Range(0f,180f)]public float horizontalViewAngleOnObjectSelection = 180;
	
	[Tooltip("Height of the camera from the floor while zooming")]
	[Range(0f,3f)]public float initialCameraHeight = 1.8288f;

	
	[Header("Gameobject Refernces")]
	[Tooltip("Attach here the camera that is to be used for animation")]
	public GameObject animationCamera;
	
	private Vector3 originalRotation;
	
	#endregion
	
	
	void Awake(){
		//----- Event that will be triggered whenever an object with event trigger is selected
		
		ObjectInteractionClient.objectSelected += FocusCameraOnObject;
	}
	
	void Start () {
		Debug.Log("Getting Camera FOV");
		//-------Takes the camera FOV and calculates derived values to later use in camera distance calculation-----
		
		CameraFOV = animationCamera.GetComponent<Camera>().fieldOfView;
		CameraFOVBasedValue = Mathf.Tan(Mathf.Deg2Rad * CameraFOV / 2f);
		originalRotation = animationCamera.transform.rotation.eulerAngles;
	}
	
	#region FocusCameraOnObject
	//===== Used for direct call or focusing the camera on objects =====
	public void FocusCameraOnObject(GameObject selectedObject){
		
		//-----Get height and width of the selcted object------
		
		float widthOfObject = 1.5f;
		float heightOfObject = selectedObject.GetComponent<Collider>().bounds.extents.y;
		
		
		//-----Object width based calculation--------
		
		//float distance = (widthOfObject/2) / CameraFOVBasedValue;
		//float distance = distanceFromObjectOnZoom;
		
		
		//------Calculate camera Position-----------
		
		Vector3 position;
		position = selectedObject.transform.position + selectedObject.transform.forward * distanceFromObjectOnZoom;
		position = position + new Vector3(0f,cameraHeightWithObjectOnZoom,0f);
		iTween.MoveTo(animationCamera,position,2f);
		
		
		//-------Camera Rotation---------
		
		Vector3 rotation;
		rotation = new Vector3(selectedObject.transform.rotation.eulerAngles.x,selectedObject.transform.rotation.eulerAngles.y + horizontalViewAngleOnObjectSelection,selectedObject.transform.rotation.eulerAngles.z);
		iTween.RotateTo(animationCamera,rotation,2f);
	}
	#endregion
	
	#region CameraDistanceOnRoomSizeChange
	//===== Used to calculate camera position based on size of the room =====
	public void RecalculateCameraOnRoomChange(GameObject WidthWall){
	
		//------Calculate Distance to Move To---------
		
		float widthOfKitchen = RoomManager.Instance.topWall.lengthOfWall;
		float depthOfKitchen = RoomManager.Instance.leftWall.lengthOfWall;
		
		Debug.Log("WidthOfKitchen" + widthOfKitchen);
		Debug.Log("DepthOfKitchen" + depthOfKitchen);
		
		
		//--------Camera Position---------
		
		float distance = Mathf.Max((widthOfKitchen/2) / CameraFOVBasedValue,depthOfKitchen * 1.02f);
		
		Debug.Log("Distance:" + distance);
		
		
		//-----Reset Camera Position---------
		
		iTween.MoveTo(animationCamera,new Vector3(WidthWall.transform.position.x,initialCameraHeight,WidthWall.transform.position.z - distance),2f);
		
		
		//-----Reset Camera Rotation---------
		
		iTween.RotateTo(animationCamera,originalRotation,2f);
	}
	#endregion
}
