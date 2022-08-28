using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncySurfaces : MonoBehaviour
{
   [SerializeField]private float bounce = 10f;
   public ParticleSystem light;

   private void OnCollisionEnter2D(Collision2D collision)
   {
        if(collision.gameObject.CompareTag("Player"))
        {
            SoundManagerScript.PlaySound("Bounce");
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce,ForceMode2D.Impulse);
            CreateLight();
        }
   }

   void CreateLight()
   {
        light.Play();
   }
}
