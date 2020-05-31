using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticPathFinder : MonoBehaviour
{
    public float creatureSpeed;
    public float pathMultiplier;
    public float rotationSpeed;
    int pathIndex = 0;
    public DNA dna;
    public bool hasFinished = false;
    bool hasBeenInitialized = false;
    bool hasCrashed = false;
    public LayerMask obstacleLayer;
    List<Vector2> travelPath = new List<Vector2>();
    Vector2 target;
    Vector2 nextPoint;
    Quaternion targetRotation;
    LineRenderer lr;

    /*private void Start()
    {
        InitCreature(new DNA(), Vector2.zero);
    }*/
    public void InitCreature(DNA newDna, Vector2 _target)
    {
        travelPath.Add(transform.position);
        lr = GetComponent<LineRenderer>();
        dna = newDna;
        target = _target;
        nextPoint = transform.position;
        travelPath.Add(nextPoint);
        hasBeenInitialized = true;
    }
    private void Update()
    {
        if (hasBeenInitialized && !hasFinished)
        {
            if(pathIndex == dna.genes.Count || 
                Vector2.Distance(transform.position, target) < 0.5f)
            {
                hasFinished = true;
            }
            if((Vector2)transform.position == nextPoint)
            {
                nextPoint = (Vector2)transform.position + dna.genes[pathIndex] * pathMultiplier;
                travelPath.Add(nextPoint);
                targetRotation = LookAt2D(nextPoint);
                pathIndex++;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, 
                    nextPoint, creatureSpeed * Time.deltaTime);
            }
            if(transform.rotation != targetRotation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
                    rotationSpeed * Time.deltaTime);
            }
            RenderLine();
        }
    }
    public void RenderLine()
    {
        List<Vector3> linePoints = new List<Vector3>();
        if(travelPath.Count > 2)
        {
            for (int i = 0; i < travelPath.Count - 1; i++)
            {
                linePoints.Add(travelPath[i]);
            }
            linePoints.Add(transform.position);
        }
        else
        {
            linePoints.Add(travelPath[0]);
            linePoints.Add(transform.position);
        }
        lr.positionCount = linePoints.Count;
        lr.SetPositions(linePoints.ToArray());
    }
    public float fitness
    {
        get
        {
            float dist = Vector2.Distance(transform.position, target);
            if(dist == 0)
            {
                dist = 0.0001f;
            }
            RaycastHit2D[] obstacles = Physics2D.RaycastAll(transform.position, target, obstacleLayer);
            float obstacleMultiplier = 1f - (0.15f * obstacles.Length);
            return (60/dist) * (hasCrashed ? 0.75f : 1f) * obstacleMultiplier;
        }
    }

    public Quaternion LookAt2D(Vector2 target, float angleOffset = -90)
    {
        Vector2 fromTo = (target - (Vector2)transform.position).normalized;
        float zRotation = Mathf.Atan2(fromTo.y, fromTo.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, zRotation + angleOffset);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            hasFinished = true;
            hasCrashed = true;
        }
    }
}
