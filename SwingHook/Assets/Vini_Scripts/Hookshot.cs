using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class Hookshot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform gunTip;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Hookshot Settings")]
    [SerializeField] private LayerMask hookshotableLayers;
    [SerializeField] private float maxHookshotDistance = 100f;
    [SerializeField] private bool useObjectCenter = true;
    [SerializeField] private KeyCode hookshotKey = KeyCode.Mouse1;

    [Header("Movement Settings")]
    [SerializeField] private float pullSpeed = 20f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private bool maintainMomentum = false;

    [Header("Visual Settings")]
    [SerializeField] private float ropeDrawspeed = 15f;

    private Vector3 hookshotPoint;
    private Vector3 currentRopePosition;
    private bool isHookshotting;
    private Vector3 pullDirection;
    private Rigidbody targetRigidbody;
    private Collider targetCollider;

    private void Awake()
    {
        ValidateComponents();
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        if (isHookshotting == true)
        {
            UpdateHookshotTarget();
            PullPlayerToTarget();
        }
    }

    private void LateUpdate()
    {
        UpdateRopeVisual();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(hookshotKey) && isHookshotting == false)
        {
            TryStartHookshot();
        }
        else if (Input.GetKeyUp(hookshotKey) && isHookshotting == true)
        {
            StopHookshot();
        }
    }

    private void TryStartHookshot()
    {
        RaycastHit hit;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward,
            out hit, maxHookshotDistance, hookshotableLayers) == false)
        {
            return;
        }

        isHookshotting = true;

        targetCollider = hit.collider;
        targetRigidbody = hit.rigidbody;

        Vector3 targetPoint = CalculateHookshotPoint(hit.collider);

        hookshotPoint = targetPoint;
        pullDirection = (hookshotPoint - playerRigidbody.position).normalized;

        if (maintainMomentum == false)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
        }

        InitializeRopeVisual();
    }

    private void UpdateHookshotTarget()
    {
        if (targetCollider != null)
        {
            hookshotPoint = CalculateHookshotPoint(targetCollider);
        }
    }

    private Vector3 CalculateHookshotPoint(Collider collider)
    {
        if (useObjectCenter && collider != null)
        {
            return collider.bounds.center;
        }

        return collider.ClosestPoint(playerRigidbody.position);
    }

    private void PullPlayerToTarget()
    {
        float distanceToTarget = Vector3.Distance(playerRigidbody.position, hookshotPoint);

        if (distanceToTarget <= stopDistance)
        {
            StopHookshot();
            return;
        }

        pullDirection = (hookshotPoint - playerRigidbody.position).normalized;

        ApplyPullForce(distanceToTarget);
    }

    private void ApplyPullForce(float distance)
    {
        float currentPullSpeed = Mathf.Lerp(pullSpeed * 0.5f, pullSpeed, distance / maxHookshotDistance);

        Vector3 targetVelocity = pullDirection * currentPullSpeed;
        playerRigidbody.linearVelocity = Vector3.Lerp(playerRigidbody.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
    }

    private void StopHookshot()
    {
        isHookshotting = false;
        targetCollider = null;
        targetRigidbody = null;
        lineRenderer.positionCount = 0;
    }

    private void InitializeRopeVisual()
    {
        lineRenderer.positionCount = 2;
        currentRopePosition = gunTip.position;
    }

    private void UpdateRopeVisual()
    {
        if (isHookshotting == false)
        {
            return;
        }

        currentRopePosition = Vector3.Lerp(currentRopePosition, hookshotPoint, Time.deltaTime * ropeDrawspeed);

        lineRenderer.SetPosition(0, gunTip.position);
        lineRenderer.SetPosition(1, currentRopePosition);
    }

    public bool IsHookshotting() => isHookshotting;

    public Vector3 GetHookshotPoint() => hookshotPoint;

    public void CancelHookshot()
    {
        if (!isHookshotting)
        {
            StopHookshot();
        }
    }

    private void ValidateComponents()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                Debug.LogError("Hookshot: LineRenderer component missing!");
            }
        }

        if (gunTip == null)
            Debug.LogError("Hookshot: Gun Tip transform not assigned!");

        if (cameraTransform == null)
            Debug.LogError("Hookshot: Camera transform not assigned!");

        if (playerRigidbody == null)
        {
            Debug.LogError("Hookshot: Player Rigidbody not assigned!");
        }
    }

    private void OnDrawGizmos()
    {
        if (isHookshotting == false)
        {
            return;
        }

        float gizmosRadius = 0.5f;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(hookshotPoint, gizmosRadius);

        if (playerRigidbody != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(playerRigidbody.position, hookshotPoint);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hookshotPoint, stopDistance);
        }
    }

}
