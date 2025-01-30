using System.Collections;
using UnityEngine;
using IAMAICore.Unity;

public class DummyAI : MonoBehaviour
{
    public ChatController chatController;

    public void SimulateAIResponse(string userMessage)
    {
        // Replace this with real AI logic or API call
        //string aiResponse = $"You said: {userMessage}";
        IAMAICoreUnity iAMAICoreUnity = new IAMAICoreUnity();

        // Add the AI's response after a delay
        StartCoroutine(DelayedAIResponse("You said: " + iAMAICoreUnity.Generate(userMessage)));
        //chatController.AddMessage(aiResponse, false);
    }

    private IEnumerator DelayedAIResponse(string aiResponse)
    {
        yield return new WaitForSeconds(1.0f);
        chatController.AddMessage(aiResponse, false);
    }
}
