using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Navigation : MonoBehaviour
{

    public UnityEngine.AI.NavMeshAgent navmeshAgent;

    public LayerMask groundMask;
	public int walkableLayer = 1;

	public float samplingDistance = 10f;

    public float randomWalkRadius = 20f;

    public bool useNavmesh{get; set;}

    void Awake(){

        // navmeshAgent = GetComponent<NavMeshAgent>();
        // navmeshAgent.enabled = false;

    }

    // Use this for initialization
    void Start()
    { 
        
		//AdjustPosition();
        //navmeshAgent.enabled = true;
        //SetNewRandomTarget();
    }

    public void AdjustPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 500f, Vector3.down, out hit, 1000f, groundMask))
        {
            float altitude = hit.point.y + 3f;
            transform.position = new Vector3(transform.position.x, altitude, transform.position.z);
    
        }
        else
        {
            Debug.LogError("NPC : adjust position : raycast failed");
        }

        if(!useNavmesh) return;
 

		NavMeshHit hit2;
        if (NavMesh.SamplePosition(transform.position, out hit2, samplingDistance, walkableLayer))
        {
            
            //transform.position = hit2.position + Vector3.up * 2f;
            navmeshAgent.Warp(hit2.position);
        }
        else
        {
            Debug.LogError("NPC : adjust position : navmesh sampling failed");
        }
    }

    public void SetNewRandomTarget()
    {

        NavMeshHit hit;
        Vector2 rand = Random.insideUnitCircle * randomWalkRadius;
        NavMesh.SamplePosition(transform.position + new Vector3(rand.x, 0f, rand.y), out hit, samplingDistance, walkableLayer);
        navmeshAgent.SetDestination(hit.position);
        //Debug.Log("new target : " + hit.position);
    }

    // Update is called once per frame
    void Update()
    {

        if (useNavmesh && navmeshAgent.remainingDistance < 0.5f) SetNewRandomTarget();

    }
}
