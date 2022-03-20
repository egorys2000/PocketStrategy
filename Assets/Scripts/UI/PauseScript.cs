using UnityEngine;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    [SerializeField]
    private Sprite[] PauseSprite;

    [SerializeField]
    private GameObject PauseContent;

    private enum Mode 
    {
        Play, Pause
    }

    [SerializeField]
    private Mode mode;
    private Image Img;

    void Awake() 
    {
        Img = GetComponent<Image>();
        mode = Mode.Play;
    }

    private void SwitchIcon() 
    {
        if (mode == Mode.Pause) mode = Mode.Play;
        else mode = Mode.Pause;

        Img.sprite = PauseSprite[(int)mode];
    }

    public void PauseButtonSwitchIcon() 
    {
        SwitchIcon();
    }
}
