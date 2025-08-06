using System.Threading;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

public class explodeAfterTime : MonoBehaviour
{
    private new ParticleSystem particleSystem;
    public AudioClip sound;
    private GameObject image;
    public TimerUtility timer = new TimerUtility(1);
    public int damage = 30;
    public float radius = 5;
    private const LayerMask layerMask = LayerMask.Pwayer;
    private LayerMask obstacleLayerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        image = transform.GetChild(0).gameObject;
        obstacleLayerMask = LayerMask.Default | LayerMask.Doors;
    }

    // Update is called once per frame
    void Update()
    {
        if (!timer.Update())
        {
            return;
        }

        var pos = transform.position;
        Destroy(image);
        var m = particleSystem.main;
        m.stopAction = ParticleSystemStopAction.Callback;
        particleSystem.Play();

        AudioSource.PlayClipAtPoint(sound, pos);

        var thingsAround = Physics2D.OverlapCircleAll(pos, radius, (int)layerMask);
        foreach (var j in thingsAround)
        {
            Transform transParent = j.transform.parent;
            ManageDamage healthScript = transParent.GetComponent<ManageDamage>();
            if (healthScript == null)
            {
                continue;
            }

            Vector2 playerPos = transParent.position;
            var toObjVector = playerPos - pos.ToVector2();
            var wall = Physics2D.Raycast(pos, toObjVector.normalized, toObjVector.magnitude, (int)obstacleLayerMask);

            if (wall.collider == null)
            {
                float t = toObjVector.magnitude / radius;
                float damageToGive = damage - Mathf.Lerp(0, damage, t);
                healthScript.ApplyDamage(damageToGive);
            }

        }
        this.enabled = false;
    }

    void OnParticleSystemStopped()
    {
        Destroy(this.gameObject);
    }
}
