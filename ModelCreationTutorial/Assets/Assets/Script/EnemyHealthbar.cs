using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarController : MonoBehaviour
{
    public EnemyBoss boss;
    private Slider healthSlider;

    private void Start()
    {
        healthSlider = GetComponent<Slider>();

        if (boss != null)
        {
            healthSlider.maxValue = boss.maxHealth;
            healthSlider.value = boss.currentHealth;
        }
    }

    private void Update()
    {
        if (boss != null)
        {
            healthSlider.value = boss.currentHealth;
        }
    }
}
