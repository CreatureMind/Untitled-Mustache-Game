using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Capture_Handler : MonoBehaviour
{
    [SerializeField] private Camera screenshotCamera;

    private int currentTextureIndex = 0;

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
#if UNITY_EDITOR
            Debug.LogError("Screenshot camera not assigned!");
#endif
            yield break;
        }

        yield return new WaitForEndOfFrame();

        currentTextureIndex = Level_Manager.Instance.currentLevelIndex;
        string path = $"{Profile_Menu.ProfilesPath}/{Game_Manager.ActiveProfile.nickname}/ScreenShots/Level_{currentTextureIndex}/map.png";

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
#if UNITY_EDITOR
        Debug.Log($"Screenshot saved: {path}");
#endif

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