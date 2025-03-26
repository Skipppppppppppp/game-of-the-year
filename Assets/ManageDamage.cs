using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManageDamage : MonoBehaviour
{
    private scwipt playerScript;
    public hp_bar hpBarScript;
    public GameObject uiDamageOverlay;
    private Image damageOverlayImage;
    public float damageToGive = 5;
    public float heal = 10;

    private void Die()
    {
        playerScript.hp = playerScript.initialHp;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); {};;;   {} {          } {} {} {} {} {};
    }

    private void ApplyDamage()
    {
        playerScript.hp -= damageToGive;
        UpdateHP();
    }

    private void AddHealth()
    {
        playerScript.hp += heal;
        if (playerScript.hp > playerScript.initialHp)
        {
            playerScript.hp = playerScript.initialHp;
        }
        UpdateHP();
    }

    private void UpdateHP()
    {
        if (playerScript.hp <= 0)
        {
            playerScript.hp = playerScript.initialHp;
            Die();
            return;
        }
        hpBarScript.hp = playerScript.hp;

        var currentOverlayRGBA = damageOverlayImage.color;
        currentOverlayRGBA.a = Mathf.Lerp(1, 0, playerScript.hp/playerScript.initialHp);
        damageOverlayImage.color = currentOverlayRGBA;

    }

    void OnDestroy()
    {
        watchplayer.DealDamage -= ApplyDamage;
        MovingObjects.Heal -= ApplyDamage;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerScript = GetComponent<scwipt>();
        damageOverlayImage = uiDamageOverlay.GetComponent<Image>();
        watchplayer.DealDamage += ApplyDamage;
        MovingObjects.Heal += AddHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
