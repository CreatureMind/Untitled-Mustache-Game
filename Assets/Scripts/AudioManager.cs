using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // singleton
    [SerializeField] private List<Sound> sounds; // custom class for a single sound in a list
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Sound s in sounds) // instantiates all sounds 
        {
            s.audioSource = gameObject.AddComponent<AudioSource>();
            s.audioSource.clip = s.clip;
            
            s.audioSource.volume = s.volume;
            s.audioSource.pitch = s.pitch;
            s.audioSource.loop = s.loop;
        }
        DontDestroyOnLoad(gameObject);
        PlaySound("theme");
    }

    public void PlaySound(string soundName) // a function to play a sound from anywhere in the script
    {
        foreach (Sound s in sounds)
        {
            if (s.name == soundName)
            {
                s.audioSource.Play();
            }
        }
    }
}

