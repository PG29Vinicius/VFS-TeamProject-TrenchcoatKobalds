using UnityEngine;

/// <summary>
/// Detects when the player enters or exits the attack range of an enemy.
/// Works with both MeleeEnemyController and RangeEnemyController.
/// </summary>
public class AttackRangeDetector : MonoBehaviour
{
    private MeleeEnemyController _meleeEnemyController;
    private RangeEnemyController _rangeEnemyController;

    /// <summary>
    /// Initializes the detector with a reference to a MeleeEnemyController.
    /// </summary>
    /// <param name="controller">The melee enemy controller to notify.</param>
    public void Initialize(MeleeEnemyController controller)
    {
        _meleeEnemyController = controller;
    }

    /// <summary>
    /// Initializes the detector with a reference to a RangeEnemyController.
    /// </summary>
    /// <param name="controller">The range enemy controller to notify.</param>
    public void InitializeForRangeEnemy(RangeEnemyController controller)
    {
        _rangeEnemyController = controller;
    }

    /// <summary>
    /// Detects when the player enters the attack range trigger and notifies the parent enemy controller.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_meleeEnemyController != null)
            {
                _meleeEnemyController.OnPlayerEnterAttackRange();
            }
            else if (_rangeEnemyController != null)
            {
                _rangeEnemyController.OnPlayerEnterAttackRange();
            }
        }
    }

    /// <summary>
    /// Detects when the player exits the attack range trigger and notifies the parent enemy controller.
    /// </summary>
    /// <param name="other">The collider that exited the trigger.</param>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_meleeEnemyController != null)
            {
                _meleeEnemyController.OnPlayerExitAttackRange();
            }
            else if (_rangeEnemyController != null)
            {
                _rangeEnemyController.OnPlayerExitAttackRange();
            }
        }
    }
}