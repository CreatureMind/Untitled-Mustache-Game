using UnityEngine;
using UnityEngine.UI;

public class Daily_Button : MonoBehaviour
{
    [SerializeField] private Image lockImage;
    [SerializeField] private Image checkmarkImage;
    [SerializeField] private Button button;

    public Image LockImage => lockImage;
    public Image CheckmarkImage => checkmarkImage;
    public Button Button => button;
}