using UnityEngine;
using System;

public class StepCounter : MonoBehaviour
{
    // ===== Public API =====

    // Total steps across all sessions (since the player started using the game)
    public int TotalStepsSinceStart => totalSteps;

    // Steps accumulated since the last time you called ConsumeSteps()
    public int StepsSinceLastCheck => stepsSinceLastCheck;

    // True after we received the first sensor event and initialized from prefs (for STEP_COUNTER)
    public bool IsInitialized { get; private set; }

    // Optional: you can bind a TMP_Text here to see status on screen (not required)
    [Header("Debug (optional)")]
    [SerializeField] private TMPro.TMP_Text debugText;

    // ===== Internal state =====

    private int totalSteps;             // Total steps (persistent across sessions)
    private int lastRawStepValue = -1;  // Last raw value from the step counter sensor
    private int stepsSinceLastCheck;    // Steps not yet "consumed" by the game

    // PlayerPrefs keys
    private const string PREF_LAST_RAW = "StepCounter_LastRaw";
    private const string PREF_TOTAL_STEPS = "StepCounter_TotalSteps";

    private enum SensorKind
    {
        None,
        StepCounter,   // TYPE_STEP_COUNTER (19) – supports AFK
        StepDetector   // TYPE_STEP_DETECTOR (18) – only while app is running
    }

    private SensorKind sensorKind = SensorKind.None;

#if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject sensorManager;
    private AndroidJavaObject stepSensor;
    private AndroidJavaObject activity;
    private StepSensorListener sensorListener;

    // Internal flag to know if we already applied offline delta from prefs
    private bool initializedFromPrefs = false;
#endif

    private void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        LogDebug("Awake: initializing StepCounter...");

        // Request permission for physical activity (Android 10+)
        try
        {
            var permission = "android.permission.ACTIVITY_RECOGNITION";
            if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(permission))
            {
                UnityEngine.Android.Permission.RequestUserPermission(permission);
                LogDebug("Requested ACTIVITY_RECOGNITION permission.");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("StepCounter: permission request failed: " + e.Message);
            LogDebug("Permission request failed: " + e.Message);
        }

        // Load total steps from previous sessions
        totalSteps = PlayerPrefs.GetInt(PREF_TOTAL_STEPS, 0);

        // Get Unity activity
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        // Get SensorManager
        sensorManager = activity.Call<AndroidJavaObject>("getSystemService", "sensor");

        if (sensorManager == null)
        {
            LogDebug("SensorManager is null! Device may not support sensors.");
            return;
        }

        // Try STEP_COUNTER first (TYPE_STEP_COUNTER == 19)
        const int TYPE_STEP_COUNTER = 19;
        const int TYPE_STEP_DETECTOR = 18;

        stepSensor = sensorManager.Call<AndroidJavaObject>("getDefaultSensor", TYPE_STEP_COUNTER);
        if (stepSensor != null)
        {
            sensorKind = SensorKind.StepCounter;
            LogDebug("Using sensor: STEP_COUNTER (type 19). AFK possible.");
        }
        else
        {
            // Fallback: try STEP_DETECTOR
            stepSensor = sensorManager.Call<AndroidJavaObject>("getDefaultSensor", TYPE_STEP_DETECTOR);
            if (stepSensor != null)
            {
                sensorKind = SensorKind.StepDetector;
                LogDebug("Using sensor: STEP_DETECTOR (type 18). AFK NOT possible, only while app running.");
                // For step detector we don't use offline prefs logic
                initializedFromPrefs = true;
                IsInitialized = true;
            }
            else
            {
                sensorKind = SensorKind.None;
                LogDebug("No STEP_COUNTER (19) or STEP_DETECTOR (18) sensor found on this device.");
                Debug.LogWarning("StepCounter: No step sensor found on this device.");
                return;
            }
        }

        // Create listener proxy and register
        sensorListener = new StepSensorListener(OnStepSensorChanged);
        int SENSOR_DELAY_NORMAL = 3;
        sensorManager.Call<bool>("registerListener", sensorListener, stepSensor, SENSOR_DELAY_NORMAL);
        LogDebug("Listener registered.");
#else
        Debug.Log("StepCounter: running in Editor or non-Android, sensor disabled.");
        LogDebug("Not running on Android device.");
#endif
    }

    private void OnDestroy()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (sensorManager != null && sensorListener != null && stepSensor != null)
        {
            sensorManager.Call("unregisterListener", sensorListener, stepSensor);
            LogDebug("Listener unregistered.");
        }
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    // Called from Java side (SensorEventListener.onSensorChanged)
    private void OnStepSensorChanged(int rawValue)
    {
        // If for some reason we got callback but no sensor kind – ignore
        if (sensorKind == SensorKind.None)
        {
            LogDebug("OnStepSensorChanged called with SensorKind.None");
            return;
        }

        // ===== CASE 1: STEP_DETECTOR (18) – each event is "rawValue" steps (usually 1) =====
        if (sensorKind == SensorKind.StepDetector)
        {
            int delta = Mathf.Max(rawValue, 1);  // usually 1 per event
            totalSteps += delta;
            stepsSinceLastCheck += delta;

            SaveState(lastRawStepValue, totalSteps);

            LogDebug($"[DETECTOR] raw={rawValue}, delta={delta}, total={totalSteps}");
            return;
        }

        // ===== CASE 2: STEP_COUNTER (19) – rawValue is total steps since reboot =====

        // First time we get a reading in this app session
        if (!initializedFromPrefs)
        {
            // Load last saved raw value (if none, default to current rawValue)
            int savedRaw = PlayerPrefs.GetInt(PREF_LAST_RAW, rawValue);

            int offlineDelta = rawValue - savedRaw;
            if (offlineDelta > 0)
            {
                // Add steps that happened while the app was closed
                totalSteps += offlineDelta;
                stepsSinceLastCheck += offlineDelta;
            }

            lastRawStepValue = rawValue;
            SaveState(rawValue, totalSteps);

            initializedFromPrefs = true;
            IsInitialized = true;

            LogDebug($"[COUNTER-FIRST] rawNow={rawValue}, rawSaved={savedRaw}, offlineDelta={offlineDelta}, total={totalSteps}");
            return;
        }

        // Normal updates while the app is running
        int d = rawValue - lastRawStepValue;
        if (d > 0)
        {
            totalSteps += d;
            stepsSinceLastCheck += d;
        }

        lastRawStepValue = rawValue;
        SaveState(rawValue, totalSteps);

        LogDebug($"[COUNTER] raw={rawValue}, delta={d}, total={totalSteps}");
    }
#endif

    // Saves raw sensor value and total steps to PlayerPrefs
    private void SaveState(int rawValue, int total)
    {
        PlayerPrefs.SetInt(PREF_LAST_RAW, rawValue);
        PlayerPrefs.SetInt(PREF_TOTAL_STEPS, total);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Call this when you want to use the steps (for research, gold, etc.).
    /// It returns the steps accumulated since last call and resets that counter to 0.
    /// </summary>
    public int ConsumeSteps()
    {
        int result = stepsSinceLastCheck;
        stepsSinceLastCheck = 0;
        LogDebug($"ConsumeSteps() -> {result}");
        return result;
    }

    private void OnApplicationPause(bool pause)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (pause && lastRawStepValue >= 0)
        {
            SaveState(lastRawStepValue, totalSteps);
            LogDebug($"OnApplicationPause: saved raw={lastRawStepValue}, total={totalSteps}");
        }
#endif
    }

    private void OnApplicationQuit()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (lastRawStepValue >= 0)
        {
            SaveState(lastRawStepValue, totalSteps);
            LogDebug($"OnApplicationQuit: saved raw={lastRawStepValue}, total={totalSteps}");
        }
#endif
    }

    // Debug helper
    private void LogDebug(string msg)
    {
        Debug.Log("[StepCounter] " + msg);
        if (debugText != null)
        {
            debugText.text = msg + "\n" +
                             $"Total: {totalSteps}, SinceLast: {stepsSinceLastCheck}, Kind: {sensorKind}, Init: {IsInitialized}";
        }
    }
}

/// <summary>
/// Proxy that implements android.hardware.SensorEventListener in C#.
/// Connected to StepCounter via a callback.
/// </summary>
#if UNITY_ANDROID && !UNITY_EDITOR
public class StepSensorListener : AndroidJavaProxy
{
    private readonly Action<int> onStepChanged;

    public StepSensorListener(Action<int> onStepChanged)
        : base("android.hardware.SensorEventListener")
    {
        this.onStepChanged = onStepChanged;
    }

    // Java signature: void onSensorChanged(SensorEvent event)
    void onSensorChanged(AndroidJavaObject sensorEvent)
    {
        // SensorEvent.values[0] = step count as float (for both STEP_COUNTER and STEP_DETECTOR)
        float[] values = sensorEvent.Get<float[]>("values");
        if (values != null && values.Length > 0)
        {
            int rawSteps = Mathf.RoundToInt(values[0]);
            onStepChanged?.Invoke(rawSteps);
        }
    }

    // Java signature: void onAccuracyChanged(Sensor sensor, int accuracy)
    void onAccuracyChanged(AndroidJavaObject sensor, int accuracy)
    {
        // Not used, but required by the interface
    }
}
#endif
