using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public KinematicPlayerController player;
    private Slider healthSlider;

    private void Start()
    {
        healthSlider = GetComponent<Slider>();

        if (player != null)
        {
            healthSlider.maxValue = player.maxHealth;
            healthSlider.value = player.maxHealth;
        }
    }

    private void Update()
    {
        if (player != null)
        {
            healthSlider.value = player.currentHealth;
        }
    }
}
