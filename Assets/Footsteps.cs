using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField] AudioClip[] clips;

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();   
    }


    // Gets called by an animation event
    void Step()
    {
        Debug.Log("Stepping!");
        if(clips.Length > 0)
        {
            AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];
            audioSource.PlayOneShot(clip);
        }
    }
}
