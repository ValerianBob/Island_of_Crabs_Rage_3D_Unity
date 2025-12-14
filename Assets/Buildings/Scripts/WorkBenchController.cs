using UnityEngine;

public class WorkBenchController : MonoBehaviour
{
    private CraftController _craftController;

    public float RangeForCraft;

    private float _distance;

    private void Start()
    {
        _craftController = GameObject.Find("Player").GetComponent<CraftController>();
    }

    void Update()
    {
        _distance = Vector3.Distance(_craftController.transform.position, transform.position);

        if (_distance <= RangeForCraft)
        {
            _craftController.isWorkBenchNear = true;
        }
        else
        {
            _craftController.isWorkBenchNear = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, RangeForCraft);
    }
}
