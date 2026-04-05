using System.Collections;
using UnityEngine;

public class SequenceTransition : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject landscapeCanvas;
    public GameObject portraitCanvas;
    
    [Header("Transition Screen")]
    public CanvasGroup fadePanelGroup; 

    [Header("Animation")]
    public Animator clorbAnimator; 
    public string triggerName = "PlayDeath"; // Set to your exact trigger name

    [Header("Timings")]
    public float delayBeforeFade = 0.5f; // How long to wait AFTER animation starts before fading
    public float fadeDuration = 1.0f;    // How fast the fade-in and fade-out happen (1 second is usually smooth)
    public float textReadingTime = 3.0f; // How long the screen stays solid so they can read the text

    public void StartTransitionSequence()
    {
        fadePanelGroup.gameObject.SetActive(true);
        fadePanelGroup.blocksRaycasts = true; 
        
        StartCoroutine(SequenceRoutine());
    }

    private IEnumerator SequenceRoutine()
    {
        // 1. PLAY THE ANIMATION
        if (clorbAnimator != null)
        {
            clorbAnimator.SetTrigger(triggerName);
        }

        // 2. DELAY FADE (Lets the animation play out for a moment)
        yield return new WaitForSeconds(delayBeforeFade);

        // 3. FADE IN THE TRANSITION PANEL (Screen goes solid color)
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadePanelGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
        fadePanelGroup.alpha = 1f;

        // 4. WAIT FOR THE PLAYER TO READ THE TEXT
        // The screen is now 100% solid, we pause here for a few seconds
        yield return new WaitForSeconds(textReadingTime); 

        // 5. THE SECRET SWAP (Happens behind the solid color panel)
        Screen.orientation = ScreenOrientation.Portrait;
        landscapeCanvas.SetActive(false);
        portraitCanvas.SetActive(true);

        // 6. FADE THE TRANSITION PANEL OUT (Reveal the new Portrait Canvas)
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadePanelGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }
        
        fadePanelGroup.alpha = 0f;
        fadePanelGroup.gameObject.SetActive(false);
        fadePanelGroup.blocksRaycasts = false;
    }
}