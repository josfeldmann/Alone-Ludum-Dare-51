using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderVal : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI text;
    public string prompt;

    private void Awake() {
        UpdateText();
    }

    public float GetValue() {
        return slider.value;
    }

    public void UpdateText() {
        text.text = prompt + GetValue().ToString(); 
    }



}
