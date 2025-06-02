using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public class ManageDamage : MonoBehaviour
{
    private scwipt playerScript;
    public hp_bar hpBarScript;
    public Image damageOverlayImage;
    public float[] phaseHPs;
    private int phase;
    public GameObject[] phaseObjects;
    private float initialHp;
    public float[] speedCoeffsAir;
    public float[] speedCoeffsGround;
    public float[] jumpForceCoeffs;
    public MovingObjects movingObjectsScript;
    public RenderOnClick renderOnClickScript;
    public Volume volume;
    private FilmGrain filmGrain;
    public bool applyFilmGrain;

    public struct Phase
    {
        public GameObject phaseObject;
        public float speedCoeffAir;
        public float speedCoeffGround;
        public float jumpForceCoeff;
    }

    private void Die()
    {
        playerScript.hp = playerScript.initialHp;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); {};;;   {} {          } {} {} {} {} {}; {{}{}{}{}{{}{}{}{}{}{}{}{{{{};;;;;;{}}}}}}
    }

    public void ApplyDamage(float damageToGive)
    {
        playerScript.hp -= damageToGive;
        UpdateHP();
    }

    public void AddHealth(float heal)
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

        Color currentOverlayRGBA = damageOverlayImage.color;

        float overlayOpacity = Mathf.Lerp(1, 0, playerScript.hp/playerScript.initialHp);

        currentOverlayRGBA.a = overlayOpacity;
        damageOverlayImage.color = currentOverlayRGBA;

        if (applyFilmGrain)
            {filmGrain.intensity.value = overlayOpacity;}
    }

    void ChangePhase(int iteration)
    {
        if (iteration != 0 && renderOnClickScript.enabled == false || movingObjectsScript.enabled == false)
        {
            movingObjectsScript.enabled = true;
            renderOnClickScript.enabled = true;
        }

        if (iteration == 0)
        {
            movingObjectsScript.enabled = false;
            renderOnClickScript.enabled = false;
        }

        int currentIteration = 0;
        foreach (GameObject phaseObject in phaseObjects)
        {

            if (currentIteration == iteration)
            {
                phaseObject.SetActive(true);
                playerScript.playerCollider = phaseObject.GetComponent<BoxCollider2D>();
                playerScript.speedCoeffAir = speedCoeffsAir[iteration];
                playerScript.speedCoeffGround = speedCoeffsGround[iteration];
                playerScript.jumpForceCoeff = jumpForceCoeffs[iteration];
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
        initialHp = playerScript.initialHp;

        if (volume.profile.TryGet(out FilmGrain fg))
        {
            filmGrain = fg;
        }

        UpdateHP();
    }

    // Update is called once per frame
    void Update()
    {
        var hp = playerScript.hp;
    }
}
