using System.Collections;
using UnityEngine;
using iamai_core_lib;
using System.Collections.Generic;
using System;
using UnityEditor.Search;
using UnityEngine.Rendering;

public class DummyAI : MonoBehaviour
{
    public ChatController chatController;
    [Tooltip("Enter LLM file name in to section with file type. ex.Llama3B.gguf")]
    public List<string> ModelList;

    public void SimulateAIResponse(string userMessage)
    {
        // Replace this with real AI logic or API call
        //string aiResponse = $"You said: {userMessage}";

        string prompt = "You are a helpful AI assistant that will send back one Response.\n\n Message: " + userMessage + "\nResponse: ";

        iamai_core_lib.AI ai = new iamai_core_lib.AI(ModelList[0]);

        // Add the AI's response after a delay
        StartCoroutine(DelayedAIResponse(ai.Generate(prompt)));
        //chatController.AddMessage(aiResponse, false);
    }

    private IEnumerator DelayedAIResponse(string aiResponse)
    {
        yield return new WaitForSeconds(0.0f);
        chatController.AddMessage(aiResponse, false);
    }
}