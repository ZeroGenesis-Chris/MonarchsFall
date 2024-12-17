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
    
    /// <summary>
    /// Setup the line renderer, rigidbody, and collider at the start of the game.
    /// </summary>
    private void Start()
    {
        // Find the line renderer component
        lineRenderer = GetComponent<LineRenderer>();

        // If a line renderer doesn't exist, add one
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Set the initial position count of the line renderer to 0
        lineRenderer.positionCount = 0;

        // Find the rigidbody component
        playerRb = GetComponent<Rigidbody2D>();

        // If a rigidbody doesn't exist, add one
        if (playerRb == null)
        {
            playerRb = gameObject.AddComponent<Rigidbody2D>();
        }

        // Set the gravity scale of the rigidbody to 1
        playerRb.gravityScale = 1f;

        // Freeze the rotation of the rigidbody
        playerRb.freezeRotation = true;

        // Find the collider component
        playerCollider = GetComponent<Collider2D>();

        // If a collider doesn't exist, add a box collider
        if (playerCollider == null)
        {
            playerCollider = gameObject.AddComponent<BoxCollider2D>();
        }
    }

    private void Update()
    {
        // Check if the left mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            // Start the grapple
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Release the grapple
            ReleaseGrapple();
        }

        // If the player is currently grappling
        if (isGrappling)
        {
            // Draw the grapple line
            DrawGrappleLine();
            
            // Get the direction of the player's input
            Vector2 inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            // Get the direction from the player to the grapple point
            Vector2 grappleDirection = (grapplePoint - (Vector2)transform.position).normalized;

            // Calculate the dot product of the input direction and the grapple direction
            float dotProduct = Vector2.Dot(inputDirection, grappleDirection);

            // Check if the player is inputting strongly in the direction of the grapple
            if (dotProduct > pullThreshold && !isDraggingObject)
            {
                // Player is pulling, so set the isPulling flag
                isPulling = true;
            }
            else
            {
                // Player is not pulling, so set the isPulling flag to false
                isPulling = false;
                // Set up the swing joint
                SetupSwingJoint();
            }

            // Check for the 'R' key press to start dragging
            if (Input.GetKeyDown(KeyCode.R) && canDrag && !IsPlayerTooClose())
            {
                // Player is dragging, so set the isDraggingObject flag
                isDraggingObject = true;
                // Set the isPulling flag to false
                isPulling = false;
            }

            // If the player is not pulling or dragging
            if (!isPulling && !isDraggingObject)
            {
                // Apply a force in the direction of the player's input
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
        // Check if the player is currently grappling
        if (isGrappling)
        {
            // Check if the player is pulling towards the grapple point
            if (isPulling)
            {
                // Pull the player towards the grapple point
                PullPlayerTowardsPoint();
            }
            // Check if the player is dragging an object and not too close to it
            else if (isDraggingObject && !IsPlayerTooClose())
            {
                // Drag the object towards the player
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

    /// <summary>
    /// Checks if the player is too close to the dragged object.
    /// The player is considered too close if the distance between the player and the object is less than
    /// the minimum drag distance, or if the player's collider is touching the object's collider.
    /// </summary>
    /// <returns>True if the player is too close, false otherwise.</returns>
    private bool IsPlayerTooClose()
    {
        // If there is no dragged object, the player is not too close
        if (draggedObjectRb == null)
        {
            return false;
        }

        // Calculate the distance between the player and the dragged object
        float distance = Vector2.Distance(transform.position, draggedObjectRb.position);

        // If the distance is less than the minimum drag distance, the player is too close
        if (distance < minDragDistance)
        {
            return true;
        }

        // If the player's collider is touching the object's collider, the player is too close
        if (playerCollider.IsTouching(draggedObjectRb.GetComponent<Collider2D>()))
        {
            return true;
        }

        // If none of the above conditions are true, the player is not too close
        return false;
    }

    private void StartGrapple()
    {
        // Get the position of the mouse in world coordinates
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the direction from the player to the mouse position
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;

        // Raycast from the player to the mouse position, checking for both grappleable and draggable objects
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxGrappleDistance, grappleableLayerMask | draggableLayerMask);

        // If we hit something
        if (hit.collider != null)
        {
            // Set the grapple point to the hit point
            grapplePoint = hit.point;

            // Calculate the length of the grapple
            grappleLength = Vector2.Distance(transform.position, grapplePoint);

            // Set the grapple flag to true
            isGrappling = true;

            // Set the pulling flag to false
            isPulling = false;

            // Set the dragging flag to false
            isDraggingObject = false;

            // Set the line renderer to have two points
            lineRenderer.positionCount = 2;

            // Check if the hit object is on the draggable layer
            if (((1 << hit.collider.gameObject.layer) & draggableLayerMask) != 0)
            {
                // If it is, set the canDrag flag to true
                canDrag = true;

                // Get the rigidbody of the hit object
                draggedObjectRb = hit.collider.GetComponent<Rigidbody2D>();

                // If the object doesn't have a rigidbody, add one
                if (draggedObjectRb == null)
                {
                    draggedObjectRb = hit.collider.gameObject.AddComponent<Rigidbody2D>();
                }
            }
            else
            {
                // If it's not, set the canDrag flag to false
                canDrag = false;
            }

            // Set up the spring joint
            SetupSwingJoint();
        }
    }

    /// <summary>
    /// Sets up the spring joint that is used for swinging from a grapple point.
    /// </summary>
    private void SetupSwingJoint()
    {
        // If the spring joint doesn't exist, add one
        if (springJoint == null)
        {
            springJoint = gameObject.AddComponent<SpringJoint2D>();
        }

        // Set the connected anchor of the spring joint to the grapple point
        springJoint.connectedAnchor = grapplePoint;

        // Set the distance of the spring joint to the length of the grapple
        springJoint.distance = grappleLength;

        // Set the frequency of the spring joint to a value that will make the player swing
        // back and forth at a reasonable speed
        springJoint.frequency = 1f;

        // Set the damping ratio of the spring joint to a value that will make the swing
        // feel smooth and natural
        springJoint.dampingRatio = 0.5f;

        // Set the autoConfigureConnectedAnchor flag of the spring joint to false, so
        // that we can manually set the connected anchor of the spring joint to the
        // grapple point
        springJoint.autoConfigureConnectedAnchor = false;

        // Enable the spring joint so that it can start swinging the player
        springJoint.enabled = true;
    }

    /// <summary>
    /// Pulls the player towards the grapple point.
    /// 
    /// This method is called when the player is pulling towards the grapple point.
    /// It works by disabling the spring joint (which is used for swinging from
    /// a grapple point) and setting the player's velocity to a direction that is
    /// towards the grapple point.
    /// 
    /// The speed at which the player moves towards the grapple point is determined
    /// by the grappleSpeed variable.
    /// </summary>
    private void PullPlayerTowardsPoint()
    {
        // Disable the spring joint so that it doesn't interfere with the player's movement
        springJoint.enabled = false;

        // Calculate the direction from the player's current position to the grapple point
        Vector2 grappleDirection = (grapplePoint - (Vector2)transform.position).normalized;

        // Set the player's velocity to the direction of the grapple point, at the grapple speed
        playerRb.velocity = grappleDirection * grappleSpeed;
    }

    /// <summary>
    /// Drags the object that the player is currently dragging towards the player.
    /// 
    /// This method is called when the player is dragging an object. It works by
    /// setting the velocity of the object's Rigidbody2D to a direction that is
    /// towards the player, at the drag object speed.
    /// 
    /// The grapple point is also updated to the current position of the dragged
    /// object.
    /// </summary>
    private void DragObjectToPlayer()
    {
        if (draggedObjectRb != null)
        {
            // Calculate the direction from the dragged object's current position to the player's position
            Vector2 dragDirection = ((Vector2)transform.position - draggedObjectRb.position).normalized;

            // Set the velocity of the dragged object's Rigidbody2D to the drag direction, at the drag object speed
            draggedObjectRb.velocity = dragDirection * dragObjectSpeed;

            // Update the grapple point to the current position of the dragged object
            grapplePoint = draggedObjectRb.position;
        }
    }

    private void DrawGrappleLine()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }


    /// <summary>
    /// Releases the grapple, stopping all grapple-related movement and disabling the spring joint.
    /// 
    /// This method is called when the player releases the grapple (i.e. when the left mouse button is released).
    /// 
    /// It sets the isGrappling, isPulling, and isDraggingObject flags to false, and sets the canDrag flag to false.
    /// 
    /// It also disables the spring joint and sets the position count of the line renderer to 0, effectively "hiding" the grapple line.
    /// 
    /// Finally, it sets the draggedObjectRb field to null, which is a reference to the Rigidbody2D of the object that the player is currently dragging.
    /// </summary>
    private void ReleaseGrapple()
    {
        // Set the isGrappling, isPulling, and isDraggingObject flags to false, indicating that the player is no longer grappling
        isGrappling = false;
        isPulling = false;
        isDraggingObject = false;

        // Set the canDrag flag to false, indicating that the player can no longer drag objects
        canDrag = false;

        // Disable the spring joint, which is used to simulate the player's grapple movement
        if (springJoint != null)
        {
            springJoint.enabled = false;
        }

        // Set the position count of the line renderer to 0, effectively "hiding" the grapple line
        lineRenderer.positionCount = 0;

        // Set the draggedObjectRb field to null, indicating that the player is no longer dragging an object
        draggedObjectRb = null;
    }
}