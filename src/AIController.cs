using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIController : ParentTrigger
{
    enum STATES { IDLE, PATROL, HOSTILE };

    private EnemyEntity entity;
    private BaseEntity target;
    private STATES state;

    // patrol
    public GameObject patrolNodesParent;
    private Transform[] patrolNodes;
    private int nextPatrol;
    private const float nodeReachDistance = 0.3f;

    // detect
    [Range(0.0f, 180.0f)]
    public float fov = 70f;
    public bool dontFollow = false;

    // audio
    public AudioClip aggroSound;

	void Start () 
    {
        target = null;
        entity = gameObject.GetComponent<EnemyEntity>();
        patrolNodes = null;
        initializePatrolNodes();
        nextPatrol = 0;

        if (patrolNodesParent == null)
            state = STATES.IDLE;
        else
            state = STATES.PATROL;
	}

    void initializePatrolNodes()
    {
        if (patrolNodesParent == null)
            return;

        patrolNodes = patrolNodesParent.GetComponentsInChildren<Transform>();

        List<Transform> pl = new List<Transform>();
        foreach(Transform t in patrolNodes)
        {
            if (t.gameObject != patrolNodesParent)
            {
                pl.Add(t);
            }
        }
        patrolNodes = pl.ToArray();
    }
	
	void Update () 
    {
        if (GC.PAUSED)
            return;
        
        switch (state)
        {
            case STATES.IDLE: stateIdle(); break;
            case STATES.PATROL: statePatrol(); break;
            case STATES.HOSTILE: stateHostile(); break;
        }
	}

    protected virtual void stateIdle()
    {
        if (target != null)
        {
            state = STATES.HOSTILE;
            return;
        }

        if (patrolNodesParent != null)
        {
            state = STATES.PATROL;
            nextPatrol = 0;
        }
        
        bool isGrounded = entity.Move(Vector3.zero);
    }

    protected virtual void statePatrol()
    {
        if (target != null)
        {
            state = STATES.HOSTILE;
            return;
        }
        
        if (patrolNodes[0] == null)
        {
            state = STATES.IDLE;
            return;
        }

        Vector3 nextNode = patrolNodes[nextPatrol].position;
        nextNode.y = transform.position.y;

        if (Vector3.Distance(transform.position, nextNode) > nodeReachDistance)
        {
            direction.Normalize();
            entity.Move(direction);
            Vector3 direction = nextNode - transform.position;
        }
        else
        {
            ++nextPatrol;
            if (nextPatrol > patrolNodes.Length - 1)
                nextPatrol = 0;
        }
    }

    protected virtual void stateHostile()
    {
        if (target == null)
        {
            state = STATES.IDLE;
            return;
        }

        entity.Move(Vector3.zero);

        float targetDistance = Vector3.Distance(transform.position, target.transform.position);
        if (targetDistance <= entity.attackDistance)
        {
            entity.attack(target);
        }
        else if (!dontFollow)
        {
            Vector3 direction = target.transform.position - transform.position;
            direction.Normalize();
            entity.Move(direction);
        }
    }

    public override void childOnTriggerStay(Collider other)
    {
        PlayerEntity pe = other.gameObject.GetComponent<PlayerEntity>();
        if (pe != null)
        {
            Physics.Linecast(transform.position, other.transform.position, out hitInfo);
            RaycastHit hitInfo;
            bool clearLos = hitInfo.collider == other;
            float angle = Vector3.Angle(other.transform.position - transform.position, transform.forward);

            if (clearLos && angle < fov)
            {
                target = pe;
                state = STATES.HOSTILE;
            }
        }
    }

    public override void childOnTriggerExit(Collider other)
    {

    }

    public void damaged(BaseEntity attacker)
    {
        attacker = attacker as PlayerEntity;
        if (attacker != null)
        {
            target = attacker;
            state = STATES.HOSTILE;
        }
    }
    public void onKill()
    {
        target = null;
    }
}