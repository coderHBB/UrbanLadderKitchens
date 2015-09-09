/***************************
Developer: Ravikiran T.A
Created Date: 07/09/2015
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
	
	public enum cameraStatus{
		ViewMode,
		EditMode	
	};
	[Header("CameraStatus")]
	public cameraStatus camStatus;
	
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
	
	[Header("Camera Animation Speeds")]
	[Tooltip("Time taken to finish camera focus on object")]
	[Range(0.1f,3f)]public float cameraFocusOnObjectSpeed = 1f;
	
	[Tooltip("Time taken by camera to return to view mode")]
	[Range(0.1f,3f)]public float cameraResetToViewModeSpeed = 1f;
	
	[Tooltip("Time taken by camera to animate to new distance when room size changes")]
	[Range(0.1f,3f)]public float cameraReadjustmentSpeed = 1f;
	
	
	[Header("Gameobject Refernces")]
	[Tooltip("Attach here the camera that is to be used for animation")]
	public GameObject animationCamera;
	
	[Tooltip("Attach here the gameobject that is to be used by Tb Drag Orbit Script")]
	public GameObject targetObject;
	
	[Tooltip("Original Rotation Values before going to Edit Mode")]
	public Vector3 originalRotation;
	
	[Tooltip("Original Position Values before going to Edit Mode")]
	public Vector3 originalPosition;
	
	#endregion
	
	
	#region Events
	public delegate void EnterViewMode(float returnTime);
	
	//----- Used by camera manager to re enable camera rotation in TB Drag Orbit Script-----time parameter tells how long to wait for moving animation to finish----
	public static event EnterViewMode enableCameraRotation;
	#endregion
	
	#region Initialization
	void Awake(){
		//----- Event that will be triggered whenever an object with event trigger is selected
		ObjectInteractionClient.objectSelected += FocusCameraOnObject;
		UIManager.EnterViewMode += GoToViewMode;
	}
	
	void Start () {
		Debug.Log("Getting Camera FOV");
		
		//-------Takes the camera FOV and calculates derived values to later use in camera distance calculation-----
		//CameraFOV = animationCamera.GetComponent<Camera>().fieldOfView;
		CameraFOV = 60f;
		CameraFOVBasedValue = Mathf.Tan(Mathf.Deg2Rad * CameraFOV / 2f);
		originalRotation = animationCamera.transform.rotation.eulerAngles;
	}
	#endregion
	
	#region FocusCameraOnObject
	//===== Used for direct call or focusing the camera on objects =====
	public void FocusCameraOnObject(GameObject selectedObject){
		
		//-----Store original rotation before focusing--------
		StoreOriginalRotation();
		
		//----Store original position before focussing-------
		StoreOriginalPosition();
			
		
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
		iTween.MoveTo(animationCamera,position,cameraFocusOnObjectSpeed);
		
		
		//-------Camera Rotation---------
		Vector3 rotation;
		rotation = new Vector3(selectedObject.transform.rotation.eulerAngles.x,selectedObject.transform.rotation.eulerAngles.y + horizontalViewAngleOnObjectSelection,selectedObject.transform.rotation.eulerAngles.z);
		iTween.RotateTo(animationCamera,rotation,cameraFocusOnObjectSpeed);
		
		//----Change Camera status to Edit Mode----It is used as check before re-enabling tb orbit script----
		camStatus = cameraStatus.EditMode;
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
		iTween.MoveTo(targetObject,new Vector3(WidthWall.transform.position.x,initialCameraHeight,WidthWall.transform.position.z - distance),cameraFocusOnObjectSpeed);
		
		//-----Reset Camera Rotation---------
		//iTween.RotateTo(animationCamera,originalRotation,cameraFocusOnObjectSpeed);
	}
	#endregion
	
	#region ResetCameraToViewMode
	//======= Used to reset the camera position and rotation when exiting edit mode and returning to view mode ========
	void ResetCameraToViewMode(){
	
		//--- Use itween to set the position and rotation of the camera to the original values that were before going to edit mode----
		iTween.MoveTo(animationCamera,originalPosition,cameraResetToViewModeSpeed);
		iTween.RotateTo(animationCamera,originalRotation,cameraResetToViewModeSpeed);
		
		//----- Set camera status to View Mode--- 
		camStatus = cameraStatus.ViewMode;
	}
	#endregion
	
	#region StoreOriginalRotation
	//====== Used to store the original rotation when going from view mode to edit mode ======
	void StoreOriginalRotation(){
	
		//----- This will make sure that the value is stored only the first time when moving from view mode to edit mode even if this fuction is called all the time-----
		if(camStatus == cameraStatus.ViewMode){
			//originalRotation = transform.rotation;
			originalRotation = new Vector3(animationCamera.transform.rotation.eulerAngles.x,animationCamera.transform.rotation.eulerAngles.y,animationCamera.transform.rotation.eulerAngles.z);
		}
	}
	#endregion
	
	#region StoreOriginalPosition
	//===== Used to store the original position of the camera when going from view mode to edit mode
	void StoreOriginalPosition(){
	
		//----- This will make sure that the value is stored only the first time when moving from view mode to edit mode even if this fuction is called all the time-----
		if(camStatus == cameraStatus.ViewMode){
			originalPosition = new Vector3(animationCamera.transform.position.x,animationCamera.transform.position.y,animationCamera.transform.position.z);
		}
	}
	#endregion
	
	#region EnterViewMode
	//===== This function will be called from UI when View Mode Button is pressed=====
	public void GoToViewMode(){
		
		//--- This event trigger will be used to enable the rotation in the TB Drag Orbit script
		if(enableCameraRotation != null)
			enableCameraRotation(cameraResetToViewModeSpeed);
				
		ResetCameraToViewMode();
		
	}
	#endregion
}
