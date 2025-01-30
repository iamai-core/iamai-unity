using System.CodeDom.Compiler;
using UnityEngine;

public class InteractiveAI : MonoBehaviour
{
    public InteractiveTreasure treasure;
    public GameObject messagePrefab;
    private bool alreadySpotted = false;

    private void Update()
    {
        if (!treasure && !alreadySpotted)
        {
            alreadySpotted = true;
            GameObject message = Instantiate(messagePrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            message.GetComponent<InteractivePopup>().text.SetText(Generate("Your treasure was stolen."));
            Destroy(message, 1.5f);
        }
    }
    public string Generate(string prompt)
    {
        Debug.Log("The AI saw you!");
        return "Hey, I saw that!";
    }
}
