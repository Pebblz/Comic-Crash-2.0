using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{



    AudioMixerGroup sfx;
    AudioMixerGroup music;
    AudioMixerGroup amb;

    private const string musicVol = "MusicVol";
    private const string sfxVol = "SFXVol";
    private const string ambVol = "AmbVol";

    [SerializeField]
    [Tooltip("The length of the transition to and from water")]
    float waterTimeout = 0.3f;

    [SerializeField]
    [Tooltip("The Filter for the underwater section")]
    AudioMixerSnapshot underwater;


    [SerializeField]
    [Tooltip("The default filter section")]
    AudioMixerSnapshot normal;


    #region SFX
    [SerializeField] AudioSource boing;
    [SerializeField] AudioSource box_break;
    [SerializeField] AudioSource coin_get;
    [SerializeField] AudioSource move_obj;
    [SerializeField] AudioSource wall_slide;
    [SerializeField] AudioSource thud;
    #endregion


    #region Ambience
    [SerializeField] AudioSource ocean;

    #endregion
    private void Awake()
    {
        AudioMixer mixer = Resources.Load("Sounds/Mixer") as AudioMixer;

        if(normal != null)
            normal.TransitionTo(0.1f);

        if (mixer != null)
        {
            //only one of each group will be found, it just likes to return an array
            sfx = mixer.FindMatchingGroups("SFX")[0];
            music = mixer.FindMatchingGroups("Music")[0];
            amb = mixer.FindMatchingGroups("Ambience")[0];


        }
        else
        {
            Debug.LogError("Error Sound Mixer was not loaded properly");
        }

    }


    #region UI_INTERACTION
    public void setSFXVol(float value)
    {

        sfx.audioMixer.SetFloat(sfxVol, Mathf.Log10(value) * 20);
    }

    public void setMusicVol(float value)
    {
        music.audioMixer.SetFloat(musicVol, Mathf.Log10(value) * 20);
    }

    public void setAmbVol(float value)
    {
        amb.audioMixer.SetFloat(ambVol, Mathf.Log10(value) * 20);
    }

    public float getSFXVol()
    {

        sfx.audioMixer.GetFloat(sfxVol, out float val);
        val = Mathf.Pow(10, val / 20);
        if (val <= 0 || val > 1)
        {
            return 0.5f;
        }
        return val;
    }

    public float getMusicVol()
    {
        music.audioMixer.GetFloat(musicVol, out float val);
        val = Mathf.Pow(10, val / 20);
        if (val <= 0 || val > 1)
        {
            return 0.5f;
        }
        return val;
    }

    public float getAMBVol()
    {
        amb.audioMixer.GetFloat(musicVol, out float val);
        val = Mathf.Pow(10, val / 20);
        if (val <= 0 || val > 1)
        {
            return 0.5f;
        }
        return val;
    }

    #endregion

    #region play_pause_methods
    public void playOceanSounds()
    {
        if (ocean.isPlaying) return;

        this.ocean.loop = true;
        this.ocean.Play();
    }

    public void playBoingSound(Vector3 world_point)
    {

        boing.gameObject.transform.position = world_point;
        this.boing.Play();
    }

    public void playBoxBreak(Vector3 world_point)
    {
        box_break.gameObject.transform.position = world_point;
        this.box_break.Play();
    }

    public void playThud(Vector3 world_point)
    {
        thud.gameObject.transform.position = world_point;
        thud.Play();
    }

    public void playCoin(Vector3 world_point)
    {
        coin_get.gameObject.transform.position = world_point;
        coin_get.Play();
    }
    #endregion

    #region FILTER_TRANSITIONS

    public void to_underwater()
    {
        underwater.TransitionTo(waterTimeout);
    }

    public void to_normal_from_water()
    {
        normal.TransitionTo(waterTimeout);
    }
    
    #endregion
}
