using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Notifications : MonoBehaviour
{
    public static Notifications Instance;

    [SerializeField] private GameObject NotificationsUI;
    [SerializeField] private GameObject textPrefab;

    public float PaddingX = 0f;
    public float PaddingY = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    public void CreateNotification(string message, Color color)
    {
        GameObject newTextObject = Instantiate(textPrefab, NotificationsUI.transform);
        TextMeshProUGUI textComponent = newTextObject.GetComponent<TextMeshProUGUI>();

        textComponent.color = color;
        textComponent.text = message;

        StartCoroutine(AnimateNotification(newTextObject));
    }

    private IEnumerator AnimateNotification(GameObject textObj)
    {
        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(PaddingX, PaddingY);

        TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();
        
        // optional: ensure CanvasGroup exists for fading
        CanvasGroup canvasGroup = textObj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = textObj.AddComponent<CanvasGroup>();

        Vector3 startPos = rect.anchoredPosition;
        Vector3 endPos = startPos + new Vector3(0f, 50f, 0f); // move up 50 px

        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // move up smoothly
            rect.anchoredPosition = Vector3.Lerp(startPos, endPos, t);

            // fade out gradually
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        Destroy(textObj);
    }
}
