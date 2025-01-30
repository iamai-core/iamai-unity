using NUnit.Framework;
using UnityEngine;

public class DialogueAI : MonoBehaviour
{
    public GameObject messagePrefab;
    public Collider2D dialogueZone;
    public BoxCollider2D player;

    private void Start()
    {
        GameObject message = Instantiate(messagePrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        message.GetComponent<InteractivePopup>().text.SetText("Come up and press 'E' to talk to me!");
        Destroy(message, 2.0f);
    }

    private void Update()
    {
        if (dialogueZone.IsTouching(player) && Input.GetKeyDown(KeyCode.E))
        {
            GameObject message = Instantiate(messagePrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            message.GetComponent<InteractivePopup>().text.SetText(Generate("The player is talking to you."));
            Destroy(message, 1.5f);
        }
    }
    public string Generate(string prompt)
    {
        return "Hey, what's up?";
    }
}
