using System.IO;
using RenderHeads.Media.AVProVideo;
using SFB;
using UnityEngine;

public class VideoFileLoader : MonoBehaviour
{
    public MediaPlayer mediaPlayer;

    private SimpleMouseLook _mouseLook;

    private void Start()
    {
        _mouseLook = FindObjectOfType<SimpleMouseLook>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            OpenVideoFile();
        }
    }

    private void OpenVideoFile()
    {
        // Fix #2: null guard — fail loudly in editor, silently in build
        if (mediaPlayer == null)
        {
            Debug.LogError("VideoFileLoader: mediaPlayer is not assigned.");
            return;
        }

        // Fix #1: coordinate cursor unlock through SimpleMouseLook so its internal state stays in sync
        bool wasLocked = _mouseLook != null && Cursor.lockState == CursorLockMode.Locked;
        if (_mouseLook != null)
            _mouseLook.UnlockCursor();

        ExtensionFilter[] extensions = new ExtensionFilter[2]
        {
            new ExtensionFilter("Video Files", "mp4", "mov", "avi"),
            new ExtensionFilter("All Files", "*")
        };
        string[] result = StandaloneFileBrowser.OpenFilePanel("Open Video File", "", extensions, multiselect: false);

        if (result.Length != 0 && File.Exists(result[0]))
        {
            Debug.Log("VideoFileLoader: loading " + result[0]);
            mediaPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, result[0]);

            // Fix #3: play immediately after loading
            mediaPlayer.Play();
        }

        // Fix #1: restore cursor to its pre-dialog state
        if (_mouseLook != null && wasLocked)
            _mouseLook.LockCursor();
    }
}
