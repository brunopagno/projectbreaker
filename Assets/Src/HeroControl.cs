﻿using System;
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
    public float breakReach = 1.8f;
    public float baseHealth = 100;
    public float regen = 1;

    // LifeBar
    public GameObject HealthBar;
    public GameObject BlueDamage;
    private float _blueDamage = 0;

    // Projectile
    public GameObject Projectile;
    public GameObject ProjectileCooldown;
    public Vector3 ProjectileOrigin;
    private Cooldown _projectileCooldown;

    // Break
    public GameObject BreakWeapon;
    public GameObject BreakWeaponCooldown;
    public Vector3 BreakOrigin;
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
        Health();
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
                Projectile,
                transform.position + new Vector3(
                    ProjectileOrigin.x * _direction,
                    ProjectileOrigin.y,
                    ProjectileOrigin.z),
                Quaternion.identity);
            ProjectileBehaviour projectileBehaviour = instantiatedProjectile.GetComponent<ProjectileBehaviour>();
            projectileBehaviour.Direction = _direction;

            _projectileCooldown.Reset();
        }

        UpdateUiCooldownBar(ProjectileCooldown, _projectileCooldown.Readyness);
    }

    private void Break()
    {
        _breakCooldown.Update();
        if (Input.GetButtonDown("Fire2") && _breakCooldown.Ready)
        {
            _isBreaking = true;

            if (_direction > 0)
            {
                BreakWeapon.transform.rotation = Quaternion.identity;
                BreakWeapon.transform.position = transform.position + BreakOrigin;
            }
            else
            {
                BreakWeapon.transform.rotation = Quaternion.Euler(0, 180, 0);
                BreakWeapon.transform.position = transform.position + new Vector3(BreakOrigin.x * _direction,
                                                                                  BreakOrigin.y,
                                                                                  BreakOrigin.z);
            }

            BreakWeapon.SetActive(true);

            _breakCooldown.Reset();
        }

        if (_isBreaking)
        {
            if (_timeToHideBreak > breakDuration)
            {
                _isBreaking = false;
                BreakWeapon.SetActive(false);

                _timeToHideBreak = 0;
            }

            _timeToHideBreak += Time.deltaTime;
        }

        UpdateUiCooldownBar(BreakWeaponCooldown, _breakCooldown.Readyness);
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

    private void Health()
    {
        UpdateUiCooldownBar(HealthBar, 1 - _blueDamage / baseHealth);
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
            ProjectileBehaviour projectileBehaviour = Projectile.GetComponent<ProjectileBehaviour>();
            _blueDamage += projectileBehaviour.baseDamage;
            if (_blueDamage > baseHealth)
            {
                _blueDamage = baseHealth;
            }
        }
        if (other.tag == "BreakWeapon")
        {
            Debug.Log("HIT BY WEAPON!");
        }
    }

    private void UpdateUiCooldownBar(GameObject cooldownBar, float readynessPercent)
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
            transform.position + new Vector3(ProjectileOrigin.x * _direction,
                                             ProjectileOrigin.y - size,
                                             ProjectileOrigin.z),
            transform.position + new Vector3(ProjectileOrigin.x * _direction,
                                             ProjectileOrigin.y + size,
                                             ProjectileOrigin.z)
            );
        Gizmos.DrawLine(
            transform.position + new Vector3(ProjectileOrigin.x * _direction - size,
                                             ProjectileOrigin.y,
                                             ProjectileOrigin.z),
            transform.position + new Vector3(ProjectileOrigin.x * _direction + size,
                                             ProjectileOrigin.y,
                                             ProjectileOrigin.z));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            transform.position + new Vector3(BreakOrigin.x * _direction,
                                             BreakOrigin.y,
                                             BreakOrigin.z),
            transform.position + new Vector3(BreakOrigin.x * _direction + breakReach * _direction,
                                             BreakOrigin.y,
                                             BreakOrigin.z)
            );
    }
    #endregion
}
