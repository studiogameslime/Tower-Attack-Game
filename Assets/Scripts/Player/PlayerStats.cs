using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Base Stats")]
    [SerializeField] private int baseDamageMin = 10;
    [SerializeField] private int baseDamageMax = 20;
    [SerializeField] private int baseHP = 100;
    [SerializeField] private int baseAttackRange = 8;

    [Header("Scaling Per Point (%)")]
    [Tooltip("כמה אחוז דמג' כל נקודת שדרוג מוסיפה (אופציה ב – ריבית דריבית)")]
    [SerializeField] private float damagePercentPerPoint = 3f;

    [Tooltip("כמה אחוז חיים כל נקודת שדרוג מוסיפה (ליניארי)")]
    [SerializeField] private float hpPercentPerPoint = 5f;

    // כמה נקודות הושקעו בכל סטאט
    private int _damagePoints;
    private int _hpPoints;

    // נקודות פנויות לשדרוג
    public int _availablePoints;

    // מפתחות ל-PlayerPrefs
    private const string DamagePointsKey = "DamagePoints";
    private const string HpPointsKey = "HpPoints";
    private const string AvailablePointsKey = "AvailablePoints";


    public int _calculatedMinDamage;
    public int _calculatedMaxDamage;
    public int _calculatedHP;
    public int _calculatedAttackRange;

    public int DamagePoints => _damagePoints;
    public int HpPoints => _hpPoints;
    public int AvailablePoints => _availablePoints;

    private void Awake()
    {
        // סינגלטון פשוט
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadFromPrefs();
    }

    private void LoadFromPrefs()
    {
        _damagePoints = PlayerPrefs.GetInt(DamagePointsKey, 0);
        _hpPoints = PlayerPrefs.GetInt(HpPointsKey, 0);
        _availablePoints = PlayerPrefs.GetInt(AvailablePointsKey, 0);
        CalculateDamageRange();
        CalculateHP();
    }

    private void SaveToPrefs()
    {
        PlayerPrefs.SetInt(DamagePointsKey, _damagePoints);
        PlayerPrefs.SetInt(HpPointsKey, _hpPoints);
        PlayerPrefs.SetInt(AvailablePointsKey, _availablePoints);
        PlayerPrefs.Save();
        CalculateDamageRange();
        CalculateHP();
    }

    /// <summary>
    /// דמג' לפי אופציה ב:
    /// כל נקודה מכפילה את הדמג' ב-(1 + אחוז השדרוג) בחזקה של כמות הנקודות.
    /// למשל 3% -> 1.03^points
    /// </summary>
    public void CalculateDamageRange()
    {
        // ריבית־דריבית
        float multiplier = Mathf.Pow(1f + damagePercentPerPoint / 100f, _damagePoints);

        _calculatedMinDamage = Mathf.RoundToInt(baseDamageMin * multiplier);
        _calculatedMaxDamage = Mathf.RoundToInt(baseDamageMax * multiplier);
    }

    /// <summary>
    /// חיים – השארתי ליניארי, אם תרצה גם ריבית־דריבית נחליף ל-Pow כמו בדמג'.
    /// </summary>
    public void CalculateHP()
    {
        float multiplier = 1f + _hpPoints * (hpPercentPerPoint / 100f);
        _calculatedHP = Mathf.RoundToInt(baseHP * multiplier);
    }

    /// <summary>
    /// לקרוא לזה כששחקן עולה רמה (נגיד כל רמה 5 נקודות)
    /// </summary>
    public void AddLevelPoints(int amount = 5)
    {
        _availablePoints += amount;
        SaveToPrefs();
    }

    /// <summary>
    /// לחיצה על Upgrade Damage
    /// </summary>
    public void UpgradeDamage()
    {
        if (_availablePoints <= 0)
            return;

        _availablePoints--;
        _damagePoints++;
        SaveToPrefs();
    }

    /// <summary>
    /// לחיצה על Upgrade HP
    /// </summary>
    public void UpgradeHP()
    {
        if (_availablePoints <= 0)
            return;

        _availablePoints--;
        _hpPoints++;
        SaveToPrefs();
    }

    public int GetRandomDamage()
    {
        return Random.Range(_calculatedMinDamage, _calculatedMaxDamage);
    }
}