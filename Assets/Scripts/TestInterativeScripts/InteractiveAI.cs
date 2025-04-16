using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class InteractiveAI : MonoBehaviour
{
    public InteractiveTreasure treasure;
    public GameObject messagePrefab;
    private bool alreadySpotted = false;

    [Tooltip("Enter LLM file name in to section with file type. ex.Llama3B.gguf")]
    public List<string> ModelList;

    private iamai_core_lib.AI ai;

    private async void Start()
    {
        ai = new iamai_core_lib.AI(ModelList[0]);
        await ai.Activate();
    }

    private async void Update()
    {
        if (!treasure && !alreadySpotted)
        {
            alreadySpotted = true;
            GameObject message = Instantiate(messagePrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            message.GetComponent<InteractivePopup>().text.SetText(await ai.GenerateAsync(
                "You are a video game NPC, and a player stole your treasure. Respond angrily with no more than 7-12 words."
                ));
            Destroy(message, 1.5f);
        }
    }
}
