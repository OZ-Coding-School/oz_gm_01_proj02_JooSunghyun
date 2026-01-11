using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float rotateSpeed = 10f;
    public float smoothTime = 0.3f;
    public float zoomSpeed = 5f;
    public float minZoom = 3f;
    public float maxZoom = 15f;
    public Vector2 xLimits = new Vector2(-50, 50);
    public Vector2 yLimits = new Vector2(-50, 50);
    public Vector2 zLimits = new Vector2(-50, 50);

    private Vector3 mVelosity = Vector3.zero;
    private Camera mCamera;
    private Vector3 mLastMousePos;
    private Transform mTarget;
    private Vector3 mTargetOffset = new Vector3(0, 10f, -10f);

    private void Start()
    {
        mCamera = Camera.main;
    }

    private void Update()
    {
        var mouse = Mouse.current;
        var keyboard = Keyboard.current;

        Vector3 moveDir = Vector3.zero;
        if ( keyboard.wKey.isPressed) { moveDir += Vector3.forward; }
        if (keyboard.sKey.isPressed) { moveDir += Vector3.back; }
        if (keyboard.aKey.isPressed) { moveDir += Vector3.left; }
        if (keyboard.dKey.isPressed) { moveDir += Vector3.right; }
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);

        //우클릭 카메라 회전
        if (mouse.rightButton.isPressed) 
        {
            Vector3 mouseDelta = mouse.delta.ReadValue();
            transform.Rotate(Vector3.up, mouseDelta.x * rotateSpeed * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, -mouseDelta.y * rotateSpeed * Time.deltaTime, Space.Self);
        }

        //줌 인, 아웃
        float scroll = mouse.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f) 
        {
            float dist = Vector3.Distance(mCamera.transform.position, transform.position);
            
            dist += scroll * zoomSpeed * Time.deltaTime;
            dist = Mathf.Clamp(dist, minZoom, maxZoom);

            if (scroll > 0)
            {
                mCamera.transform.position = transform.position + mCamera.transform.forward * dist;
            }
            else if(scroll < 0)
            {
                mCamera.transform.position = transform.position - mCamera.transform.forward * dist;
            }
        }
       
        //카메라 이동 범위 제한
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, xLimits.x, xLimits.y);
        pos.y = Mathf.Clamp(pos.y, xLimits.x, xLimits.y);
        pos.z = Mathf.Clamp(pos.z, zLimits.x, zLimits.y);
        transform.position = pos;

        mLastMousePos = mouse.position.ReadValue();
    }
}
