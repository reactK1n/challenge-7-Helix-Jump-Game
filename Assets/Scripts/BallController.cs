using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private bool ignoreNextCollision;

    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private float impulseForce;

    [SerializeField]
    private GameObject trial;

    private Vector3 startPos;

    private Vector3 lastRotation;

    void Awake()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float maxDistance = .5f;
        float radius = .5f;
        var obstructed = Physics.SphereCast(transform.position, radius, Vector3.down, out RaycastHit rayCastHit, maxDistance);
        if (obstructed)
        {
            if (rayCastHit.transform.TryGetComponent(out Goal goal))
            {
                StartCoroutine(PauseTrialForNextStageRoutine());
                GameManager.Instance.NextLevel();
            }
        }
    }

    IEnumerator PauseTrialForNextStageRoutine()
    {
        trial.SetActive(false);

        yield return new WaitForSeconds(.3f);

        trial.SetActive(true);
    }

    // Update is called once per frame
    public void ResetBall()
    {
        transform.position = startPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (ignoreNextCollision) return;

        var helix = FindObjectOfType<Helix>();

        var deathPart = collision.transform.GetComponent<DeathPart>();

        if (deathPart)
        {
            deathPart.HitDeathPath();
            helix.ResetHelixToPreviousState();
        }

        helix.SetLastRotation();

        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * impulseForce, ForceMode.Impulse);

        ignoreNextCollision = true;

        Invoke(nameof(AllowCollision), .2f);
    }

    private void AllowCollision()
    {
        ignoreNextCollision = false;
    }
}
