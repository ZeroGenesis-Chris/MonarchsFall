using UnityEngine;

public class GrappleMechanic : MonoBehaviour
{
    public float grappleSpeed = 10f;
    public float maxGrappleDistance = 10f;
    public LayerMask grappleableLayerMask;
    public LayerMask draggableLayerMask;
    public float swingForce = 5f;
    public float minGrappleLength = 1f;
    public float pullThreshold = 0.7f; // Threshold for initiating a pull
    public float dragObjectSpeed = 5f; // Speed at which objects are dragged
    public float minDragDistance = 2f; // Minimum distance required between player and object to drag

    private bool isGrappling = false;
    private bool isPulling = false;
    private bool isDraggingObject = false;
    private Vector2 grapplePoint;
    private LineRenderer lineRenderer;
    private Rigidbody2D playerRb;
    private SpringJoint2D springJoint;
    private float grappleLength;
    private Rigidbody2D draggedObjectRb;
    private bool canDrag = false;
    private Collider2D playerCollider;
    
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.positionCount = 0;

        playerRb = GetComponent<Rigidbody2D>();
        if (playerRb == null)
        {
            playerRb = gameObject.AddComponent<Rigidbody2D>();
        }
        playerRb.gravityScale = 1f;
        playerRb.freezeRotation = true;

        playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            playerCollider = gameObject.AddComponent<BoxCollider2D>();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ReleaseGrapple();
        }

        if (isGrappling)
        {
            DrawGrappleLine();
            
            Vector2 inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            Vector2 grappleDirection = (grapplePoint - (Vector2)transform.position).normalized;

            float dotProduct = Vector2.Dot(inputDirection, grappleDirection);

            // Check if player is inputting strongly in the direction of the grapple
            if (dotProduct > pullThreshold && !isDraggingObject)
            {
                isPulling = true;
            }
            else
            {
                isPulling = false;
                SetupSwingJoint();
            }

            // Check for 'R' key press to start dragging
            if (Input.GetKeyDown(KeyCode.R) && canDrag && !IsPlayerTooClose())
            {
                isDraggingObject = true;
                isPulling = false;
            }

            // Apply swing force if not pulling or dragging
            if (!isPulling && !isDraggingObject)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    playerRb.AddForce(Vector2.left * swingForce);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    playerRb.AddForce(Vector2.right * swingForce);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (isGrappling)
        {
            if (isPulling)
            {
                PullPlayerTowardsPoint();
            }
            else if (isDraggingObject && !IsPlayerTooClose())
            {
                DragObjectToPlayer();
            }
            else
            {
                // Adjust the grapple length based on vertical input
                if (Input.GetKey(KeyCode.W) && grappleLength > minGrappleLength)
                {
                    grappleLength -= 0.1f;
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    grappleLength += 0.1f;
                }
                springJoint.distance = grappleLength;
            }
        }
    }

    private bool IsPlayerTooClose()
    {
        if (draggedObjectRb != null)
        {
            float distance = Vector2.Distance(transform.position, draggedObjectRb.position);
            return distance < minDragDistance || playerCollider.IsTouching(draggedObjectRb.GetComponent<Collider2D>());
        }
        return false;
    }

    private void StartGrapple()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxGrappleDistance, grappleableLayerMask | draggableLayerMask);

        if (hit.collider != null)
        {
            grapplePoint = hit.point;
            grappleLength = Vector2.Distance(transform.position, grapplePoint);
            isGrappling = true;
            isPulling = false;
            isDraggingObject = false;
            lineRenderer.positionCount = 2;

            // Check if the hit object is on the draggable layer
            if (((1 << hit.collider.gameObject.layer) & draggableLayerMask) != 0)
            {
                canDrag = true;
                draggedObjectRb = hit.collider.GetComponent<Rigidbody2D>();
                if (draggedObjectRb == null)
                {
                    draggedObjectRb = hit.collider.gameObject.AddComponent<Rigidbody2D>();
                }
            }
            else
            {
                canDrag = false;
            }

            SetupSwingJoint();
        }
    }

    private void SetupSwingJoint()
    {
        if (springJoint == null)
        {
            springJoint = gameObject.AddComponent<SpringJoint2D>();
        }
        springJoint.connectedAnchor = grapplePoint;
        springJoint.distance = grappleLength;
        springJoint.frequency = 1f;
        springJoint.dampingRatio = 0.5f;
        springJoint.autoConfigureConnectedAnchor = false;
        springJoint.enabled = true;
    }

    private void PullPlayerTowardsPoint()
    {
        springJoint.enabled = false;
        Vector2 grappleDirection = (grapplePoint - (Vector2)transform.position).normalized;
        playerRb.velocity = grappleDirection * grappleSpeed;
    }

    private void DragObjectToPlayer()
    {
        if (draggedObjectRb != null)
        {
            Vector2 dragDirection = ((Vector2)transform.position - draggedObjectRb.position).normalized;
            draggedObjectRb.velocity = dragDirection * dragObjectSpeed;

            // Update grapple point to the current position of the dragged object
            grapplePoint = draggedObjectRb.position;
        }
    }

    private void DrawGrappleLine()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }


    private void ReleaseGrapple()
    {
        isGrappling = false;
        isPulling = false;
        isDraggingObject = false;
        canDrag = false;
        lineRenderer.positionCount = 0;
        if (springJoint != null)
        {
            springJoint.enabled = false;
        }
        draggedObjectRb = null;
    }
}