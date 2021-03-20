using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    //Refs
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    //vars
    [SerializeField, Range(0, 600)] private float TimeOfDay;
    
    private void Update()
    {
        if (Preset == null)
            return;
        if(Application.isPlaying)
        {
            TimeOfDay += Time.deltaTime;
            TimeOfDay %= 600;
            UpdateLighting(TimeOfDay/600);
        } else
        {
            UpdateLighting(TimeOfDay / 600);
        }
    }
    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        if(DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360) - 90f, -170, 0));
        }
    }
    private void OnValidate()
    {
        if (DirectionalLight != null)
            return;
        if(RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        } else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach(Light light in lights)
            {
                if(light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }
}
