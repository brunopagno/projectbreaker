using UnityEngine;

public class HeroControl : MonoBehaviour
{
    public Rigidbody body;

    public float speed = 2;
    public float acceleration = 200;
    public float jump = 450;
    public float fireRate = 4;

    public GameObject projectile;
    public Vector3 projectileOrigin;

    private bool _isJumping = false;
    private float _timeBetweenShoots;
    private float _timeToShoot;

    private void Start()
    {
        _timeBetweenShoots = fireRate / 60.0f;
    }

    private void FixedUpdate()
    {
        ResolveRun();
        ResolveJump();
        ResolveShoot();
        //ResolveBreak();
    }

    private void ResolveRun()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        body.AddForce(Vector3.right * horizontalInput * acceleration);
        if (Mathf.Abs(body.velocity.x) > speed)
        {
            body.velocity = body.velocity.normalized * speed;
        }
    }

    private void ResolveJump()
    {
        if (!_isJumping && Input.GetButtonDown("Jump"))
        {
            _isJumping = true;
            body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
            body.AddForce(Vector3.up * jump);
        }
    }

    private void ResolveShoot()
    {
        _timeToShoot += Time.deltaTime;
        if (Input.GetButton("Fire1") && _timeToShoot > _timeBetweenShoots)
        {
            Instantiate(projectile, transform.position + projectileOrigin, Quaternion.identity);
            _timeToShoot = 0;
        }
    }

    //private void ResolveBreak()
    //{
    //    // nothing here, yet
    //}

    private void OnTriggerEnter(Collider other)
    {
        _isJumping = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        float size = 0.2f;
        Gizmos.DrawLine(
            transform.position + projectileOrigin + new Vector3(0, -size, 0),
            transform.position + projectileOrigin + new Vector3(0, size, 0)
            );
        Gizmos.DrawLine(
            transform.position + projectileOrigin + new Vector3(-size, 0, 0),
            transform.position + projectileOrigin + new Vector3(size, 0, 0));
    }
}
