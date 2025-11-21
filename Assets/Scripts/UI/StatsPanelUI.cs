using TMPro;
using UnityEngine;

public class StatsPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI hpText;
   // [SerializeField] private TextMeshProUGUI pointsText;

    private void OnEnable()
    {
        Refresh();
    }

    private void Start()
    {
        // למקרה שהפאנל נטען פעיל מהתחלה
        Refresh();
    }

    private void Refresh()
    {
        if (PlayerStats.Instance == null)
            return;

        damageText.text = $"{PlayerStats.Instance._calculatedMinDamage}-{PlayerStats.Instance._calculatedMaxDamage}";
        hpText.text = $"{PlayerStats.Instance._calculatedHP}";
       // pointsText.text = $"Points: {PlayerStats.Instance.AvailablePoints}";
    }

    // לשים בפונקציית OnClick של כפתור Upgrade ליד הדמג'
    public void OnUpgradeDamageButton()
    {
        PlayerStats.Instance.UpgradeDamage();
        Refresh();
    }

    // לשים בפונקציית OnClick של כפתור Upgrade ליד ה-HP
    public void OnUpgradeHPButton()
    {
        PlayerStats.Instance.UpgradeHP();
        Refresh();
    }
}
