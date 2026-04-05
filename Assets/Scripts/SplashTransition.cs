using System.Collections;
using UnityEngine;

public class SplashTransition : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup splashCanvasGroup;
    public GameObject startCanvas;

    [Header("Timing Settings")]
    public float displayTime = 2.0f; // How long it stays fully visible
    public float fadeDuration = 1.5f; // How long the fade-out takes

    void Start()
    {
        // As soon as the game starts, ensure the Start Canvas is off
        startCanvas.SetActive(false);
        
        // Ensure the splash screen is fully visible
        splashCanvasGroup.alpha = 1f;

        // Begin the sequence
        StartCoroutine(SplashSequence());
    }

    IEnumerator SplashSequence()
    {
        // 1. Wait for the logo/splash art to be displayed
        yield return new WaitForSeconds(displayTime);

        // 2. Gradually fade out the Canvas Group
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            
            // Mathf.Lerp smoothly transitions from 1 (solid) to 0 (invisible)
            splashCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            
            yield return null; // Wait for the very next frame before looping again
        }

        // Force alpha to exactly 0 at the end just to be safe
        splashCanvasGroup.alpha = 0f;

        // 3. The fade is done! Turn off the Splash Canvas and turn on the Start Menu
        splashCanvasGroup.gameObject.SetActive(false);
        startCanvas.SetActive(true);
    }
}