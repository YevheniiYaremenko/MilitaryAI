using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	[SerializeField] Vector2 cameraPositionLimits = new Vector2(50,50);
    [SerializeField] Vector2 cameraZoomLimits = new Vector2(50,80);
    [SerializeField] float pixelOffset = 10;
    [SerializeField] float cameraSpeed = .5f;
    [SerializeField] float zoomSpeed = 1f;

    void Update()
    {
        ChangeCameraPosition();
        ChangeCameraZoom();
        
        if (Input.GetMouseButtonUp(1))
        {
            OnClick();
        }
    }

    void ChangeCameraZoom()
    {
        float zoomAmplitude = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - zoomAmplitude * zoomSpeed, cameraZoomLimits.x, cameraZoomLimits.y);
    }

    void ChangeCameraPosition()
    {
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical")).normalized * cameraSpeed;
        var camPosition = Camera.main.transform.position;
        camPosition = new Vector3(
            Mathf.Clamp(camPosition.x + movement.x, -cameraPositionLimits.x / 2, cameraPositionLimits.x / 2),
            camPosition.y,
            Mathf.Clamp(camPosition.z + movement.y, -cameraPositionLimits.y / 2, cameraPositionLimits.y / 2));
        Camera.main.transform.position = camPosition;
    }

    void OnClick()
    {
        var hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (hits.Length>0)
        {
            foreach(var hit in hits)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    MainController.Instance.SetPosition(hit.point);
                }
            }
        }
    }
}
