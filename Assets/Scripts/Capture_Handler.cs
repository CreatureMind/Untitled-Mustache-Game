using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Capture_Handler : MonoBehaviour
{
    [SerializeField] private Camera screenshotCamera;
    
    public static int currentTextureIndex = 0;

    public static Action CaptureAction;

    private void Awake()
    {
        CaptureAction += StartTakeScreenshot;
    }

    private void StartTakeScreenshot()
    {
        StartCoroutine(TakeScreenshot());
    }

    private IEnumerator TakeScreenshot()
    {
        if (screenshotCamera == null)
        {
            Debug.LogError("Screenshot camera not assigned!");
            yield break;
        }

        yield return new WaitForEndOfFrame();

        currentTextureIndex = Level_Manager.Instance.currentLevelIndex;
        string path = $"{Application.persistentDataPath}/ScreenShots/Level_{currentTextureIndex}/map.png";

        // Create temporary RenderTexture with fixed resolution
        RenderTexture tempRT = new RenderTexture(1024, 768, 24);
        screenshotCamera.targetTexture = tempRT;
        screenshotCamera.Render();

        // Create and read the screenshot
        RenderTexture.active = tempRT;
        Texture2D screenshot = new Texture2D(tempRT.width, tempRT.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
        screenshot.Apply();

        // Save PNG
        byte[] data = screenshot.EncodeToPNG();
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
        System.IO.File.WriteAllBytes(path, data);

        Debug.Log($"Screenshot saved: {path}");

        // Cleanup
        screenshotCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(screenshot);
        Destroy(tempRT);
    }
    
    private void OnDestroy()
    {
        CaptureAction -= StartTakeScreenshot;
    }
}



// // Save the screenshot
// byte[] data = screenshot.EncodeToPNG();
// var path = Application.dataPath + "/Screenshots/Screenshot_" +
//            System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".png";
//
// // Ensure directory exists
// System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
// System.IO.File.WriteAllBytes(path, data);
//Debug.Log($"Added new screenshot {path}");
// Clean up