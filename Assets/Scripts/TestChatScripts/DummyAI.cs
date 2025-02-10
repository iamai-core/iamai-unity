using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class DummyAI : MonoBehaviour
{
    public ChatController chatController;

    [Tooltip("Enter LLM file name in to section with file type. ex.Llama3B.gguf")]
    public List<string> ModelList;

    [TextAreaAttribute] public string initialPrompt;
    private iamai_core_lib.AI ai;

    private void Start()
    {
        ai = new iamai_core_lib.AI(ModelList[0]);
        ai.SetMaxTokens(256);
        StartCoroutine(DelayedAIResponse(ai.Generate(
            "You are a helpful AI assistant that will send back one response.\n\n Message: " + initialPrompt + "\nResponse: "
            )));
    }

    public void SimulateAIResponse(string userMessage)
    {
        // Add the AI's response after a delay
        StartCoroutine(DelayedAIResponse(ai.Generate(
            "You are a helpful AI assistant that will send back one response.\n\n Message: " + userMessage + "\nResponse: "
            )));
    }

    private IEnumerator DelayedAIResponse(string aiResponse)
    {
        yield return new WaitForSeconds(0.0f);
        chatController.AddMessage(aiResponse, false);
    }
}