using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health _health;
    // We use RectTransform to change size (scale), not Image fill
    [SerializeField] private RectTransform _fillBar;

    private void Update()
    {
        if (_health == null || _fillBar == null) return;

        // This shrinks the bar by changing its Scale X
        _fillBar.localScale = new Vector3(_health.HeathPercentage, 1, 1);
    }
}