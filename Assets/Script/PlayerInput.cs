using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static readonly string ZMoveAxis = "Vertical";
    public static readonly string XMoveAxis = "Horizontal";
    public static readonly string RotateAxis = "Mouse X";
    public static readonly string FireButton = "Fire1";

    public float ZMove { get; private set; }
    public float XMove { get; private set; }
    public float Rotate { get; private set; }
    public bool Fire { get; private set; }

    private void Update()
    {
        ZMove = Input.GetAxis(ZMoveAxis);
        XMove = Input.GetAxis(XMoveAxis);
        Rotate = Input.GetAxis(RotateAxis);
        Fire = Input.GetButton(FireButton);
    }
}