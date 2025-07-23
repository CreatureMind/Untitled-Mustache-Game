using UnityEngine;
using UnityEngine.UI;
using CandyCoded.HapticFeedback;

public class Vibration_Manager : MonoBehaviour
{
    private static Vibration_Manager instance;
    public static Vibration_Manager Instance => instance;
    private Button[] allButtonsInGame;
     
    private bool isVibrationEnabled = true;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        AddListenersToAllButtons();
    }

    private void AddListenersToAllButtons()
    {
        allButtonsInGame = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None); // true to include inactive objects
        foreach (Button b in allButtonsInGame)
        {
            b.onClick.AddListener(HapticVibrate);
        }
    }

    private void HapticVibrate()
    {
        if (isVibrationEnabled)
        {
            HapticFeedback.MediumFeedback();
            Debug.Log("brr");
        }
    }
    
    public void NormalVibrate()
    {
        if (isVibrationEnabled)
        {
            Handheld.Vibrate();
            Debug.Log("brr brr");
        }
    }

    public void DisableVibration()
    {
        isVibrationEnabled = false;
        Debug.Log("brr OFF");
    }
    
    public void EnableVibration()
    {
        isVibrationEnabled = true;
        Debug.Log("brr ON");
    }
}
