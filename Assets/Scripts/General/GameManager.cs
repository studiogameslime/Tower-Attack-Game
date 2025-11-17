using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private StepCounter stepCounter;
    [SerializeField] private TMP_Text stepsCounterText;

    private void Update()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (stepCounter == null || stepsCounterText == null) return;

        stepsCounterText.text =
            $"Total: {stepCounter.TotalStepsSinceStart}\n" +
            $"SinceLastCheck: {stepCounter.StepsSinceLastCheck}\n" +
            $"Initialized: {stepCounter.IsInitialized}";
#else
        if (stepsCounterText != null)
            stepsCounterText.text = "Not running on Android.";
#endif
    }
}
