using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Scene_Loader : MonoBehaviour
{
    // Splash and Loading Screen GameObjects
    public CanvasGroup splashScreenPrefab; // Splash screen with CanvasGroup for fade-out
    public CanvasGroup loadingScreenPrefab; // Loading screen with CanvasGroup and progress bar
    public Slider progressBar;

    // Time settings
    public float splashScreenTime = 3f; // Duration of the splash screen in seconds
    public float fadeDuration = 1f; // Fade-out duration in seconds
    public float minimumLoadTime = 2f; // Minimum time to show the loading screen

    // Internals
    private static bool FirstRun { get; set; } = true;
    private bool _isSceneLoading = false;

    // private GameObject _activeSplashScreen;
    // private GameObject _activeLoadingScreen;
    // private GameObject _activeProgressBar;

    private void Awake()
    {
        // Ensure this GameObject persists across scenes
        DontDestroyOnLoad(gameObject);

        // Show the splash screen on the first run
        if (!FirstRun) return;
        FirstRun = false;
        ShowSplashScreen();
    }

    public void LoadScene(int sceneToLoad)
    {
        if (!_isSceneLoading)
        {
            StartCoroutine(LoadSceneWithTransition(sceneToLoad));
        }
    }

    private void ShowSplashScreen()
    {
        StartCoroutine(ShowSplashAndLoad());
    }

    private IEnumerator ShowSplashAndLoad()
    {
        // Wait for the splash screen duration
        yield return new WaitForSeconds(splashScreenTime);

        // Fade out the splash screen
        if (splashScreenPrefab)
            yield return StartCoroutine(FadeOutAndDestroy(splashScreenPrefab, fadeDuration));

        // Now load the main scene
        LoadScene(1);
    }


    private IEnumerator LoadSceneWithTransition(int sceneToLoad)
    {
        _isSceneLoading = true;

        if (loadingScreenPrefab)
        {
            loadingScreenPrefab.gameObject.SetActive(true);
            loadingScreenPrefab.alpha = 1f;
        }

        if (progressBar)
        {
            progressBar.value = 0f;
            progressBar.gameObject.SetActive(true);
        }

        // Start loading the scene asynchronously
        var asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncLoad.allowSceneActivation = false;

        // TODO: Add logic to start all menus while the scene is loading
        
        float elapsedTime = 0f;
        while (!asyncLoad.isDone)
        {
            elapsedTime += Time.deltaTime;

            if (progressBar)
            {
                // Calculate the current load percentage based on `asyncLoad.progress` and `minimumLoadTime`
                var sceneLoadProgress = asyncLoad.progress / 0.9f; // Normalize progress (0.0 - 0.9 mapped to 0.0 - 1.0)
                var timeProgress = Mathf.Clamp01(elapsedTime / minimumLoadTime);

                var overallProgress = Mathf.Max(sceneLoadProgress, timeProgress);

                // Smoothly animate the progress bar toward the calculated overall progress
                progressBar.value = Mathf.Lerp(progressBar.value, overallProgress, Time.deltaTime * 5f);
            }

            // Wait until the minimum load time and scene preparation are done
            if (elapsedTime >= minimumLoadTime && asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        // Wait for the fade-out transition before destroying the loading screen
        yield return StartCoroutine(FadeOutAndDestroy(loadingScreenPrefab, fadeDuration));

        _isSceneLoading = false;
    }

    private IEnumerator FadeOutAndDestroy(CanvasGroup screen, float duration)
    {
        if (!screen)
            yield break;

        //yield return new WaitForSeconds(duration);

        var elapsedTime = 0f;
        var startAlpha = screen.alpha;

        // Gradually fade the alpha to 0
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            screen.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / duration);
            yield return null;
        }

        screen.alpha = 0f; // Ensure it's fully transparent
        screen.gameObject.SetActive(false); // Clean up the screen
        progressBar.gameObject.SetActive(false); // Clean up the progress bar
    }
}