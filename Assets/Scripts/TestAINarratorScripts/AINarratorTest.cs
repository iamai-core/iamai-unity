using UnityEngine;

public class AINarratorTest : MonoBehaviour
{
    public GameObject Treasure;
    private bool stolen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { print(Generate("The game just began. To the right of the player is a treasure chest, and to their left is the owner of the chest, whose guard is down...")); }

    // Update is called once per frame
    void Update() 
    {
        if (!Treasure && !stolen)
        {
            stolen = true;
            print(Generate("The player stole the treasure!"));
        }
    }

    string Generate(string prompt) { return "SAMPLE TEXT sample text"; }
}
