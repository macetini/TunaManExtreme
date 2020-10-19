using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    //public static string SHOW_FLASH_TRIGGER = "ShowFlash";
    public static string MUZZLE_FLASH = "MuzzleFlash";

    private Animator _animator;

    void Start()
    {

        _animator = GetComponent<Animator>();
    }

    public void PlayFlash()
    {
        _animator.Play(MUZZLE_FLASH);        
    }

    /*
    private void Update()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(MUZZLE_FLASH))
        {
            //Debug.Log("Playing");
        }
        else
        {
            //Debug.Log("Waiting");
            }           
    }
    */

}
