using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateBehaviour : MonoBehaviour
{
    public PlayerBehaviour player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            player.setInDragRange(true);
            player.setDragObject(this.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            player.setInDragRange(false);
            player.setDragObject(null);
        }
    }
}
