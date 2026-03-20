using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class SimpleMouseLook : MonoBehaviour
{
    [Header("Mouse Settings")]
    [Range(0.1f, 20f)]
    public float sensitivity = 5.0f;

    [Header("Zoom Settings")]
    [Range(1f, 179f)]
    public float minFOV = 30f;
    public float maxFOV = 90f;
    public float zoomSpeed = 10f;

    [Header("Control Settings")]
    public bool lockOnStart = true;

    [Header("Optional AVPro")]
    public MediaPlayer mediaPlayer;  // Drag your AVPro MediaPlayer here

    private bool cursorLocked = false;
    private float rotationX = 0f;
    private float rotationY = 0f;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;

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
        //Zoom in/out 
        HandleZoom();
        //Spacebar for pause/resume 
        HandlePlaybackToggle();
        if (mediaPlayer != null)
{
    // ⬅ Skip backward 3 seconds
    if (Input.GetKeyDown(KeyCode.LeftArrow))
    {
        double newTime = mediaPlayer.Control.GetCurrentTime() - 3.0;
        mediaPlayer.Control.Seek(Mathf.Max(0f, (float)newTime));
    }

    // ➡ Skip forward 3 seconds
    if (Input.GetKeyDown(KeyCode.RightArrow))
    {
        double newTime = mediaPlayer.Control.GetCurrentTime() + 3.0;
        double duration = mediaPlayer.Info.GetDuration();
        mediaPlayer.Control.Seek((float)Mathf.Min((float)newTime, (float)duration));
    }
}

    }

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

    void RotateWithMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }

    void HandleZoom()
    {
        if (cam == null) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            cam.fieldOfView -= scroll * zoomSpeed;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
        }
    }

    void HandlePlaybackToggle()
    {
        if (mediaPlayer != null && Input.GetKeyDown(KeyCode.Space))
        {
            if (mediaPlayer.Control.IsPlaying())
            {
                mediaPlayer.Pause();
            }
            else
            {
                mediaPlayer.Play();
            }
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorLocked = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorLocked = false;
    }
}
