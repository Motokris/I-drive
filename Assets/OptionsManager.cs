using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public GameObject volumeSlider;

    private void Start()
    {
        volumeSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("VolumeValue");
    }

    public void Back()
    {
        PlayerPrefs.SetFloat("VolumeValue", volumeSlider.GetComponent<Slider>().value);
    }
}
