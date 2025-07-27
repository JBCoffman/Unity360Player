using UnityEngine;

public class SimpleMouseLook : MonoBehaviour
{
    public float sensitivity = 100f;
    private float xRotation = 0f;
    private Transform yawRoot;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        yawRoot = transform.parent;  // Assign parent as the horizontal rotator
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (yawRoot != null)
        {
            yawRoot.Rotate(Vector3.up * mouseX);
        }
    }
}
