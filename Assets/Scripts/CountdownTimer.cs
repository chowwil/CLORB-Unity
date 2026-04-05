using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [Header("UI Box References")]
    // The Left Box
    public TextMeshProUGUI leftValueText;
    public TextMeshProUGUI leftLabelText;
    
    // The Right Box
    public TextMeshProUGUI rightValueText;
    public TextMeshProUGUI rightLabelText;

    [Header("Canvas Transitions")]
    public GameObject countdownCanvas;
    public GameObject congratsCanvas;

    private float timeRemainingInSeconds;
    private bool isTimerRunning = false;

    // Called by your Setup screen
    public void BeginCountdown(int hours, int minutes)
    {
        timeRemainingInSeconds = (hours * 3600f) + (minutes * 60f);
        isTimerRunning = true;
        UpdateDisplay();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timeRemainingInSeconds -= Time.deltaTime;

            if (timeRemainingInSeconds <= 0)
            {
                timeRemainingInSeconds = 0;
                isTimerRunning = false;
                FinishTimer();
            }

            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        // Calculate raw hours, minutes, and seconds
        int h = Mathf.FloorToInt(timeRemainingInSeconds / 3600f);
        int m = Mathf.FloorToInt((timeRemainingInSeconds % 3600f) / 60f);
        int s = Mathf.FloorToInt(timeRemainingInSeconds % 60f);

        Debug.Log("Time Remaining: " + h + ":" + m + ":" + s);

        // Dynamic Display Logic
        if (h > 0)
        {
            // If there is an hour or more left, show HH:MM
            leftValueText.text = h.ToString("00");
            leftLabelText.text = "HOURS";

            rightValueText.text = m.ToString("00");
            rightLabelText.text = "MINUTES";
        }
        else
        {
            // If it drops under an hour, switch to showing MM:SS
            leftValueText.text = m.ToString("00");
            leftLabelText.text = "MINUTES";

            rightValueText.text = s.ToString("00");
            rightLabelText.text = "SECONDS";
        }
    }

    private void FinishTimer()
    {
        countdownCanvas.SetActive(false);
        congratsCanvas.SetActive(true);
    }
}