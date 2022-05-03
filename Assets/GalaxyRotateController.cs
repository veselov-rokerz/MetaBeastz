using UnityEngine;

public class GalaxyRotateController : MonoBehaviour
{
    private float initialValue;

    // Start is called before the first frame update
    void Start()
    {
        initialValue = RenderSettings.skybox.GetFloat("_Rotation");
    }

    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time);
    }

    private void OnApplicationQuit()
    {
        RenderSettings.skybox.SetFloat("_Rotation", initialValue);
    }
}
