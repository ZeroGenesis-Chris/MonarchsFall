using UnityEngine;

public class LedgeGrabSystem : MonoBehaviour
{
    [SerializeField] private LayerMask ledgeLayer;
    [SerializeField] private float grabDistance = 0.7f;
    [SerializeField] private float climbSpeed = 2f;
    [SerializeField] private Vector2 grabOffset = new Vector2(0.25f, 0.5f);

    private bool isGrabbingLedge = false;
    private Vector2 ledgePosition;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (!isGrabbingLedge)
        {
            CheckForLedge();
        }
        else
        {
            HandleLedgeGrab();
        }
    }

    private void CheckForLedge()
    {
        Vector2 raycastOrigin = (Vector2)transform.position + new Vector2(boxCollider.offset.x, boxCollider.offset.y + boxCollider.size.y / 2);
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.right * transform.localScale.x, grabDistance, ledgeLayer);

        if (hit.collider != null)
        {
            ledgePosition = hit.point;
            isGrabbingLedge = true;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
        }
    }

    private void HandleLedgeGrab()
    {
        // Position the character relative to the ledge
        transform.position = ledgePosition - grabOffset;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Climb up
            StartCoroutine(ClimbLedge());
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            // Let go of ledge
            ReleaseLedge();
        }
    }

    private System.Collections.IEnumerator ClimbLedge()
    {
        Vector2 startPos = transform.position;
        Vector2 endPos = ledgePosition + Vector2.up * 0.5f; // Adjust this offset as needed

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * climbSpeed;
            transform.position = Vector2.Lerp(startPos, endPos, elapsedTime);
            yield return null;
        }

        ReleaseLedge();
    }

    private void ReleaseLedge()
    {
        isGrabbingLedge = false;
        rb.gravityScale = 1; // Reset to default gravity
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 raycastOrigin = (Vector2)transform.position + new Vector2(boxCollider.offset.x, boxCollider.offset.y + boxCollider.size.y / 2);
        Gizmos.DrawLine(raycastOrigin, raycastOrigin + Vector2.right * transform.localScale.x * grabDistance);
    }
}