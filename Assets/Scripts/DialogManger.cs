using UnityEngine;
using TMPro;
using System;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;

    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;

    private Action onDialogFinished;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void ShowDialog(string text, Action onFinishedCallback)
    {
        dialogPanel.SetActive(true);
        dialogText.text = text;
        onDialogFinished = onFinishedCallback;
    }

    void Update()
    {
        if (dialogPanel.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            dialogPanel.SetActive(false);
            onDialogFinished?.Invoke();
        }
    }
}