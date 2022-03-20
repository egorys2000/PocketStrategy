using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SliderPrefixValue : MonoBehaviour
{
    [SerializeField]
    private Slider Slider;

    [SerializeField]
    private TextMeshProUGUI Text;

    private string InitialText;

    [SerializeField]
    private List<string> TextOptions;

    void Awake() 
    {
        InitialText = Text.text;
        UpdateText();
    }


    public void UpdateText() 
    {
        if(TextOptions.Count == 0) Text.text = InitialText + ": " + Slider.value.ToString();
        else Text.text = InitialText + ": " + TextOptions[(int)Slider.value];
    }
}
