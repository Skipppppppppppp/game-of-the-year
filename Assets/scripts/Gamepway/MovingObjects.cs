using System;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable

public sealed class MovingObjects : MonoBehaviour
{
    private int LayerMask;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        edibleLayer = 1 << UnityEngine.LayerMask.NameToLayer("Peopwe");
        foreach (var x in new[]
        {
            "Peopwe",
            "Moveable Stuff",
        })
        {
            var layer = UnityEngine.LayerMask.NameToLayer(x);
            LayerMask |= 1 << layer;
        }
        healthScript = player.GetComponent<ManageDamage>();
    }

    private Rigidbody2D? RaycastForObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(ray,float.PositiveInfinity,LayerMask);
        var trans = rayHit.transform;
        if (trans == null)
        {
            return null;
        }
        return rayHit.rigidbody;
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
            var raycasted = RaycastForObject();
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
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
        float distanceFromObjToMouseX = Mathf.Abs(mousePosition2D.x - objectPosition.x);
        float distanceFromObjToMouseY = Mathf.Abs(mousePosition2D.y - objectPosition.y);
        float distanceToObjectX = Mathf.Abs(player.transform.position.x - objectPosition.x);
        float distanceToMouseX =  Mathf.Abs(player.transform.position.x - mousePosition2D.x);
        float distanceToObjectY = Mathf.Abs(player.transform.position.y - objectPosition.y);
        float distanceToMouseY =  Mathf.Abs(player.transform.position.y - mousePosition2D.y);
        float totalDistanceToObject = distanceToObjectX + distanceToObjectY;
        bool isObjectMovingX = true;
        bool isObjectMovingY = true;
        if (distanceToObjectX >= 12 && distanceToMouseX >= distanceToObjectX)
        {
            isObjectMovingX = false;
        }
        if (distanceToObjectY >= 12 && distanceToMouseY >= distanceToObjectY)
        {
            isObjectMovingY = false;
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
            float directionX = mousePosition2D.x - objectPosition.x;
            float maxForInterp = this.maxForInterp/rememberedInitialProperties.GravityScale;
            float p = (distanceFromObjToMouseX - minForDistance) / (maxForDistance - minForDistance);
            Vector2 forceForAddingX = new Vector2 (directionX*Mathf.Lerp(minForInterp, maxForInterp, p),0);
            rb2d.AddForce(forceForAddingX);
        }
        else
        {
            movingObject.linearVelocityX = 0f;
        }
        if (isObjectMovingY)
        {
            float directionY = mousePosition2D.y - objectPosition.y;
            float maxForInterp = this.maxForInterp/rememberedInitialProperties.GravityScale;
            float p = (distanceFromObjToMouseY - minForDistance) / (maxForDistance - minForDistance);
            Vector2 forceForAddingY = new Vector2 (0,directionY*Mathf.Lerp(minForInterp, maxForInterp, p));
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