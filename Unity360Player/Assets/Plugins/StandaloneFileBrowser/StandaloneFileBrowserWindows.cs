using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Ookii.Dialogs;

namespace SFB
{
    public class WindowWrapper : IWin32Window
    {
        private IntPtr _hwnd;
        public IntPtr Handle => _hwnd;

        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }
    }

    public class StandaloneFileBrowserWindows : IStandaloneFileBrowser
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        public string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            var dialog = new VistaOpenFileDialog();
            dialog.Title = title;
            if (extensions != null)
            {
                dialog.Filter = GetFilterFromFileExtensionList(extensions);
                dialog.FilterIndex = 1;
            }
            else
            {
                dialog.Filter = string.Empty;
            }
            dialog.Multiselect = multiselect;
            if (!string.IsNullOrEmpty(directory))
            {
                dialog.FileName = GetDirectoryPath(directory);
            }
            string[] result = dialog.ShowDialog(new WindowWrapper(GetActiveWindow())) == DialogResult.OK
                ? dialog.FileNames
                : new string[0];
            dialog.Dispose();
            return result;
        }

        public void OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect, Action<string[]> cb)
        {
            cb(OpenFilePanel(title, directory, extensions, multiselect));
        }

        public string[] OpenFolderPanel(string title, string directory, bool multiselect)
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = title;
            if (!string.IsNullOrEmpty(directory))
            {
                dialog.SelectedPath = GetDirectoryPath(directory);
            }
            string[] result = dialog.ShowDialog(new WindowWrapper(GetActiveWindow())) == DialogResult.OK
                ? new string[] { dialog.SelectedPath }
                : new string[0];
            dialog.Dispose();
            return result;
        }

        public void OpenFolderPanelAsync(string title, string directory, bool multiselect, Action<string[]> cb)
        {
            cb(OpenFolderPanel(title, directory, multiselect));
        }

        public string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
        {
            var dialog = new VistaSaveFileDialog();
            dialog.Title = title;
            string fileName = "";
            if (!string.IsNullOrEmpty(directory))
                fileName = GetDirectoryPath(directory);
            if (!string.IsNullOrEmpty(defaultName))
                fileName += defaultName;
            dialog.FileName = fileName;
            if (extensions != null)
            {
                dialog.Filter = GetFilterFromFileExtensionList(extensions);
                dialog.FilterIndex = 1;
                dialog.DefaultExt = extensions[0].Extensions[0];
                dialog.AddExtension = true;
            }
            else
            {
                dialog.DefaultExt = string.Empty;
                dialog.Filter = string.Empty;
                dialog.AddExtension = false;
            }
            string result = dialog.ShowDialog(new WindowWrapper(GetActiveWindow())) == DialogResult.OK
                ? dialog.FileName
                : "";
            dialog.Dispose();
            return result;
        }

        public void SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions, Action<string> cb)
        {
            cb(SaveFilePanel(title, directory, defaultName, extensions));
        }

        private static string GetFilterFromFileExtensionList(ExtensionFilter[] extensions)
        {
            string filter = "";
            for (int i = 0; i < extensions.Length; i++)
            {
                filter += extensions[i].Name + "(";
                foreach (string ext in extensions[i].Extensions)
                    filter += "*." + ext + ",";
                filter = filter.Remove(filter.Length - 1);
                filter += ") |";
                foreach (string ext in extensions[i].Extensions)
                    filter += "*." + ext + "; ";
                filter += "|";
            }
            return filter.Remove(filter.Length - 1);
        }

        private static string GetDirectoryPath(string directory)
        {
            string path = Path.GetFullPath(directory);
            if (!path.EndsWith("\\"))
                path += "\\";
            if (Path.GetPathRoot(path) == path)
                return directory;
            return Path.GetDirectoryName(path) + Path.DirectorySeparatorChar;
        }
    }
}
