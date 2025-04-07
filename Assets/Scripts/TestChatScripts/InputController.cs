using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button submitButton;
    public DummyAI AI;
    ChatController chatController;

    void Awake()
    {
        chatController = GetComponent<ChatController>();
        inputField.onEndEdit.AddListener(AcceptStringInput);
    }

    void AcceptStringInput(string userInput)
    {
        chatController.AddMessage(userInput, true);
        InputComplete(userInput);
    }

    void InputComplete(string userInput)
    {
        AI.SimulateAIResponse(userInput);
        inputField.ActivateInputField();
        inputField.text = null;
    }
}
