using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip PlayerJump;
    public static AudioClip PlayerDash;
    public static AudioClip PlayerBounce;
    static AudioSource audiosrc;
    
    void Start()
    {
        PlayerJump=Resources.Load<AudioClip>("JumpV2");
        PlayerDash=Resources.Load<AudioClip>("Dash");
        PlayerBounce=Resources.Load<AudioClip>("Lilly Pad Bounce");

        audiosrc=GetComponent<AudioSource>();
    }

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
            case "Bounce":
                audiosrc.PlayOneShot(PlayerBounce);
                break;
        }
    }
}
