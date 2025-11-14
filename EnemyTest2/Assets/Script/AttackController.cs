using UnityEngine;

public class AttackController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _kickHitbox;
    [SerializeField] private GameObject _punchHitbox;

    [Header("Attack Settings")]
    [SerializeField] private float _kickDuration = 0.2f;
    [SerializeField] private float _punchDuration = 0.15f;
    [SerializeField] private float _kickForce = 80f;
    [SerializeField] private float _punchForce = 50f;


    private bool _canAttack = true;

    void Start()
    {
        if (_kickHitbox == null) _kickHitbox = GameObject.Find("KickHitbox");
        if (_punchHitbox == null) _punchHitbox = GameObject.Find("PunchHitbox");

        if (_kickHitbox != null) _kickHitbox.SetActive(false);
        if (_punchHitbox != null) _punchHitbox.SetActive(false);
    }

    void Update()
    {
        if (!_canAttack) return;

        // F = Kick
        if (Input.GetKeyDown(KeyCode.F))
        {
            Kick();
            Debug.Log("Kick!!");
        }

        // Left Click = Punch
        if (Input.GetKeyDown(KeyCode.G))
        {
            Punch();
            Debug.Log("Punch!!");
        }
    }

    // Kick attack
    void Kick()
    {
        if (_kickHitbox == null) return;

        _canAttack = false;
        _kickHitbox.SetActive(true);
        Invoke(nameof(EndKick), _kickDuration);
    }

    void EndKick()
    {
        if (_kickHitbox != null) _kickHitbox.SetActive(false);
        _canAttack = true;
    }

    // Punch attack
    void Punch()
    {
        if (_punchHitbox == null) return;

        _canAttack = false;
        _punchHitbox.SetActive(true);
        Invoke(nameof(EndPunch), _punchDuration);
    }

    void EndPunch()
    {
        if (_punchHitbox != null) _punchHitbox.SetActive(false);
        _canAttack = true;
    }


    public float GetKickForce() {  return _kickForce; }
    public float GetPunchForce() {  return _punchForce; }

}