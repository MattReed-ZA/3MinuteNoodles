using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFX : MonoBehaviour
{
    public AudioSource Fs1,Fs2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FS1()
    {
       Fs1.Play();
    }

    void FS2()
    {
       Fs2.Play();
    }
}

