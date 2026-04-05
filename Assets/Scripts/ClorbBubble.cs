using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems; // Required for UI hovering!
using TMPro;

// Adding IPointerEnterHandler and IPointerExitHandler lets this script act as a button
public class ClorbBubble : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public GameObject speechBubble; // Drag the SpeechBubble object here
    public TextMeshProUGUI bubbleText; // Drag the ThoughtText object here

    [Header("Hover Settings")]
    public string hoverText = "Pet me!";

    [Header("Random Thoughts")]
    public string[] randomThoughts; // A list of things the Clorb can say
    public float minWaitTime = 5f;
    public float maxWaitTime = 15f;
    public float thoughtDisplayTime = 3f; // How long the random text stays on screen

    private bool isHovering = false;

    void Start()
    {
        // 1. Hide the bubble as soon as the game starts
        HideBubble();
        
        // 2. Start the invisible random timer
        StartCoroutine(RandomThoughtRoutine());
    }

    // --- HOVER LOGIC ---
    // Triggers the moment the mouse enters the Clorb's image boundary
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        StopAllCoroutines(); // Pause the random timer so it doesn't interrupt us
        ShowBubble(hoverText);
    }

    // Triggers the moment the mouse leaves the boundary
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        HideBubble();
        StartCoroutine(RandomThoughtRoutine()); // Restart the random timer
    }

    // --- RANDOM THOUGHT LOGIC ---
    private IEnumerator RandomThoughtRoutine()
    {
        while (true) // This loops forever in the background
        {
            // Wait for a random amount of seconds
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // Double check we aren't currently hovering over it
            if (!isHovering && randomThoughts.Length > 0)
            {
                // Pick a random line of text from our list
                string randomText = randomThoughts[Random.Range(0, randomThoughts.Length)];
                ShowBubble(randomText);

                // Wait for a few seconds so the player can read it
                yield return new WaitForSeconds(thoughtDisplayTime);

                // Hide it again, and the while(true) loop will automatically restart!
                if (!isHovering) 
                {
                    HideBubble();
                }
            }
        }
    }

    // --- HELPER FUNCTIONS ---
    private void ShowBubble(string textToShow)
    {
        bubbleText.text = textToShow;
        speechBubble.SetActive(true);
    }

    private void HideBubble()
    {
        speechBubble.SetActive(false);
    }

    public void StopTalking()
    {
        // 1. Kill the random thought timer
        StopAllCoroutines(); 
        
        // 2. Force the bubble to disappear so it isn't stuck on screen
        HideBubble(); 
    }
}