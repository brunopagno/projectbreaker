using System;
using UnityEngine;

public class HeroControl : MonoBehaviour
{
    private CharacterController _controller;
    private Vector3 _velocity;

    // Base attributes
    public float gravity = 1;
    public float speed = 2;
    public float jump = 10;
    public float fireRate = 4;
    public float breakFreq = 1.25f;
    public float breakDuration = 0.12f;
    public float baseHealth = 100;
    public float regen = 1;

    // Damage
    public GameObject blueDamage;
    private float _blueDamage = 0;

    // Projectile
    public GameObject projectile;
    public GameObject projectileCooldown;
    public Vector3 projectileOrigin;
    private Cooldown _projectileCooldown;

    // Break
    public GameObject breakWeapon;
    public GameObject breakWeaponCooldown;
    public Vector3 breakOrigin;
    public float breakReach;
    private Cooldown _breakCooldown;
    private float _timeToHideBreak;
    private bool _isBreaking;

    //  1 => right
    // -1 => left
    private int _direction = 1;

    private void Start()
    {
        _velocity = new Vector3();
        _controller = GetComponent<CharacterController>();
        _projectileCooldown = new Cooldown(1f / fireRate);
        _breakCooldown = new Cooldown(1f / breakFreq);
    }

    private void Update()
    {
        Gravity();
        Run();
        Jump();
        Facing();
        Shoot();
        Break();
        CharacterController();
        Clear();
        RegenDamage();
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
            _direction = 1;
        }
        if (_velocity.x < 0)
        {
            _direction = -1;
        }
    }

    private void Shoot()
    {
        _projectileCooldown.Update();
        if (Input.GetButton("Fire1") && _projectileCooldown.Ready)
        {
            GameObject instantiatedProjectile = Instantiate(
                projectile,
                transform.position + new Vector3(
                    projectileOrigin.x * _direction,
                    projectileOrigin.y,
                    projectileOrigin.z),
                Quaternion.identity);
            ProjectileBehaviour projectileBehaviour = instantiatedProjectile.GetComponent<ProjectileBehaviour>();
            projectileBehaviour.Direction = _direction;

            _projectileCooldown.Reset();
        }

        UpdateUiCooldowonBar(projectileCooldown, _projectileCooldown.Readyness);
    }

    private void Break()
    {
        _breakCooldown.Update();
        if (Input.GetButtonDown("Fire2") && _breakCooldown.Ready)
        {
            _isBreaking = true;

            if (_direction > 0)
            {
                breakWeapon.transform.rotation = Quaternion.identity;
                breakWeapon.transform.position = transform.position + breakOrigin;
            }
            else
            {
                breakWeapon.transform.rotation = Quaternion.Euler(0, 180, 0);
                breakWeapon.transform.position = transform.position + new Vector3(breakOrigin.x * _direction,
                                                                                  breakOrigin.y,
                                                                                  breakOrigin.z);
            }

            breakWeapon.SetActive(true);

            _breakCooldown.Reset();
        }

        if (_isBreaking)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + breakOrigin, transform.position + breakOrigin + Vector3.right * breakReach * _direction, out hit))
            {
                if (hit.collider.tag == "Player")
                {
                    Debug.Log("I HIT A GUY");
                }
            }

            if (_timeToHideBreak > breakDuration)
            {
                _isBreaking = false;
                breakWeapon.SetActive(false);

                _timeToHideBreak = 0;
            }

            _timeToHideBreak += Time.deltaTime;
        }

        UpdateUiCooldowonBar(breakWeaponCooldown, _breakCooldown.Readyness);
    }

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

    private void RegenDamage()
    {
        _blueDamage -= Time.deltaTime;
        if (_blueDamage < 0)
        {
            _blueDamage = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Projectile")
        {
            ProjectileBehaviour projectileBehaviour = projectile.GetComponent<ProjectileBehaviour>();
            _blueDamage += projectileBehaviour.baseDamage;
            if (_blueDamage > baseHealth)
            {
                _blueDamage = baseHealth;
            }
        }
    }

    private void UpdateUiCooldowonBar(GameObject cooldownBar, float readynessPercent)
    {
        Vector3 scale = cooldownBar.transform.localScale;
        scale.Set(readynessPercent, scale.y, scale.z);
        cooldownBar.transform.localScale = scale;
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        float size = 0.2f;
        Gizmos.DrawLine(
            transform.position + new Vector3(projectileOrigin.x * _direction,
                                             projectileOrigin.y - size,
                                             projectileOrigin.z),
            transform.position + new Vector3(projectileOrigin.x * _direction,
                                             projectileOrigin.y + size,
                                             projectileOrigin.z)
            );
        Gizmos.DrawLine(
            transform.position + new Vector3(projectileOrigin.x * _direction - size,
                                             projectileOrigin.y,
                                             projectileOrigin.z),
            transform.position + new Vector3(projectileOrigin.x * _direction + size,
                                             projectileOrigin.y,
                                             projectileOrigin.z));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            transform.position + new Vector3(breakOrigin.x * _direction,
                                             breakOrigin.y,
                                             breakOrigin.z),
            transform.position + new Vector3(breakOrigin.x * _direction + breakReach * _direction,
                                             breakOrigin.y,
                                             breakOrigin.z)
            );
    }
    #endregion
}
