using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    public GameObject[] colors;
    public Sprite red, orange, yellow, green, turqoaise, blue;
    public bool opened = false;
    private void Awake()
    {
        MainUI.Instance.colorPicker = this;
    }
    public void OnColorPickerPress()
    {
        opened = !opened;
        foreach (var color in colors)
        {
            //you can set to start your animation here if you want on them
            color.gameObject.SetActive(opened);
        }
    }
    public void OnRedColorPress()
    {
        MainUI.Instance.currentGradient = Gradient.Red;
        GetComponent<Image>().sprite = red;
        OnColorPickerPress();
    }
    public void OnOrangeColorPress()
    {
        MainUI.Instance.currentGradient = Gradient.Orange;
        GetComponent<Image>().sprite = orange;
        OnColorPickerPress();
    }
    public void OnYellowColorPress()
    {
        MainUI.Instance.currentGradient = Gradient.Yellow;
        GetComponent<Image>().sprite = yellow;
        OnColorPickerPress();
    }
    public void OnGreenColorPress()
    {
        MainUI.Instance.currentGradient = Gradient.Green;
        GetComponent<Image>().sprite = green;
        OnColorPickerPress();
    }
    public void OnTurqoaiseColorPress()
    {
        MainUI.Instance.currentGradient = Gradient.Turqoaise;
        GetComponent<Image>().sprite = turqoaise;
        OnColorPickerPress();
    }
    public void OnBlueColorPress()
    {
        MainUI.Instance.currentGradient = Gradient.Blue;
        GetComponent<Image>().sprite = blue;
        OnColorPickerPress();
    }
}
