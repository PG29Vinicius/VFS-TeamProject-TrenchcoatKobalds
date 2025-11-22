using UnityEngine;

public class GrapplingGun : MonoBehaviour 
{
    #region Variables

    [Header("References")]
    [SerializeField][Tooltip("The object that will be used as the starting point for tracing the Grappling Hook rope.")] private Transform gunTip;
    [SerializeField][Tooltip("The camera parent object transform. Do not drag the camera itself here.")] private Transform cameraTransform;
    [SerializeField][Tooltip("The player parent object transform. Do not drag any player's child objects here.")] private Transform playerTransform;
    [SerializeField][Tooltip("The component responsible for drawing the rope of the Grappling Hook.")] private LineRenderer lineRenderer;

    [Header("Grapple Settings")]
    [SerializeField][Tooltip("The layers that the grappling hook can be used on. You may choose more than one layer.")] private LayerMask grappleableLayers;
    [SerializeField][Tooltip("The maximum distance the player can grapple objects.")] private float maxGrappleDistance = 100f;
    [SerializeField][Tooltip("Check this if you want the Grappling Hook to grapple in the center of the object. Uncheck otherwise.")] private bool useObjectCenter = true;
    [SerializeField][Tooltip("Check this if you want the Grappling Hook to grapple in the center of the object. Uncheck otherwise.")] private KeyCode grappleKey = KeyCode.Mouse0;

    [Header("Physics Settings")]
    [SerializeField][Tooltip("The weight of the player used to configure the Spring Joint component.")] private float playerMass = 100f;
    [SerializeField][Tooltip("The force between the Player and the grappled object. This is being used to keep them together.")] private float springForce = 4.5f;
    [SerializeField][Tooltip("The force used to absorb, or dampen, the spring force.")] private float damperForce = 7f;
    [SerializeField][Tooltip("The maximum distance between the Player and the grappled object.")] private float maxDistanceMultiplier = 0.4f;
    [SerializeField][Tooltip("The minimum distance between the Player and the grappled object.")] private float minDistanceMultiplier = 0.25f;

    [Header("Visual Settings")]
    [SerializeField][Tooltip("The speed at which the rope is traced towards the grappled object.")] private float ropeDrawSpeed = 8f;

    private SpringJoint activeJoint;
    private Vector3 grapplePoint;
    private Vector3 currentRopePosition;
    private bool isGrappling;

    #endregion

    #region Getters
    public bool IsGrappling() => isGrappling;

    public Vector3 GetGrapplePoint() => grapplePoint;

    public bool HasActiveJoint() => activeJoint != null;

    #endregion

    private void Awake()
    {
        ValidateComponents();
    }

    private void Update()
    {
        HandleInput();
    }

    private void LateUpdate()
    {
        UpdateRopeVisual();
    }

    /// <summary>
    /// Checks the Input for the grappling action. Start or stop the grapple based on the Player action.
    /// </summary>
    private void HandleInput()
    {
        if (Input.GetKeyDown(grappleKey) && isGrappling == false)
        {
            TryStartGrapple();
        }
        else if (Input.GetKeyUp(grappleKey) && isGrappling == true)
        {
            StopGrapple();
        }
    }
    
    /// <summary>
    /// Checks the object that was hit by a Raycast, then tries to draw the rope towards the object that was detected in the RaycastHit.
    /// </summary>
    private void TryStartGrapple()
    {
        RaycastHit hit;

        if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, 
            out hit, maxGrappleDistance, grappleableLayers) == false)
        {
            return;
        }

        isGrappling = true;

        Vector3 targetPoint = CalculateGrapplePoint(hit);
        CreateGrappleJoint(targetPoint);
        InitializeRopeVisual();
    }

    /// <summary>
    /// Calculates where to draw the rope based on where the Raycast hit the object.
    /// </summary>
    /// <param name="hit">The RaycastHit parameter, used to return where the Raycast hit the object.</param>
    /// <returns></returns>
    private Vector3 CalculateGrapplePoint(RaycastHit hit)
    {
        if (useObjectCenter && hit.collider != null)
        {
            return hit.collider.bounds.center;
        }

        return hit.point;
    }

    /// <summary>
    /// Creates the Spring Joint component to deal with the physics based movement.
    /// </summary>
    /// <param name="targetPoint">The object that was grappled.</param>
    private void CreateGrappleJoint(Vector3 targetPoint)
    {
        grapplePoint = targetPoint;

        activeJoint = playerTransform.gameObject.AddComponent<SpringJoint>();
        activeJoint.autoConfigureConnectedAnchor = false;
        activeJoint.connectedAnchor = grapplePoint;

        float distance = Vector3.Distance(playerTransform.position, grapplePoint);
        ConfigureJointPhysics(activeJoint, distance);
    }

    /// <summary>
    /// Sets the Spring Joint physics variables.
    /// </summary>
    /// <param name="joint">A reference to the Spring Joint component.</param>
    /// <param name="distance">The minimum and maximum distance between the Player and the grappled object.</param>
    private void ConfigureJointPhysics(SpringJoint joint, float distance)
    {
        joint.maxDistance = distance * maxDistanceMultiplier;
        joint.minDistance = distance * minDistanceMultiplier;
        joint.spring = springForce;
        joint.damper = damperForce;
        joint.massScale = playerMass;
    }

    /// <summary>
    /// Destroys the Spring Joint component when the player stops the grapple.
    /// </summary>
    private void StopGrapple()
    {
        if (activeJoint != null)
        {
            Destroy(activeJoint);
            activeJoint = null;
        }

        lineRenderer.positionCount = 0;
        isGrappling = false;
    }

    /// <summary>
    /// Starts drawing the rope of the grappling hook using a Line Renderer component.
    /// </summary>
    private void InitializeRopeVisual()
    {
        lineRenderer.positionCount = 2;
        currentRopePosition = gunTip.position;
    }

    /// <summary>
    /// Sets the origin and the destination of the rope with the Line Renderer component.
    /// </summary>
    private void UpdateRopeVisual()
    {
        if (isGrappling == false || activeJoint == null)
        {
            return;
        }

        currentRopePosition = Vector3.Lerp(currentRopePosition, grapplePoint, ropeDrawSpeed * Time.deltaTime);

        lineRenderer.SetPosition(0, gunTip.position);
        lineRenderer.SetPosition(1, currentRopePosition);
    }

    /// <summary>
    /// Checks if the components are null and tries to get them again, to avoid null references.
    /// </summary>
    private void ValidateComponents()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();

            if (lineRenderer == null)
            {
                Debug.LogError("GrapplingGun: LineRenderer component missing!");
            }
        }

        if (gunTip == null)
        {
            Debug.LogError("GrapplingGun: Gun Tip transform not assigned!");
        }

        if (cameraTransform == null)
        {
            Debug.LogError("GrapplingGun: Camera transform not assigned!");
        }

        if (playerTransform == null)
        {
            Debug.LogError("GrapplingGun: Player transform not assigned!");
        }
    }

    // A debug function to check collisions with Gizmos in the Unity Editor.
    private void OnDrawGizmos()
    {
        if (isGrappling == false)
        {
            return;
        }

        float radius = 0.5f;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(grapplePoint, radius);

        if (playerTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(playerTransform.position, grapplePoint);
        }
    }
}
