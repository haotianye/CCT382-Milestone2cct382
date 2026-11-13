using UnityEngine;

public class DustSpawner : MonoBehaviour
{
    public GameObject dustPrefab;
    public float speedThreshold = 3f;
    public float spawnInterval = 0.2f;
    public Vector3 footOffset = new Vector3(0f, -0.9f, 0f);

    private CharacterController controller;
    private float timer;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (controller == null || dustPrefab == null)
            return;

        Vector3 velocity = controller.velocity;
        velocity.y = 0f;

        if (controller.isGrounded &&
            velocity.magnitude > speedThreshold)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                Vector3 position =
                    transform.TransformPoint(footOffset);

                GameObject dust = Instantiate(
                    dustPrefab,
                    position,
                    Quaternion.identity
                );

                ParticleSystem ps =
                    dust.GetComponent<ParticleSystem>();

                if (ps != null)
                    ps.Play();

                Destroy(dust, 2f);

                timer = spawnInterval;
            }
        }
        else
        {
            timer = 0f;
        }
    }
}