using System;
using UnityEngine;

public class HeroControl : MonoBehaviour
{
    private CharacterController _controller;
    private Vector3 _velocity;

    public float gravity = 1;
    public float speed = 2;
    public float jump = 10;
    public float fireRate = 4;

    public GameObject projectile;
    public Vector3 projectileOrigin;

    private float _timeBetweenShoots;
    private float _timeToShoot;
    private bool _facingRight = true;
    public bool FacingRight
    {
        get
        {
            return _facingRight;
        }
    }

    private void Start()
    {
        _velocity = new Vector3();
        _controller = GetComponent<CharacterController>();
        _timeBetweenShoots = fireRate / 60.0f;
    }

    private void Update()
    {
        Gravity();
        Run();
        Jump();
        Facing();
        Shoot();
        //Break();
        CharacterController();
        Clear();
    }

    private void Gravity()
    {
        _velocity.y -= gravity;
    }

    private void Run()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        _velocity.x += horizontalInput * speed;
    }

    private void Jump()
    {
        if (_controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            _velocity.y += jump;
        }
    }

    private void Facing()
    {
        if (_velocity.x > 0)
        {
            _facingRight = true;
        }
        if (_velocity.x < 0)
        {
            _facingRight = false;
        }
    }

    private void Shoot()
    {
        _timeToShoot += Time.deltaTime;
        if (Input.GetButton("Fire1") && _timeToShoot > _timeBetweenShoots)
        {
            int direction = FacingRight ? 1 : -1;

            GameObject instantiatedProjectile = Instantiate(
                projectile,
                transform.position + new Vector3(
                    projectileOrigin.x * direction,
                    projectileOrigin.y,
                    projectileOrigin.z),
                Quaternion.identity);
            ProjectileBehaviour projectileBehaviour = instantiatedProjectile.GetComponent<ProjectileBehaviour>();
            projectileBehaviour.Direction = FacingRight ? 1 : -1;
            _timeToShoot = 0;
        }
    }

    //private void Break()
    //{
    //    // nothing here, yet
    //}

    private void CharacterController()
    {
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void Clear()
    {
        _velocity.x = 0;
        if (_controller.isGrounded)
        {
            _velocity.y = 0;
        }
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        float size = 0.2f;
        Gizmos.DrawLine(
            transform.position + new Vector3(projectileOrigin.x * (FacingRight ? 1 : -1),
                                             projectileOrigin.y,
                                             projectileOrigin.z) + new Vector3(0, -size, 0),
            transform.position + new Vector3(projectileOrigin.x * (FacingRight ? 1 : -1),
                                             projectileOrigin.y,
                                             projectileOrigin.z) + new Vector3(0, size, 0)
            );
        Gizmos.DrawLine(
            transform.position + new Vector3(projectileOrigin.x * (FacingRight ? 1 : -1),
                                             projectileOrigin.y,
                                             projectileOrigin.z) + new Vector3(-size, 0, 0),
            transform.position + new Vector3(projectileOrigin.x * (FacingRight ? 1 : -1),
                                             projectileOrigin.y,
                                             projectileOrigin.z) + new Vector3(size, 0, 0));
    }
    #endregion
}
