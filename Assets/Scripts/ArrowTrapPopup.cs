using UnityEngine;
using System.Collections;

public class ArrowTrapPopup : MonoBehaviour
{
    public GameObject popupUI;
    public float displayTime = 3f; // how long to show the popup

    private Coroutine hideCoroutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (popupUI != null)
            {
                popupUI.SetActive(true);

                // If there's a previous timer, stop it
                if (hideCoroutine != null)
                    StopCoroutine(hideCoroutine);

                // Start a new timer to hide it after a delay
                hideCoroutine = StartCoroutine(HideAfterDelay());
            }
        }
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);
        popupUI.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // optional: remove this if you always want it to stay for the full duration
    }
}
