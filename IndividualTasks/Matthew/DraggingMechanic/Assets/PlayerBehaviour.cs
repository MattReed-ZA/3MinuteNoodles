using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public KeyCode upButton = KeyCode.UpArrow;
    public KeyCode downButton = KeyCode.DownArrow;
    public KeyCode leftButton = KeyCode.LeftArrow;
    public KeyCode rightButton = KeyCode.RightArrow;

    private float speed = 5f;
    private float dragSpeed = 2f;

    private SpriteRenderer player;

    private bool inDragRange = false;
    private bool dragging = false;
    private GameObject dragObject = null;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //To Start Dragging
        if(inDragRange == true && dragObject != null && Input.GetMouseButtonDown(0))
        {
            if(dragging == false)
            {
                dragging = true;

                dragObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                dragObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            else
            {
                dragging = false;

                dragObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                dragObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
                dragObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }

        //Movement
        if(dragging == true && dragObject != null)
        {
            if(Input.GetKey(upButton))
            {
                GetComponent<Rigidbody2D>().velocity = new Vector3(0f, dragSpeed, 0f);
                //@TODO
                //1. Change Character Sprite
                //2. Change Character Sprite Orientation
                //3. Move DragObject
                dragObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0f, dragSpeed, 0f);
            }
            else if(Input.GetKey(downButton))
            {
                GetComponent<Rigidbody2D>().velocity = new Vector3(0f, dragSpeed*-1, 0f);
                //@TODO
                //1. Change Character Sprite
                //2. Change Character Sprite Orientation
                //3. Move DragObject
                dragObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0f, dragSpeed*-1, 0f);
            }
            else if(Input.GetKey(leftButton))
            {
                GetComponent<Rigidbody2D>().velocity = new Vector3(dragSpeed*-1, 0f, 0f);
                //@TODO
                //1. Change Character Sprite
                //2. Change Character Sprite Orientation
                //3. Move DragObject
                dragObject.GetComponent<Rigidbody2D>().velocity = new Vector3(dragSpeed*-1, 0f, 0f);
            }
            else if(Input.GetKey(rightButton))
            {
                GetComponent<Rigidbody2D>().velocity = new Vector3(dragSpeed, 0f, 0f);
                //@TODO
                //1. Change Character Sprite
                //2. Change Character Sprite Orientation
                //3. Move DragObject
                dragObject.GetComponent<Rigidbody2D>().velocity = new Vector3(dragSpeed, 0f, 0f);
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = new Vector3(0f, 0f, 0f);
                //@TODO
                //1. Change Character Sprite
                //2. Change Character Sprite Orientation
                //3. Move DragObject
                dragObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0f, 0f, 0f);
            }
        }
        else
        {
            if(Input.GetKey(upButton))
            {
                GetComponent<Rigidbody2D>().velocity = new Vector3(0f, speed, 0f);
            }
            else if(Input.GetKey(downButton))
            {
                GetComponent<Rigidbody2D>().velocity = new Vector3(0f, speed*-1, 0f);
            }
            else if(Input.GetKey(leftButton))
            {
                GetComponent<Rigidbody2D>().velocity = new Vector3(speed*-1, 0f, 0f);
            }
            else if(Input.GetKey(rightButton))
            {
                GetComponent<Rigidbody2D>().velocity = new Vector3(speed, 0f, 0f);
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = new Vector3(0f, 0f, 0f);
            }
        }
    }

    public void setInDragRange(bool v)
    {
        inDragRange = v;
    }

    public void setDragObject(GameObject obj)
    {
        if(obj != null)
        {
            dragObject = obj;
            Debug.Log(dragObject);
        }
    }
}
