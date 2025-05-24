using TMPro;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public TextMeshProUGUI text;
    private bool isPaused;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }

        if (isPaused == false)
        {
            isPaused = true;
            text.text = "Paused, press ESC to unpause";
            Time.timeScale = 0;
            return;
        }

        isPaused = false;
        text.text = "";
        Time.timeScale = 1;
    }
}
