using UnityEngine;


public abstract class Base_Menu : MonoBehaviour
{
    [SerializeField] protected MenuState menuState;
    public MenuState MenuState => menuState;
    [SerializeField] protected CanvasGroup canvasGroup;
    
    protected bool isUpdated = false;
    
    public virtual void Show()
    {
        //gameObject.SetActive(true);
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        OnMenuOpen();
    }

    public virtual void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        //gameObject.SetActive(false);
        OnMenuClose();
    }

    public virtual void Initialize()
    {
<<<<<<< HEAD
        isUpdated = true;
=======
        
>>>>>>> parent of d63a79e (Revert "wip")
    }

    protected virtual void OnMenuOpen() { }
    protected virtual void OnMenuClose() { }
}
