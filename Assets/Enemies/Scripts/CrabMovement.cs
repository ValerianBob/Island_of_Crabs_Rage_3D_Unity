using UnityEngine;
using UnityEngine.AI;

public class CrabMovement : MonoBehaviour
{
    private NavMeshAgent _agent;

    public Transform Player;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        Player = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        _agent.SetDestination(Player.position);
    }
}
