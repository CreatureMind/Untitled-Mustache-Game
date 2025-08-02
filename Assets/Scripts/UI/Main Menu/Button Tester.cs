using UnityEngine;

public class ButtonTester : MonoBehaviour
{
    public void OnButtonClick()
    {
#if UNITY_EDITOR
        Debug.Log("Button clicked!");
#endif
        // You can add more functionality here, like calling other methods or changing UI elements.
    }
}
