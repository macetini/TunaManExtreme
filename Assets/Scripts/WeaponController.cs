using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public static Vector3 OFFSET = new Vector2(1.45f, 0.2f);

    public GameObject flashPrefab;
    public float reloadTime = 0.3f;

    private GameObject _flash;
    private AnimController _animController;

    private float _reloadTimeLeft = 0f;
    public bool reloading
    {
        get
        {
            return _reloadTimeLeft > 0f;
        }
    }

    private void Start()
    {
        _flash = Instantiate(
            flashPrefab,
            transform.position + OFFSET,
            transform.rotation,
            transform
            );

        _animController = _flash.GetComponent<AnimController>();
    }

    public void PlayFlash()
    {
        if (_reloadTimeLeft <= 0f)
        {
            _animController.PlayFlash();
            _reloadTimeLeft = reloadTime;
        }
    }

    private void Update()
    {
        if (_reloadTimeLeft > 0f) _reloadTimeLeft -= Time.deltaTime;        
    }
}
