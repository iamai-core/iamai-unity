using System.Collections;
using UnityEngine;
using iamai_core_lib;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering;
//using UnityEditor.Search;

public class DummyAI : MonoBehaviour
{
    public ChatController chatController;
    [Tooltip("Enter LLM file name in to section with file type. ex.Llama3B.gguf")]
    public List<string> ModelList;
    [TextAreaAttribute] public string initialPrompt;
    private iamai_core_lib.AI ai;

    private async void Start()
    {
        ai = new iamai_core_lib.AI(ModelList[0], 4096, 512, 512, 8);
        await ai.Activate();
        DelayedAIResponse(await ai.GenerateAsync(
             initialPrompt
            ));
    }

    public async void SimulateAIResponse(string userMessage)
    {
		// Add the AI's response after a delay
		DelayedAIResponse(await ai.GenerateAsync(
            userMessage
            ));
    }

    private void DelayedAIResponse(string aiResponse)
    {
        chatController.AddMessage(aiResponse, false);
    }
}