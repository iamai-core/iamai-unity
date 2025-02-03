using System.Collections;
using UnityEngine;
using iamai_core_lib;

public class DummyAI : MonoBehaviour
{
    public ChatController chatController;

    public void SimulateAIResponse(string userMessage)
    {
        // Replace this with real AI logic or API call
        //string aiResponse = $"You said: {userMessage}";
        iamai_core_lib.AI ai = new iamai_core_lib.AI("llama-3.2-1b-instruct-q4_k_m.gguf");

        // Add the AI's response after a delay
        StartCoroutine(DelayedAIResponse(ai.Generate(userMessage)));
        //chatController.AddMessage(aiResponse, false);
    }

    private IEnumerator DelayedAIResponse(string aiResponse)
    {
        yield return new WaitForSeconds(0.0f);
        chatController.AddMessage(aiResponse, false);
    }
}