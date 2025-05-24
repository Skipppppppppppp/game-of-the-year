using System;
using Mono.Cecil.Cil;
using UnityEngine;

#nullable enable

public sealed class MovingObjects : MonoBehaviour
{
    private int layerMask;
    public GameObject player;
    public float minForInterp;
    public float maxForInterp;
    public float minForDistance;
    public float maxForDistance;
    private const float maxForInterpCoeff = 1;
    public Rigidbody2D? movingObject;
    private RememberInitialProperties? rememberedInitialProperties;
    [Range(0,20)] public float linearDampingScale;
    public float distanceToEat = 1;
    private ManageDamage healthScript;
    private int edibleLayer;
    public float healthToGive = 15;
    private int obstacleLayer;
    public float maxDistance = 12;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        edibleLayer = 1 << LayerMask.NameToLayer("Peopwe");
        foreach (var x in new[]
        {
            "Peopwe",
            "Moveable Stuff",
        })
        {
            var layer = LayerMask.NameToLayer(x);
            layerMask |= 1 << layer;
        }
        obstacleLayer = 1 << LayerMask.NameToLayer("Default");
        healthScript = player.GetComponent<ManageDamage>();
    }



    // Update is called once per frame
    void Update()
    {
        if (!Input.GetMouseButton(1))
        {
            if (movingObject == null)
            {
                return;
            }
            var r = rememberedInitialProperties!;
            var b = movingObject.GetComponent<IObjectSelectedHandler>();
            if (b != null)
            {
                b.Deselected();
            }

            movingObject.gravityScale = r.GravityScale;
            movingObject.linearDamping = r.LinearDamping;
            Destroy(r);

            movingObject = null;
            return;
        }

        if (movingObject == null)
        {
            var raycasted = RaycastHelper.TrySelectObject(
                playerPos: player.transform.position,
                maxDistance: maxDistance,
                layerMask: layerMask,
                obstacleLayerMask: obstacleLayer);
            if (raycasted == null)
            {
                return;
            }
            if (raycasted.gameObject.GetComponent<RememberInitialProperties>() != null)
            {
                return;
            }
            movingObject = raycasted;
            var r = movingObject.gameObject.AddComponent<RememberInitialProperties>();
            rememberedInitialProperties = r;
            r.Props = raycasted.GetPropertiesForRemember();
            movingObject.gravityScale = 0;
        }
    }

    void OnDrawGizmos()
    {
        if (movingObject == null)
        {
            return;
        }
        var rb2d = movingObject;
        Vector2 objectPosition = CenterOfMassFinder.FindObjectPosition(rb2d);
        Gizmos.DrawCube(objectPosition,new Vector3 (1,1,1));
    }

    void FixedUpdate()
    {
        if (!Input.GetMouseButton(1) || movingObject == null)
        {
            return;
        }
        movingObject.linearDamping = rememberedInitialProperties!.LinearDamping * linearDampingScale;
        var rb2d = movingObject;
        Vector2 objectPosition = CenterOfMassFinder.FindObjectPosition(rb2d);

        Vector2 playerPosition2D = new Vector2(player.transform.position.x, player.transform.position.y);

        Vector2 distancesFromPlayerToMouse = MousePositionHelper.FindDistancesToMouse(player.transform.position);
        Vector2 distancesFromObjToMouse = MousePositionHelper.FindDistancesToMouse(objectPosition);
        Vector2 distancesToObject = playerPosition2D - objectPosition;

        float absDistanceToObjectX = Mathf.Abs(distancesToObject.x);
        float absDistanceToObjectY = Mathf.Abs(distancesToObject.y);

        bool objectToTheRightOfPlayer = distancesToObject.x < 0;
        bool objectAbovePlayer = distancesToObject.y < 0;

        float totalDistanceToObject = absDistanceToObjectX + absDistanceToObjectY;

        bool isObjectMovingX = true;
        bool isObjectMovingY = true;

        if (absDistanceToObjectX >= maxDistance)
        {
            if (objectToTheRightOfPlayer && distancesFromPlayerToMouse.x > maxDistance)
            {
                isObjectMovingX = false;
            }
            if (!objectToTheRightOfPlayer && distancesFromPlayerToMouse.x < -maxDistance)
            {
                isObjectMovingX = false;
            }
        }
        if (absDistanceToObjectY >= maxDistance)
        {
            if (objectAbovePlayer && distancesFromObjToMouse.y > maxDistance)
            {
                isObjectMovingY = false;
            }
            if (!objectAbovePlayer && distancesFromObjToMouse.y < -maxDistance)
            {
                isObjectMovingY = false;
            }
        }
        if (isObjectMovingX)
        {
            if (totalDistanceToObject <= distanceToEat && 1 << movingObject.gameObject.layer == edibleLayer)
            {
                healthScript.AddHealth(healthToGive);
                Destroy(movingObject.gameObject);
                movingObject = null;
                return;
            }
            float maxForInterp = this.maxForInterp/rememberedInitialProperties.GravityScale;
            float p = (distancesFromObjToMouse.x - minForDistance) / (maxForDistance - minForDistance);
            Vector2 forceForAddingX = new Vector2 (distancesFromObjToMouse.x*Mathf.Lerp(minForInterp, maxForInterp, p*p),0);
            rb2d.AddForce(forceForAddingX);
        }
        else
        {
            movingObject.linearVelocityX = 0f;
        }
        if (isObjectMovingY)
        {
            float maxForInterp = this.maxForInterp/rememberedInitialProperties.GravityScale;
            float p = (distancesFromObjToMouse.y - minForDistance) / (maxForDistance - minForDistance);
            Vector2 forceForAddingY = new Vector2 (0,distancesFromObjToMouse.y*Mathf.Lerp(minForInterp, maxForInterp, p));
            rb2d.AddForce(forceForAddingY);
        }
        else
        {
            movingObject.linearVelocityY = 0f;
        }

        var b = movingObject.GetComponent<IObjectSelectedHandler>();
        if (b != null)
        {
            b.ProcessBeingSelected();
        }
    }
}

public interface IObjectSelectedHandler
{
    void ProcessBeingSelected();
    void Deselected();
}