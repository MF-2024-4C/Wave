using System;
using System.Collections.Generic;
using Michsky.UI.Heat;
using Quantum;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class MessageWindowManager : MonoBehaviour
{
    public static MessageWindowManager Instance;

    [SerializeField] private ModalWindowManager modalWindowManager;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private ButtonManager confirmButton;
    [SerializeField] private ButtonManager cancelButton;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        var prefab = Resources.Load<MessageWindowManager>("MessageWindow");
        Instance = Instantiate(prefab);
        DontDestroyOnLoad(Instance.gameObject);
    }

    public void ShowMessage(string message, string description = "", string confirmButtonText = "Confirm",
        UnityAction confirmEvent = null, string cancelButtonText = "", UnityAction cancelEvent = null)
    {
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        
        modalWindowManager.Show();
        messageText.text = message;
        descriptionPanel.SetActive(!string.IsNullOrEmpty(description));
        descriptionText.text = description;
        confirmButton.gameObject.SetActive(!string.IsNullOrEmpty(confirmButtonText));
        cancelButton.gameObject.SetActive(!string.IsNullOrEmpty(cancelButtonText));
        confirmButton.buttonText = confirmButtonText;
        cancelButton.buttonText = cancelButtonText;
        if (confirmEvent != null) confirmButton.onClick.AddListener(confirmEvent);;
        if (cancelEvent != null) cancelButton.onClick.AddListener(cancelEvent);
        
        confirmButton.onClick.AddListener(() => modalWindowManager.Hide());
        confirmButton.onClick.AddListener(() => modalWindowManager.Hide());
    }
}