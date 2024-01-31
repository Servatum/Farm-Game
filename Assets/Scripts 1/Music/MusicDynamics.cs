using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicDynamics : MonoBehaviour
{
    #region Variables
    //Objects
    public GameObject Player;
    public AudioSource Drums;
    public AudioSource Bass;

    //Bools
    public static bool playingDrums = false;
    public static bool playingBass = false;
    public static bool disableAll = false;
    public static bool musicVolume = true;
    private bool musicChanging = false;
    #endregion
    void Update()
    {
        #region Hell
        if (playingDrums == true)
        {
            Drums.mute = false;
        }

        if (playingBass == true)
        {
            Bass.mute = false;
        }

        if (disableAll == true)
        {
            disableAll = false;
            playingDrums = false;
            Drums.mute = true;
            playingBass = false;
            Bass.mute = true;
        }
        if (musicVolume == false && musicChanging == false)
        {
            musicChanging = true;
            StartCoroutine(VolumeDecreaser());
        }
        if (musicVolume == true && musicChanging == false)
        {
            musicChanging = true;
            StartCoroutine(VolumeIncreaser());
        }
        #endregion
    }
    #region VolumeSliders
    IEnumerator VolumeDecreaser()
    {
        while (Drums.volume > 0.3f)
        {
            yield return new WaitForSeconds(0.3f);
            Drums.volume -= 0.1f;
            Debug.Log(Drums.volume);
        }
        Drums.volume = 0.3f;
        musicChanging = false;
    }
    IEnumerator VolumeIncreaser()
    {
        while (Drums.volume < 1f)
        {
            yield return new WaitForSeconds(0.3f);
            Drums.volume += 0.1f;
            Debug.Log(Drums.volume);
        }
        Drums.volume = 1f;
        musicChanging = false;
    }
    #endregion
}
