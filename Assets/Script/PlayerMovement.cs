using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static readonly int hashMoveSpeed = Animator.StringToHash("Move");

    public float MoveSpeed = 10f;

    private Camera _playerCamera;
    private PlayerInput _playerInput;
    private Animator _playerAnimator;
    private Rigidbody _playerRigidbody;

    private void Awake()
    {
        _playerCamera = Camera.main;
        _playerInput = GetComponent<PlayerInput>();
        _playerAnimator = GetComponent<Animator>();
        _playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float totalMove = new Vector2(_playerInput.XMove, _playerInput.ZMove).magnitude;
        _playerAnimator.SetFloat(hashMoveSpeed, totalMove);
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection = (Vector3.forward * _playerInput.ZMove) + (Vector3.right * _playerInput.XMove);
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        Vector3 nextPosition = _playerRigidbody.position + moveDirection * MoveSpeed * Time.fixedDeltaTime;
        _playerRigidbody.MovePosition(nextPosition);

        Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Vector3 lookTarget = new Vector3(point.x, transform.position.y, point.z);

            transform.LookAt(lookTarget);
        }
    }
}