using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform Target;
    public float YOffset = 10.0f;
    private float _cameraZoom = 5.0f;
    private float _zoomMinimumOffset = 0.5f;
    private Camera _camera;
    private void Start()
    {
        _camera = gameObject.GetComponent<Camera>();
    }
    private void LateUpdate()
    {
        Vector3 newPosition = Target.position;
        newPosition.y += YOffset;
        transform.position = newPosition;

        Vector3 newEulerAngle = Target.eulerAngles;
        newEulerAngle.x = 90;
        newEulerAngle.z = 0;
        transform.eulerAngles = newEulerAngle;
    }
    public void ZoomIn()
    {
        if (_cameraZoom > 30.0f) return;
        _cameraZoom += _zoomMinimumOffset;
        _camera.orthographicSize = _cameraZoom;

    }
    public void ZoomOut()
    {
        if (_cameraZoom < 1.0f) return;
        _cameraZoom -= _zoomMinimumOffset;
        _camera.orthographicSize = _cameraZoom;
    }
}
