using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float speed = 8;

    private float _limit = 40;

    private void Update()
    {
        transform.position += Vector3.right * Time.deltaTime * speed;

        if (Mathf.Abs(transform.position.x) > _limit)
        {
            Destroy(this.gameObject);
        }
    }
}
