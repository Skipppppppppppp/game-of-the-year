using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class DoorFinder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var root in rootObjects)
        {
            Transform[] allTransforms = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allTransforms)
            {
                if (t.gameObject.layer == (int) Layer.Doors)
                    Debug.Log(t);
            }
        }
    }
}