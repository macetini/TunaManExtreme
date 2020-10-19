using UnityEngine;

public class HeroController : MonoBehaviour
{
    public GameObject weaponPrefab;

    [Range(1.0f, 20.0f)]
    public float weaponRotationSpeed = 10.0f;

    public float maxJumpForce = 15;

    [Range(0.1f, 1.0f)]
    public float jumpForceIncrement = 0.5f;

    [Range(0.1f, 5.0f)]
    public float boostMultiplier = 2f;

    private GameObject _weaponInstance;
    private WeaponController _weaponControler;

    private Rigidbody2D _rb;

    private Vector3 _heroViewportPos = new Vector2(0.5f, 0.6f);

    private bool _boostActive = false;
    private float _boostAngleTarget;
    private Quaternion _boostRotation;
    private Vector2 _boostForceDirection;

    private float _currentJumpForce = 0.0f;

    private bool _grounded;

    public float currentJumpForce
    {
        get
        {
            return _currentJumpForce;
        }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        CircleCollider2D collider = GetComponent<CircleCollider2D>();

        Quaternion initRotation = Quaternion.Euler(new Vector3(0f, 0f, 180));

        _weaponInstance = Instantiate(
            weaponPrefab,
            new Vector2(transform.position.x, transform.position.y),
            initRotation
            );

        _weaponControler = _weaponInstance.GetComponent<WeaponController>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateJump();

        UpdateWeaponPosition();

        if (!_boostActive) UpdateWeaponRotation();

        if (!_boostActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ShootForward();
            }
            else if (!_weaponControler.reloading && Input.GetMouseButtonDown(1))
            {
                ShootBackwards();
            }
        }

        if (_boostActive) ResetFInishedBoost();
    }

    protected void UpdateJump()
    {
        if (_grounded && currentJumpForce < maxJumpForce && Input.GetKey(KeyCode.Space))
        {
            _currentJumpForce += jumpForceIncrement;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (_currentJumpForce > 0f)
            {
                _rb.AddForce(Vector2.up * _currentJumpForce, ForceMode2D.Impulse);

                _currentJumpForce = 0f;
            }
        }
    }

    protected void UpdateWeaponPosition()
    {
        _weaponInstance.transform.position = new Vector2(transform.position.x, transform.position.y);
    }

    protected void UpdateWeaponRotation()
    {
        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

        float angle = AngleBetweenTwoPoints(_heroViewportPos, mouseOnScreen);

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, (int)angle));

        float rotationProgess = angle / _weaponInstance.transform.rotation.eulerAngles.z;

        _weaponInstance.transform.rotation = Quaternion.Slerp(_weaponInstance.transform.rotation, targetRotation, weaponRotationSpeed * Time.deltaTime);
    }

    protected void ShootForward()
    {
        float currentAngle = _weaponInstance.transform.rotation.eulerAngles.z;
        float oppositeAngle = NormalizeAngle(currentAngle + 180f);

        Quaternion boostRotation = Quaternion.Euler(0f, 0f, oppositeAngle);

        Vector2 forceDirection = boostRotation * Vector2.left;

        _rb.AddForce(forceDirection * boostMultiplier, ForceMode2D.Impulse);

        _weaponControler.PlayFlash();
    }

    protected void ShootBackwards()
    {
        _boostActive = true;

        float currentAngle = _weaponInstance.transform.rotation.eulerAngles.z;
        float oppositeAngle = currentAngle + 180f;
        float angleDifference = Mathf.DeltaAngle(currentAngle, oppositeAngle);

        _boostAngleTarget = (int)NormalizeAngle(currentAngle - angleDifference);

        Quaternion boostRotation = Quaternion.Euler(0f, 0f, currentAngle);

        _boostForceDirection = boostRotation * Vector2.left;
    }

    protected void ResetFInishedBoost()
    {
        float roundedAngle = Mathf.Ceil(_weaponInstance.transform.rotation.eulerAngles.z);

        if (roundedAngle >= _boostAngleTarget - 7f && roundedAngle <= _boostAngleTarget + 7f)
        {
            _weaponControler.PlayFlash();
        }

        if (roundedAngle >= _boostAngleTarget - 2f && roundedAngle <= _boostAngleTarget + 2f)
        {
            _boostActive = false;
            _rb.AddForce(_boostForceDirection * boostMultiplier, ForceMode2D.Impulse);
        }

        Quaternion newRotation = Quaternion.Euler(new Vector3(0f, 0f, _boostAngleTarget));

        _weaponInstance.transform.rotation = Quaternion.Slerp(_weaponInstance.transform.rotation, newRotation, weaponRotationSpeed * Time.deltaTime);
    }

    protected float AngleBetweenTwoPoints(Vector2 a, Vector3 b)
    {
        float newAngle = Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;

        return Mathf.Floor(newAngle);
    }

    public float NormalizeAngle(float angle)
    {
        while (angle > 360)
            angle -= 360;

        while (angle < 0)
            angle += 360;

        return angle;
    }

    void OnCollisionStay2D(Collision2D collider)
    {
        if (collider.gameObject.layer == TerrainGenerator.LAYER_GROUND) CheckIfGrounded();        
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.layer == TerrainGenerator.LAYER_POO) ResetPlayerPos();
    }

    protected void ResetPlayerPos()
    {
        transform.position = new Vector2(0f, 3.5f);
    }

    void OnCollisionExit2D(Collision2D collider)
    {
        if (collider.gameObject.layer == TerrainGenerator.LAYER_GROUND) _grounded = false;
    }

    private void CheckIfGrounded()
    {
        RaycastHit2D[] hits;

        //We raycast down 1 pixel from this position to check for a collider
        Vector2 positionToCheck = transform.position;
        hits = Physics2D.RaycastAll(positionToCheck, new Vector2(0, -1), 0.01f);

        //if a collider was hit, we are grounded
        if (hits.Length > 0)
        {
            _grounded = hits.Length > 0;
        }

    }
}
