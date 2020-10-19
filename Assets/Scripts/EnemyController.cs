using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject pooPrefab;

    public float reloadTime = 0.3f;

    [Range(0.1f, 1.0f)]
    public float pooProbability = 0.5f;

    public float timeBetweenPoos = 0.3f;

    public Vector2 movementBounds = new Vector2(2f, 2f);

    [Range(1f, 20f)]
    public float movementSpeed = 10;

    private SpriteRenderer _sr;

    private bool _moveLeftState;
    private Vector2 _relativeBounds;

    private float _pooTimeLeft;

    void Start()
    {
        _pooTimeLeft = timeBetweenPoos;

        _sr = this.GetComponent<SpriteRenderer>();
        _relativeBounds = new Vector2(transform.position.x - movementBounds.x, transform.position.x + movementBounds.y);
    }

    void Update()
    {
        UpdatePosition();
        CheckForPoo();
    }

    protected void UpdatePosition()
    {
        float xPos = transform.position.x;

        float moveTarget = _moveLeftState ? _relativeBounds.x : _relativeBounds.y;

        float xPosMove = Mathf.Lerp(xPos, moveTarget, (movementSpeed / 10f) * Time.deltaTime);

        transform.position = new Vector2(xPosMove, transform.position.y);

        if ((int)xPosMove == (int)moveTarget)
        {
            _moveLeftState = !_moveLeftState;
            _sr.flipY = _moveLeftState;
        }
    }

    protected void CheckForPoo()
    {
        if (_pooTimeLeft > 0f) _pooTimeLeft -= Time.deltaTime;

        if (_pooTimeLeft <= 0f && Random.Range(0, 1f) <= pooProbability)
        {
            _pooTimeLeft = timeBetweenPoos;

            GameObject poo = Instantiate(pooPrefab, transform.position, transform.rotation);
            poo.layer = TerrainGenerator.LAYER_POO;
        }
    }
}
