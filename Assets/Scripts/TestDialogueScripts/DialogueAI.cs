using NUnit.Framework;
using UnityEngine;
using iamai_core_lib;
using System.Collections.Generic;

public class DialogueAI : MonoBehaviour
{
    public GameObject messagePrefab;
    public Collider2D dialogueZone;
    public BoxCollider2D player;

    [Tooltip("Enter LLM file name in to section with file type. ex.Llama3B.gguf")]
    public List<string> ModelList;

    private iamai_core_lib.AI ai;

    private void Start()
    {
        ai = new iamai_core_lib.AI(ModelList[0]);
        ai.SetMaxTokens(256);

        GameObject message = Instantiate(messagePrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        message.GetComponent<InteractivePopup>().text.SetText("Come up and press 'E' to talk to me!");
        Destroy(message, 2.0f);
    }

    private void Update()
    {
        if (dialogueZone.IsTouching(player) && Input.GetKeyDown(KeyCode.E))
        {
            GameObject message = Instantiate(messagePrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            message.GetComponent<InteractivePopup>().text.SetText(ai.Generate(
                "You are a video game NPC, and a player is initiating dialogue with you. Respond with no more than 7-12 words."
                ));
            Destroy(message, 1.5f);
        }
    }
}
