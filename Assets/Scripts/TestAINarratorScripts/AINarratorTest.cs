using System.Collections.Generic;
using UnityEngine;

public class AINarratorTest : MonoBehaviour
{
    public GameObject Treasure;
    private bool stolen = false;

    [Tooltip("Enter LLM file name in to section with file type. ex.Llama3B.gguf")]
    public List<string> ModelList;

    private iamai_core_lib.AI ai;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ai = new iamai_core_lib.AI(ModelList[0]);
        ai.SetMaxTokens(256);
        print(ai.Generate("You are the narrator and the game just began. The player is a thief. To the right of the player is a treasure chest, and to their left is the owner of the chest, whose guard is down. Narrate this."));      
    }

    // Update is called once per frame
    void Update() 
    {
        if (!Treasure && !stolen)
        {
            stolen = true;
            print(ai.Generate("The player stole the treasure and the owner saw it! Narrate this!"));
        }
    }
}
