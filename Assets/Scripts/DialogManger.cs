using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.InputSystem;

public class DialogManager : MonoBehaviour
{
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;
    public GameObject rightClickPrompt;
    public float clickBufferTime = 0.5f;
    public float pulseSpeed = 5f;
    public float pulseAmplitude = 0.05f;

    private Action onDialogFinished;
    private float timeDialogOpened;
    private Coroutine promptAnimationCoroutine;

    void Awake()
    {
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
    }

    public void ShowDialog(string text, Action onFinishedCallback)
    {
        if (dialogPanel == null) return;

        dialogPanel.SetActive(true);
        if (rightClickPrompt != null) rightClickPrompt.SetActive(true);

        dialogText.text = text;
        onDialogFinished = onFinishedCallback;
        timeDialogOpened = Time.time;

        if (promptAnimationCoroutine != null) StopCoroutine(promptAnimationCoroutine);
        promptAnimationCoroutine = StartCoroutine(AnimatePrompt());
    }

    void Update()
    {
        if (dialogPanel == null || !dialogPanel.activeInHierarchy) return;

        if (Mouse.current.rightButton.wasPressedThisFrame && Time.time > timeDialogOpened + clickBufferTime)
        {
            if (promptAnimationCoroutine != null) StopCoroutine(promptAnimationCoroutine);
            dialogPanel.SetActive(false);
            if (rightClickPrompt != null) rightClickPrompt.SetActive(false);

            onDialogFinished?.Invoke();
        }
    }

    private IEnumerator AnimatePrompt()
    {
        if (rightClickPrompt == null) yield break;
        Vector3 baseScale = rightClickPrompt.transform.localScale;
        while (true)
        {
            float scaleOffset = 1.0f + (Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude);
            rightClickPrompt.transform.localScale = baseScale * scaleOffset;
            yield return null;
        }
    }
}