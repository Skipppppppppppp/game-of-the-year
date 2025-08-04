using System;
using TDCards.Editor;
using UnityEngine;

#nullable enable

public sealed class MovingObjects : MonoBehaviour
{
    private LayerMask layerMask;
    public GameObject player;
    public float minForInterp;
    public float maxForInterp;
    public float minForDistance;
    public float maxForDistance;
    private const float maxForInterpCoeff = 1;
    public Rigidbody2D? movingObject;
    private RememberInitialProperties? rememberedInitialProperties;
    [Range(0, 20)] public float linearDampingScale;
    public float distanceToEat = 1;
    private ManageDamage healthScript;
    private LayerMask edibleLayer;
    public float healthToGive = 15;
    private LayerMask obstacleLayer;
    public float maxDistance = 12;
    public float distanceToLetGo = 15;
    public Transform? grabbedPointTrans;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        edibleLayer = LayerMask.Peopwe;
        layerMask = LayerMask.Peopwe | LayerMask.MoveableStuff;
        obstacleLayer = LayerMask.Default;
        healthScript = player.GetComponent<ManageDamage>();
    }

    void LetGo()
    {
        var r = rememberedInitialProperties!;
        var b = movingObject!.GetComponent<IObjectSelectedHandler>();
        if (b != null)
        {
            b.Deselected();
        }

        movingObject.gravityScale = r.GravityScale;
        movingObject.linearDamping = r.LinearDamping;
        Destroy(r);

        Transform[] transChildren = movingObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in transChildren)
        {
            if (child.tag == "Grabbed Point")
            {
                Destroy(child.gameObject);
                break;
            }
        }

        movingObject = null;
        return;
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
            LetGo();
            return;
        }

        if (movingObject == null)
        {
            var raycasted = RaycastHelper.TrySelectObject(
                playerPos: player.transform.position,
                maxDistance: maxDistance,
                layerMask: (int) layerMask,
                obstacleLayerMask: (int) obstacleLayer);
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

            GameObject grabbedPoint = new GameObject("Grabbed Point");
            grabbedPoint.tag = "Grabbed Point";
            grabbedPointTrans = grabbedPoint.transform;
            grabbedPointTrans.parent = movingObject.transform;

            Vector2 objectCOM = CenterOfMassFinder.FindObjectPosition(movingObject);
            Vector2 distanceFromMouseToCOM = MousePositionHelper.FindDistancesToMouse(objectCOM);

            grabbedPointTrans.position = distanceFromMouseToCOM + objectCOM;

            rememberedInitialProperties = r;
            r.Props = raycasted.GetPropertiesForRemember();
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
        Gizmos.DrawCube(objectPosition, new Vector3(1, 1, 1));
    }

    void FixedUpdate()
    {
        if (!Input.GetMouseButton(1) || movingObject == null)
            return;

        Vector2 grabbedPointPos = grabbedPointTrans.position;
        movingObject.linearDamping = rememberedInitialProperties!.LinearDamping * linearDampingScale;

        var rb2d = movingObject;
        Vector2 objectPosition = CenterOfMassFinder.FindObjectPosition(rb2d);
        Vector2 playerPosition2D = player.transform.position;
        Vector2 mouseToObj = MousePositionHelper.FindDistancesToMouse(grabbedPointPos);
        Vector2 mouseToPlayer = MousePositionHelper.FindDistancesToMouse(playerPosition2D);
        Vector2 directionToObject = playerPosition2D - grabbedPointPos;

        float totalDistanceToObject = directionToObject.magnitude;


        if (totalDistanceToObject <= distanceToEat && edibleLayer.Contains((Layer) movingObject.gameObject.layer))
        {
            healthScript.AddHealth(healthToGive);
            Destroy(movingObject.gameObject);
            movingObject = null;
            return;
        }

        if (Mathf.Abs(directionToObject.x) >= distanceToLetGo || directionToObject.y >= distanceToLetGo)
        {
            LetGo();
            return;
        }

        Vector2 force = Vector2.zero;
        Vector2[] axes = { Vector2.right, Vector2.up };

        for (int i = 0; i < axes.Length; i++)
        {
            Vector2 axis = axes[i];
            float direction = Vector2.Dot(directionToObject, axis);
            float distanceToObjectAxis = Mathf.Abs(direction);

            float distanceFromMouseAxis = Vector2.Dot(mouseToObj, axis);
            float distanceFromPlayerMouseAxis = Vector2.Dot(mouseToPlayer, axis);

            if (distanceToObjectAxis < maxDistance ||
                (direction < 0 && distanceFromPlayerMouseAxis <= maxDistance) ||
                (direction > 0 && distanceFromPlayerMouseAxis >= -maxDistance))
            {
                float distanceFactor = Mathf.Clamp01((Mathf.Abs(distanceFromMouseAxis) - minForDistance) / (maxForDistance - minForDistance));
                float interpolatedForce = Mathf.Lerp(minForInterp, maxForInterp / rememberedInitialProperties.GravityScale, distanceFactor * distanceFactor);
                force += axis * distanceFromMouseAxis * interpolatedForce;
            }
            else
            {
                if (distanceToObjectAxis >= distanceToLetGo)
                {
                    LetGo();
                    return;
                }

                if (axis == Vector2.right)
                    movingObject.linearVelocityX = 0f;
                else
                    movingObject.linearVelocityY = 0f;

                if (distanceToObjectAxis - maxDistance >= 0.5)
                {
                    movingObject.AddForce(directionToObject * 50);
                    return;
                }
            }
        }

        rb2d.AddForceAtPosition(force, grabbedPointPos);
        var handler = movingObject.GetComponent<IObjectSelectedHandler>();
        handler?.ProcessBeingSelected();
    }

}

public interface IObjectSelectedHandler
{
    void ProcessBeingSelected();
    void Deselected();
}