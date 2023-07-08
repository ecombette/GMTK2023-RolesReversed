using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[ExecuteInEditMode]
public class ScreenShotRenderer : MonoBehaviour
{
    string _mainPath;
    [Range(1, 10)]
    public int ResolutionScale = 1;

    private void Start()
    {
        _mainPath = Application.persistentDataPath + "/ScreenShot";
    }

    [ContextMenu("Take A Screenshot")]
    public void TakeAScreenshot()
    {
        int fileCount = 0;

        if (Directory.Exists(_mainPath))
        {
            fileCount = Directory.GetFiles(_mainPath).Length;
            Capture("/Capture_" + fileCount +".png");
        }
        else
        {
            Directory.CreateDirectory(_mainPath);
            Capture("/Capture_0.png");
        }   
    }

    public void TakeAScreenshot(string name)
    {
        if (Directory.Exists(_mainPath))
        {
            Capture("/" + name + ".png");
        }
        else
        {
            Directory.CreateDirectory(_mainPath);
            Capture("/" + name + ".png");
        }
    }

    private void Capture(string name)
    {
        ScreenCapture.CaptureScreenshot(_mainPath + name, ResolutionScale);
        System.Diagnostics.Process.Start(_mainPath.Replace(@"/", @"\"));
    }

    [ContextMenu("Open Folder")]
    private void OpenFolder()
    {
        System.Diagnostics.Process.Start(_mainPath.Replace(@"/", @"\"));
    }
}
