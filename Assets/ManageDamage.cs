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
    public float[] phaseHPs;
    private int phase;
    public GameObject[] phaseObjects;
    private float initialHp;
    public float[] speedCoeffsAir;
    public float[] speedCoeffsGround;

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
        float hp = playerScript.hp;

        if (hp <= 0)
        {
            hp = initialHp;
            Die();
            return;
        }

        int iteration = 0;

        foreach(float phaseHP in phaseHPs)
        {
            if (hp > phaseHP)
            {
                iteration++;
                continue;
            }
                ChangePhase(iteration);
                iteration++;
                break;
        }

        hpBarScript.hp = hp;

        var currentOverlayRGBA = damageOverlayImage.color;
        currentOverlayRGBA.a = Mathf.Lerp(1, 0, hp/initialHp);
        damageOverlayImage.color = currentOverlayRGBA;

    }

    void OnDestroy()
    {
        watchplayer.DealDamage -= ApplyDamage;
        MovingObjects.Heal -= AddHealth;
    }

    void ChangePhase(int iteration)
    {
        int currentIteration = 0;
        foreach (GameObject phaseObject in phaseObjects)
        {

            if (currentIteration == iteration)
            {
                phaseObject.SetActive(true);
                playerScript.speedCoeffAir = speedCoeffsAir[iteration];
                playerScript.speedCoeffGround = speedCoeffsGround[iteration];
                currentIteration++;
                continue;
            }
            phaseObject.SetActive(false);
            currentIteration++;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerScript = GetComponent<scwipt>();
        damageOverlayImage = uiDamageOverlay.GetComponent<Image>();
        watchplayer.DealDamage += ApplyDamage;
        MovingObjects.Heal += AddHealth;
        initialHp = playerScript.initialHp;
    }

    // Update is called once per frame
    void Update()
    {
        var hp = playerScript.hp;
    }
}
