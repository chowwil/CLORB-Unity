using UnityEngine;

public class CanvasOrientation : MonoBehaviour
{
    // This runs the moment this Canvas is turned ON (SetActive(true))
    void OnEnable()
    {
        // Force the screen to Landscape
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    // This runs the moment this Canvas is turned OFF (SetActive(false))
    void OnDisable()
    {
        // Force the screen back to Portrait for the rest of the game
        Screen.orientation = ScreenOrientation.Portrait;
    }
}