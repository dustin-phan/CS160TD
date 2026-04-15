using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class VolumeManager : MonoBehaviour
{
    [SerializeField] Slider MasterVolume;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!PlayerPrefs.HasKey("MasterVolume"))
        {
            PlayerPrefs.SetFloat("MasterVolume", 1);
            LoadVolumePrefs();
        }
        else
        {
            LoadVolumePrefs();
        }
    }

    public void ChangeVolume()
    {
        AudioListener.volume = MasterVolume.value;
        SaveVolumePrefs();
    }

    private void LoadVolumePrefs()
    {
        MasterVolume.value = PlayerPrefs.GetFloat("MasterVolume");
    }

    private void SaveVolumePrefs()
    {
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume.value);
    }
}
