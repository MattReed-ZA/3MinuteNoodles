using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip PlayerJump;
    //public static AudioClip PlayerRun;
    public static AudioClip PlayerDash;
    static AudioSource audiosrc;
    
    // Start is called before the first frame update
    void Start()
    {
        PlayerJump=Resources.Load<AudioClip>("Jump");
        //PlayerRun=Resources.Load<AudioClip>("Running");
        PlayerDash=Resources.Load<AudioClip>("Dash");

        audiosrc=GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound(string clip)
    {
        switch(clip)
        {
            case "Jump":
                audiosrc.PlayOneShot(PlayerJump);
                break;
            case "Dash":
                audiosrc.PlayOneShot(PlayerDash);
                break;
        }
    }
}
