using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatController : MonoBehaviour
{
    public Transform chatContent;
    public GameObject userMessagePrefab;
    public GameObject aiMessagePrefab;

    public void AddMessage(string message, bool isUser = false)
    {
        GameObject messageObject = Instantiate(isUser ? userMessagePrefab : aiMessagePrefab, chatContent);

        TMP_Text messageText = messageObject.GetComponentInChildren<TMP_Text>();
        messageText.text = message;

        Canvas.ForceUpdateCanvases();
        chatContent.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0.0f;
    }
}
