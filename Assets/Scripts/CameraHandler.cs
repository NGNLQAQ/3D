using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public Transform target;
    public Transform cameraTrans;
    public Transform cameraPivot;
    private Transform player;
    private Vector3 cameraPos;
    private LayerMask ignoreLayers;

    public static CameraHandler singleton;

    public float lookSpeed = 0.1f;
    public float followSpeed = 0.1f;
    public float pivotSpeed = 0.03f;

    private float targetPos;
    private float defaultPos;
    private float lookAngle;
    private float pivotAngle;
    public float minPivot = -35;
    public float maxPivot = 35;
    public float cameraRadius = 0.2f;
    public float cameraCollision = 0.2f;
    public float minimumCollision = 0.2f;

    private void Awake()
    {
        singleton = this;
        player = this.transform;
        defaultPos = cameraTrans.localPosition.z;
        ignoreLayers = ~2;
    }

    public void Follow(float delta)
    {
        Vector3 targetPosition = Vector3.Lerp(player.position, target.position, delta / followSpeed);
        player.position = targetPosition;

        CameraCollision(delta);
    }

    public void Rotation(float delta, float mouseX, float mouseY)
    {
        lookAngle += (mouseX * lookSpeed) / delta;
        pivotAngle -= (mouseY * pivotSpeed) / delta;
        pivotAngle = Mathf.Clamp(pivotAngle, minPivot, maxPivot);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        player.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void CameraCollision(float delta)
    {
        targetPos = defaultPos;
        RaycastHit hit;
        Vector3 direction = cameraTrans.position = cameraPivot.position;
        direction.Normalize();

        if(Physics.SphereCast(cameraPivot.position, cameraRadius, direction, out hit, Mathf.Abs(targetPos), ignoreLayers))
        {
            float dis = Vector3.Distance(cameraPivot.position, hit.point);
            targetPos = -(dis - cameraCollision);
        }
        if(Mathf.Abs(targetPos) < minimumCollision)
        {
            targetPos = -minimumCollision;
        }

        cameraPos.z = Mathf.Lerp(cameraTrans.localPosition.z, targetPos, delta / 0.2f);
        cameraTrans.localPosition = cameraPos;
    }
}
