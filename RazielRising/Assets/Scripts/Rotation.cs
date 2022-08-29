using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private float speed=2.0f;
    [SerializeField] private Rigidbody2D wheel;

    private void Update()
    {
        transform.Rotate(0,0,360 * speed * Time.deltaTime);
        wheel.constraints=RigidbodyConstraints2D.FreezePositionY;
        wheel.constraints=RigidbodyConstraints2D.FreezePositionX;
    }
}
