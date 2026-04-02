using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Camera _camera;

    public Transform target;
    private Vector3 _offset;

    private Vector3 _currentVelocity = Vector3.zero;

    private void Awake()
    {
        _camera = Camera.main;
        _offset = _camera.transform.position - target.position;
    }

    private void LateUpdate()
    {
        _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, target.position + _offset, ref _currentVelocity, 0.2f);
    }
}