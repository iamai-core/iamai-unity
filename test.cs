public class TestIamaiCore : MonoBehaviour
{
    private IamaiCore iamai;

    void Start()
    {
        iamai = FindObjectOfType<IamaiCore>();
        iamai.SendMessage("Test message");
    }
}
