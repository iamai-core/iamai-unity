using System.CodeDom.Compiler;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveAI : MonoBehaviour
{
    public InteractiveTreasure treasure;
    public GameObject messagePrefab;
    private bool alreadySpotted = false;

    [Tooltip("Enter LLM file name in to section with file type. ex.Llama3B.gguf")]
    public List<string> ModelList;

    private iamai_core_lib.AI ai;

    private void Start()
    {
        ai = new iamai_core_lib.AI(ModelList[0]);
        ai.SetMaxTokens(256);
    }

    private void Update()
    {
        if (!treasure && !alreadySpotted)
        {
            alreadySpotted = true;
            GameObject message = Instantiate(messagePrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            message.GetComponent<InteractivePopup>().text.SetText(ai.Generate(
                "You are a NPC in a video game, and you just saw the player steal your treasure. Respond angrily with as few words as possible."
                ));
            Destroy(message, 1.5f);
        }
    }
}
