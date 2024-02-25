using UnityEngine;

public class PulsatingEffect : MonoBehaviour
{
    public float pulseSpeed = 1.5f; // Adjust the speed of the pulsating effect
    public float maxScale = 1.2f; // Adjust the maximum scale of the pulsating effect

    private void Update()
    {
        // Calculate the pulsating scale using a sine function
        float pulsatingScale = 1f + Mathf.Sin(Time.time * pulseSpeed) * 0.5f * (maxScale - 1f);

        // Apply the scale to the sprite
        transform.localScale = new Vector3(pulsatingScale, pulsatingScale, 1f);
    }
}
