using UnityEngine;

public class GrapplingGun : MonoBehaviour 
{
    [Header("References")]
    [SerializeField] private Transform gunTip;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Grapple Settings")]
    [SerializeField] private LayerMask grappleableLayers;
    [SerializeField] private float maxGrappleDistance = 100f;
    [SerializeField] private bool useObjectCenter = true;

    [Header("Physics Settings")]
    [SerializeField] private float playerMass = 100f;
    [SerializeField] private float springForce = 4.5f;
    [SerializeField] private float damperForce = 7f;
    [SerializeField] private float maxDistanceMultiplier = 0.4f;
    [SerializeField] private float minDistanceMultiplier = 0.25f;

    [Header("Visual Settings")]
    [SerializeField] private float ropeDrawSpeed = 8f;

    private SpringJoint activeJoint;
    private Vector3 grapplePoint;
    private Vector3 currentRopePosition;
    private bool isGrappling;

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

    #region Input Handling

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && isGrappling == false)
        {
            TryStartGrapple();
        }
        else if (Input.GetMouseButtonUp(0) && isGrappling == true)
        {
            StopGrapple();
        }
    }

    #endregion

    #region Grapple Logic

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

    private Vector3 CalculateGrapplePoint(RaycastHit hit)
    {
        if (useObjectCenter && hit.collider != null)
        {
            return hit.collider.bounds.center;
        }

        return hit.point;
    }

    private void CreateGrappleJoint(Vector3 targetPoint)
    {
        grapplePoint = targetPoint;

        activeJoint = playerTransform.gameObject.AddComponent<SpringJoint>();
        activeJoint.autoConfigureConnectedAnchor = false;
        activeJoint.connectedAnchor = grapplePoint;

        float distance = Vector3.Distance(playerTransform.position, grapplePoint);
        ConfigureJointPhysics(activeJoint, distance);
    }

    private void ConfigureJointPhysics(SpringJoint joint, float distance)
    {
        joint.maxDistance = distance * maxDistanceMultiplier;
        joint.minDistance = distance * minDistanceMultiplier;
        joint.spring = springForce;
        joint.damper = damperForce;
        joint.massScale = playerMass;
    }

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

    #endregion

    #region Rope Visual

    private void InitializeRopeVisual()
    {
        lineRenderer.positionCount = 2;
        currentRopePosition = gunTip.position;
    }

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

    #endregion

    #region Public Accesses

    public bool IsGrappling() => isGrappling;

    public Vector3 GetGrapplePoint() => grapplePoint;

    public bool HasActiveJoint() => activeJoint != null;

    #endregion

    #region Validation

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

    #endregion

    #region Debug

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

    #endregion

}
