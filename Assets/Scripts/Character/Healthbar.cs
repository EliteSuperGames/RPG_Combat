using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField]
    public Slider slider;

    public void SetSize(float sizeNormalized)
    {
        slider.value = sizeNormalized;
    }
}
