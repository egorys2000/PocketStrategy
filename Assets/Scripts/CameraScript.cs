using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Vector2 worldStartPoint;

    [SerializeField]
    private Camera MainCamera;

    [SerializeField]
    private UIManagerScript GameScreenController;

    IEnumerator CameraRoutine()
    {
        while (GameScreenController.PartyRunning) 
        {
            TouchCameraMove();

            MouseCameraMove();
            WheelZoom();

            yield return null;
        }
    }

    public void SwitchCamera(bool on) //switch on or switch off?
    {
        if (!on) { StopCoroutine(MainCoroutine); }
        else { StartCoroutine(MainCoroutine); }
    }

    private IEnumerator MainCoroutine;

    void Start()
    {
        MainCoroutine = CameraRoutine();
    }

    [SerializeField]
    private float panSpeed = 2, panSpeedAndroid = 0.5f, reactionTime = 0.5f;

    private static Vector2 StartTouchPos;
    private static Vector3 CameraInitialPos;
    private static float CameraStartMovingTime;

    private void TouchCameraMove() 
    {               
        // only work with one touch
        if (Input.touchCount == 1)
        {
            Touch currentTouch = Input.GetTouch(0);
            if (currentTouch.phase == TouchPhase.Began)
            {
                
                StartTouchPos = Input.GetTouch(0).position;

                CameraInitialPos = Camera.main.transform.position;
                CameraStartMovingTime = Time.time;
            }

            if (currentTouch.phase == TouchPhase.Moved)
            {
                
                Vector2 Delta = Input.GetTouch(0).position - StartTouchPos;

                Delta = Delta * panSpeedAndroid;

                //smoothly
                Vector2 ShiftCamera = Vector2.Lerp(Vector2.zero, Delta, (Time.time - CameraStartMovingTime) * 0.75f );

                if((Time.time - CameraStartMovingTime) <= reactionTime)
                Camera.main.transform.position = CameraInitialPos - (Vector3)ShiftCamera;
            }
        }
    }

    private void MouseCameraMove() 
    {
        if (Input.GetMouseButton(0)) // left mouse button
        {
            var newPosition = new Vector3();
            newPosition.x = Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime;
            newPosition.y = Input.GetAxis("Mouse Y") * panSpeed * Time.deltaTime;
            // translates to the opposite direction of mouse position.
            transform.Translate(-newPosition);
        }
    }

    [SerializeField]
    private float minFov = 15f, maxFov = 150f, sensitivity = 10f;
 
 private void WheelZoom()
    {
        float new_fov = Camera.main.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        new_fov = Mathf.Clamp(new_fov, minFov, maxFov);
        panSpeed = panSpeed * new_fov / Camera.main.fieldOfView ;
        
        Camera.main.fieldOfView = new_fov;
    }
}
