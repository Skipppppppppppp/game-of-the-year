using System;
using System.Threading.Tasks;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DarkenScreen : MonoBehaviour
{
    public Image overlay;
    private float opacity;
    private float targetOpacity;
    // public float step = 0.01f;
    public float epsilon = 0.009f;
    private float time;
    float opacityPerTime;
    TaskCompletionSource<int> taskCompletionSource;

    public Task<int> StartChangingOpacity(float targetValue, float timeLimit)
    {
        targetOpacity = targetValue;
        time = timeLimit;
        opacityPerTime = (targetOpacity - opacity) / time;
        taskCompletionSource = new();
        return taskCompletionSource.Task;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        opacity = overlay.color.a;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (time <= 0)
        {
            if (taskCompletionSource != null)
            {
                var t = taskCompletionSource;
                taskCompletionSource = null;

                t.SetResult(1);
            }
            return;
        }
        
        float dTime = Time.deltaTime;

        time -= dTime;
        float step = opacityPerTime * dTime;

        opacity += step;
        Color c = overlay.color;
        overlay.color = new Color(c.r, c.g, c.b, opacity);
    }

}
