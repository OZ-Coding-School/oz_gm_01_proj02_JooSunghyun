using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    public float moveSpeed = 20f;
    public float rotateSpeed = 10f;
    public float smoothTime = 0.3f;
    public float zoomSpeed = 5f;
    public float minZoom = 3f;
    public float maxZoom = 15f;
    public Vector2 xLimits = new Vector2(-150, 150);
    public Vector2 yLimits = new Vector2(-100, 200);
    public Vector2 zLimits = new Vector2(-150, 150);

    private Vector3 mVelosity = Vector3.zero;
    private Camera mCamera;
    private Vector3 mLastMousePos;

    private List<Transform> mTargets = new List<Transform>();
    private List<Transform> mLastTargets = new List<Transform>();
    private Vector3 mTargetOffset = new Vector3(0, 140f, -80f);
    private bool mHasLockedOn = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mCamera = Camera.main;
    }

    private void Update()
    {
        MenualControl();
        if (!mHasLockedOn) 
        {
            FollowTargets();
        }
    }

    private void StartManual() 
    {
        mLastTargets.Clear();
        mLastTargets.AddRange(mTargets);
        ClearTargets();
    }

    private void MenualControl() 
    {
        var mouse = Mouse.current;
        var keyboard = Keyboard.current;

        Vector3 moveDir = Vector3.zero;
        if (keyboard.wKey.isPressed) { StartManual(); moveDir += Vector3.forward; }
        if (keyboard.sKey.isPressed) { StartManual(); moveDir += Vector3.back; }
        if (keyboard.aKey.isPressed) { StartManual(); moveDir += Vector3.left; }
        if (keyboard.dKey.isPressed) { StartManual(); moveDir += Vector3.right; }
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);

        //우클릭 카메라 회전
        if (mouse.rightButton.isPressed)
        {
            StartManual();
            Vector3 mouseDelta = mouse.delta.ReadValue();
            transform.Rotate(Vector3.up, mouseDelta.x * rotateSpeed * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, -mouseDelta.y * rotateSpeed * Time.deltaTime, Space.Self);
        }

        //줌 인, 아웃
        float scroll = mouse.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            StartManual();
            float dist = Vector3.Distance(mCamera.transform.position, transform.position);

            dist += scroll * zoomSpeed * Time.deltaTime;
            dist = Mathf.Clamp(dist, minZoom, maxZoom);

            if (scroll > 0)
            {
                mCamera.transform.position = transform.position + mCamera.transform.forward * dist;
            }
            else if (scroll < 0)
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

    private void FollowTargets() 
    {
        if (mTargets.Count == 0 || mHasLockedOn) return;

        Vector3 center = GetCenterPoint();
        Vector3 desiredPos = center + mTargetOffset;

        if (Vector3.Distance(transform.position, desiredPos) < 0.1f) 
        {
            transform.position = desiredPos;
            transform.LookAt(center);
            mHasLockedOn = true;
            return;
        }
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref mVelosity, smoothTime);
        transform.LookAt(center);
        //줌 조절
        float idleDist = GetGreatestDistance();
        float zoom = Mathf.Lerp(maxZoom, minZoom, idleDist / 150f);
        mCamera.fieldOfView = Mathf.Lerp(mCamera.fieldOfView, zoom, Time.deltaTime);
    }

    private Vector3 GetCenterPoint() 
    {
        if (mTargets.Count == 1) return mTargets[0].position;

        var bounds = new Bounds(mTargets[0].position, Vector3.zero);
        for (int i = 1; i < mTargets.Count; i++)
        {
            bounds.Encapsulate(mTargets[i].position);
        }
        return bounds.center;
    }

    private float GetGreatestDistance() 
    {
        var bounds = new Bounds(mTargets[0].position, Vector3.zero);
        for (int i = 1; i < mTargets.Count; i++) 
        {
            bounds.Encapsulate(mTargets[i].position);
        }
        return bounds.size.magnitude;
    }

    public void SetTargets(List<Transform> target) { mTargets = target; mHasLockedOn = false; }
    public void ClearTargets() { mTargets.Clear(); mHasLockedOn = false; }
}
