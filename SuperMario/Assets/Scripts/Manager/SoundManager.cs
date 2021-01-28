using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public bool muted;
    Toggle toogle;

    void Awake()
    {
        if (toogle == null)
            toogle = FindObjectOfType<Toggle>();
    }

    void Start()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public void ToogleMusic(bool newValue)
    {
        muted = newValue;

        if (muted)
        {
            toogle.isOn = true;
            AudioListener.volume = 0;
        }
        else
        {
            toogle.isOn = false;
            AudioListener.volume = 1;
        }
    }


}
