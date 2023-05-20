using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private List<AudioSource> destroyNoise;
    [SerializeField] private AudioSource music;
    public void PlayRandomDestroyNoise(){
        //Choose a random number
        int clipToPlay = Random.Range(0, destroyNoise.Count);

        //play that clip
        destroyNoise[clipToPlay].Play();
    }

}
