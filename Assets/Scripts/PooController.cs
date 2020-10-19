using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooController : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collider)
    {       
        Destroy(this.gameObject);
    }    
}
