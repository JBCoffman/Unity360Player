using System.IO;
using RenderHeads.Media.AVProVideo;
using SFB;
using UnityEngine;

public class VideoFileLoader : MonoBehaviour
{
    public MediaPlayer mediaPlayer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            OpenVideoFile();
        }
    }

    private void OpenVideoFile()
    {
        // Unlock cursor so the user can interact with the file dialog
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ExtensionFilter[] extensions = new ExtensionFilter[2]
        {
            new ExtensionFilter("Video Files", "mp4", "mov", "avi", "mkv"),
            new ExtensionFilter("All Files", "*")
        };
        string[] array = StandaloneFileBrowser.OpenFilePanel("Open Video File", "", extensions, multiselect: false);
        if (array.Length != 0 && File.Exists(array[0]))
        {
            string text = array[0];
            Debug.Log("Selected video path: " + text);
            mediaPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, text);
        }

        // Re-lock cursor after dialog closes
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
