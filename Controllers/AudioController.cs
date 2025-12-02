using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public static AudioController audioController;
    public AudioSource mySoundBox;
    public AudioMixer myMixer;
    public AudioClip[] musics;

    public float currentMasterVolume {  get; private set; }
    public float currentMusicVolume { get; private set; }
    public float currentSFXVolume { get; private set; }

    public AudioSource onHover;
    public AudioSource onClick;

    private void Awake()
    {
        if (audioController == null)
        {
            audioController = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        

    }

    
    void Start()
    {
        mySoundBox = GetComponent<AudioSource>();

        //currentMasterVolume = 1f;
        //currentMusicVolume = .5f;
        //currentSFXVolume = .5f;

        //Initialize();
    }

    public void Initialize()
    {
        ChangeMasterVolume(currentMasterVolume);
        ChangeMusicVolume(currentMusicVolume);
        ChangeSFXVolume(currentSFXVolume);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMute();
        }
    }

    private float LinearToDb(float linear)
    {
        // Evita -Infinity quando linear = 0
        if (linear <= 0.0001f)
            return -80f; // praticamente mudo no mixer

        return Mathf.Log10(linear) * 20f; // 1 → 0 dB, 0.5 → ~-6 dB, etc.
    }
    
    
    public void ToggleMute()
    {
        mySoundBox.mute = !mySoundBox.mute;
    }

    public void ChangeMasterVolume(float value)
    {
        currentMasterVolume = value;        
        myMixer.SetFloat("MasterVol", LinearToDb(value));
        
    }

    public void ChangeMusicVolume(float value)
    {
        currentMusicVolume = value;
        myMixer.SetFloat("MusicVol", LinearToDb(value));
        
    }

    public void ChangeSFXVolume(float value)
    {
        currentSFXVolume = value;
        myMixer.SetFloat("SFXVol", LinearToDb(value));
        
    }

    public void SwitchMusic(string music)
    {
        switch (music)
        {
            case "Menu":
                mySoundBox.clip = musics[0];
                break;
            case "Jogo":
                mySoundBox.clip = musics[1];
                break;
            default:
                break;
        }
        mySoundBox.loop = true;
        mySoundBox.Play();
    }


}
