using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggablesController : MonoBehaviour
{
    public PlayerController player;

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
        if(other.tag == "Player" && player != null)
        {
            player.setDragObject(this.GetComponent<Rigidbody2D>());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player" && player != null)
        {
            player.setDragObject(null);
        }
    } 
}
