using UnityEngine;
using System.Collections;

public class MainCam : MonoBehaviour
{
		public bool follow = true;

		//The target of the camera. The camera will always point to this object.
		public Transform _target;
		
		//The default altitude of the camera from the target.
		public float _yadd = 10.0f;

		//The default distance of the camera from the target.
		public float _distance = 25.0f;
		
		//Control the speed of zooming and dezooming.
		public float _zoomStep = 1.0f;
		
		//The speed of the camera. Control how fast the camera will rotate.
		public float _xSpeed = 1f;
		public float _ySpeed = 1f;
		
		//The position of the cursor on the screen. Used to rotate the camera.
		private float _x = 0.0f;
		private float _y = 0.0f;
		
		//Distance vector. 
		private Vector3 _distanceVector;

 //Move the camera to its initial position.

		void Start ()
		{
		_distanceVector = new Vector3(0.0f,_yadd,-_distance);
			
			Vector2 angles = this.transform.localEulerAngles;
			_x = angles.x;
			_y = angles.y;
			
			this.Rotate(_x, _y);
			
		}
		
  // Rotate the camera or zoom depending on the input of the player.

		void LateUpdate()
		{
			if ( _target )
			{
				this.RotateControls();
				this.Zoom();
			}
		}
		
	void OnGUI ()
	{

		if (GUI.Button (new Rect (115,40,80,20), "Reset")) Application.LoadLevel(0);
		
		if (GUI.Button (new Rect (5,0,80,20), "Fullscreen"))
		{
			Screen.fullScreen = !Screen.fullScreen; 
		}

		if (follow == true)
		{
			if (GUI.Button (new Rect (115, 0, 80, 20), "Follow ON"))
				follow = false;
		}
		else
		{
			GUI.color = Color.red;
			if (GUI.Button (new Rect (115, 0, 80, 20), "Follow OFF"))
				follow = true;
		}
	}



  // Rotate the camera when the first button of the mouse is pressed.

		void RotateControls()
		{
			if(Input.GetKey(KeyCode.Mouse2))
			{
				_x += Input.GetAxis("Mouse X") * _xSpeed;
				_y += -Input.GetAxis("Mouse Y")* _ySpeed;
				
				
			}
		if(follow == true) this.Rotate(_x,_y);
		}
		

  //Transform the cursor mouvement in rotation and in a new position
  // for the camera.

		void Rotate( float x, float y )
		{
			//Transform angle in degree in quaternion form used by Unity for rotation.
			Quaternion rotation = Quaternion.Euler(y,x,0.0f);
			
			//The new position is the target position + the distance vector of the camera
			//rotated at the specified angle.
			Vector3 position = rotation * _distanceVector + _target.position;
			
			//Update the rotation and position of the camera.
			transform.rotation = rotation;
			transform.position = position;
		}
		
		
  // Zoom or dezoom depending on the input of the mouse wheel.

		void Zoom()
		{
			if ( Input.GetAxis("Mouse ScrollWheel") < 0.0f )
			{
				this.ZoomOut();
			}
			else if ( Input.GetAxis("Mouse ScrollWheel") > 0.0f )
			{
				this.ZoomIn();
			}
			
		}
		
  // Reduce the distance from the camera to the target and
  // update the position of the camera (with the Rotate function).

		void ZoomIn()
		{
			_distance -= _zoomStep;
		_distanceVector = new Vector3(0.0f,_yadd,-_distance);
			this.Rotate(_x,_y);
		}
		
  // Increase the distance from the camera to the target and
  // update the position of the camera (with the Rotate function).

		void ZoomOut()
		{
			_distance += _zoomStep;
		_distanceVector = new Vector3(0.0f,_yadd,-_distance);
			this.Rotate(_x,_y);
		}
		
	
}
