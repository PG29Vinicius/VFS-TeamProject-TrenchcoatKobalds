using UnityEngine;

public class AttackController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _hitbox;

    [Header("Attack Settings")]
    [SerializeField] private float _kickDuration = 0.2f;
    [SerializeField] private float _punchDuration = 0.15f;

    private bool _canAttack = true;

    void Start()
    {
        if (_hitbox == null) _hitbox = GameObject.Find("Hitbox");
        if (_hitbox != null) _hitbox.SetActive(false);
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
        if (_hitbox == null) return;

        _canAttack = false;
        _hitbox.SetActive(true);
        Invoke(nameof(EndKick), _kickDuration);
    }

    void EndKick()
    {
        if (_hitbox != null) _hitbox.SetActive(false);
        _canAttack = true;
    }

    // Punch attack
    void Punch()
    {
        if (_hitbox == null) return;

        _canAttack = false;
        _hitbox.SetActive(true);
        Invoke(nameof(EndPunch), _punchDuration);
    }

    void EndPunch()
    {
        if (_hitbox != null) _hitbox.SetActive(false);
        _canAttack = true;
    }
}