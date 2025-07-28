using UnityEngine;

public class SimpleMouseLook : MonoBehaviour
{
    // Controls how fast the camera moves with the mouse. Adjust in Inspector.
    [Header("Mouse Settings")]
    [Range(0.1f, 20f)]
    public float sensitivity = 5.0f;

    // Whether the mouse should start locked on launch.
    public bool lockOnStart = true;

    // Internals
    private bool cursorLocked = false;
    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        // Optionally lock the mouse on start
        if (lockOnStart)
        {
            LockCursor();
        }
    }

    void Update()
    {
        HandleMouseLockToggle();

        if (cursorLocked)
        {
            RotateWithMouse();
        }
    }

    /// <summary>
    /// Handles ESC to unlock and left-click to re-lock
    /// </summary>
    void HandleMouseLockToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }

        if (!cursorLocked && Input.GetMouseButtonDown(0))
        {
            LockCursor();
        }
    }

    /// <summary>
    /// Handles horizontal and vertical rotation while mouse is locked
    /// </summary>
    void RotateWithMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotationY += mouseX;
        rotationX -= mouseY;

        // Prevent flipping over top/bottom
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }

    /// <summary>
    /// Locks and hides the mouse
    /// </summary>
    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorLocked = true;
    }

    /// <summary>
    /// Unlocks and shows the mouse
    /// </summary>
    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorLocked = false;
    }
}
