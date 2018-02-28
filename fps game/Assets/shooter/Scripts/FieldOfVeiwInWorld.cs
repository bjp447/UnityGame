using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfVeiwInWorld : MonoBehaviour
{
    [SerializeField] private float fovRadius;
    [SerializeField] private float viewAngle;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private List<Transform> visibleTargets = new List<Transform>();

    private void Start()
    {
        StartCoroutine("FindTargetsDelay", 0.2f);
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireSphere(transform.position, fovRadius);

        Vector3 viewingAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewingAngleB = DirFromAngle(viewAngle / 2, false);
        Gizmos.DrawLine(transform.position, transform.position + viewingAngleA * fovRadius);//transform.position + viewAngle * fovRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewingAngleB * fovRadius);//transform.position + viewAngle * fovRadius);

        Gizmos.color = Color.red;
        foreach (Transform visibleTarget in visibleTargets)
        {
            Gizmos.DrawLine(transform.position, visibleTarget.position);
        }
    }
    */

    
    private IEnumerator FindTargetsDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            visibleTargets = FindTargetsInView();
            //print("myList size: " + myList.Count);
        }
    }
    
    //finds all objects in view
    public List<Transform> FindTargetsInView()
    {
        List<Transform> visibleTargets = new List<Transform>();
        visibleTargets.Clear();

        foreach (Collider targetCollider in Physics.OverlapSphere(transform.position, fovRadius))
        {
            Transform target = targetCollider.transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                RaycastHit hit;
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (Physics.Raycast(transform.position, directionToTarget ,out hit, distanceToTarget))
                {
                    if (hit.collider == targetCollider)
                        visibleTargets.Add(hit.transform);
                }
            }
        }
        return visibleTargets;
    }

    //find objects within view of string: tag
    public List<Transform> FindTargetsInView(string tag)
    {
        List<Transform> visibleTargets = new List<Transform>();
        visibleTargets.Clear();

        foreach (Collider targetCollider in Physics.OverlapSphere(transform.position, fovRadius))
        {
            if (targetCollider.CompareTag(tag) == true)
            {
                //print("object with tag: " + tag + "found");
                Transform target = targetCollider.transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
                {
                    RaycastHit hit;
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);

                    if (Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget))
                    {
                        if (hit.collider == targetCollider)
                            visibleTargets.Add(hit.transform);
                    }
                }
            }
        }
        return visibleTargets; 
    }

    //finds objects based on masks
    public List<Transform> FindTargetsInView(LayerMask obstaculMask, LayerMask targetMask)
    {
        List<Transform> visibleTargets = new List<Transform>();
        visibleTargets.Clear();

        foreach (Collider targetCollider in Physics.OverlapSphere(transform.position, fovRadius, targetMask))
        {
            Transform target = targetCollider.transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstaculMask))
                    visibleTargets.Add(target);
            }
        }
        return visibleTargets;
    }

    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (angleIsGlobal == false)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees*Mathf.Deg2Rad), 0 , Mathf.Cos(angleInDegrees*Mathf.Deg2Rad));
    }
}
