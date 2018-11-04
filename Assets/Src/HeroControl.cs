using System;
using UnityEngine;

public class HeroControl : MonoBehaviour
{
    private int _heroId = -1;
    private static int _nextHeroId = 1;

    private CharacterController _controller;
    private InputController _inputController;
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
    public float executeThreshold = 5f;

    private bool _isDead = false;
    private float _currentHealth;

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
        _heroId = _nextHeroId;
        _nextHeroId += 1;

        _inputController = new InputController();

        _velocity = new Vector3();
        _controller = GetComponent<CharacterController>();
        _projectileCooldown = new Cooldown(1f / fireRate);
        _breakCooldown = new Cooldown(1f / breakFreq);

        _currentHealth = baseHealth;
    }

    private void Update()
    {
        if (!_isDead)
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
            Death();
        }
    }

    private void Gravity()
    {
        _velocity.y -= gravity;
    }

    private void Run()
    {
        float horizontalInput = _inputController.GetAxis("Horizontal");

        _velocity.x += horizontalInput * speed;
    }

    private void Jump()
    {
        if (_controller.isGrounded && _inputController.GetButtonDown("Jump"))
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
        if (_inputController.GetButton("Fire1") && _projectileCooldown.Ready)
        {
            GameObject instantiatedProjectile = Instantiate(
                Projectile,
                transform.position + new Vector3(
                    ProjectileOrigin.x * _direction,
                    ProjectileOrigin.y,
                    ProjectileOrigin.z),
                Quaternion.identity);
            ProjectileBehaviour projectileBehaviour = instantiatedProjectile.GetComponent<ProjectileBehaviour>();
            projectileBehaviour.OwnerId = _heroId;
            projectileBehaviour.Direction = _direction;

            _projectileCooldown.Reset();
        }

        UpdateUiCooldownBar(ProjectileCooldown, _projectileCooldown.Readyness);
    }

    private void Break()
    {
        _breakCooldown.Update();
        if (_inputController.GetButtonDown("Fire2") && _breakCooldown.Ready)
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
        if (_currentHealth <= executeThreshold)
        {
            Animator animator = HealthBar.GetComponent<Animator>();
            animator.SetBool("ExecuteHealth", true);
        }

        float currentHealthPercentual = _currentHealth / baseHealth;
        UpdateUiCooldownBar(BlueDamage, currentHealthPercentual);
        UpdateUiCooldownBar(HealthBar, currentHealthPercentual - _blueDamage / baseHealth);
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
        // ignore collisions with self
        HeroControl heroControl = other.GetComponentInParent<HeroControl>();
        ProjectileBehaviour projectileBehaviour = other.GetComponent<ProjectileBehaviour>();
        if (heroControl != null && heroControl._heroId == _heroId ||
            projectileBehaviour != null && projectileBehaviour.OwnerId == _heroId)
        {
            return;
        }

        if (other.tag == "Projectile")
        {
            _blueDamage += projectileBehaviour.baseDamage;
            if (_blueDamage > _currentHealth)
            {
                _blueDamage = _currentHealth;
            }
        }
        if (other.tag == "BreakWeapon")
        {
            if (_currentHealth <= executeThreshold)
            {
                _currentHealth = 0;
                _blueDamage = 0;
            }
            else
            {
                _currentHealth -= _blueDamage;
                _blueDamage = 0;
                if (_currentHealth < 0)
                {
                    _currentHealth = 0;
                }
            }
        }
    }

    private void Death()
    {
        if (_currentHealth <= 0)
        {
            _isDead = true;
            Animator heroAnimator = GetComponent<Animator>();
            heroAnimator.SetBool("IsDead", _isDead);
            BreakWeapon.SetActive(false);
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
            transform.position + new Vector3(BreakOrigin.x * _direction + 1.5f * _direction,
                                             BreakOrigin.y,
                                             BreakOrigin.z)
            );
    }
    #endregion
}
