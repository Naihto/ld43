using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour {

    [SerializeField] AudioSource audioSource;
    [SerializeField] Toggle muteToggle;
    [SerializeField] Slider volume;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void MuteToggle()
    {
        audioSource.mute = !muteToggle.isOn;
    }

    public void ChangeVol()
    {
        audioSource.volume = volume.value;
    }
}
