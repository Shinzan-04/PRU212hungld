using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void Exit()
    {
        Debug.Log("Exit Game"); 

        Application.Quit();
    }
}