using System.Linq;
using UnityEngine;

public class PlaceBuildBlockController : MonoBehaviour
{
    public bool canPlaceBuild = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Palma"))
        {
            canPlaceBuild = false;
        }
        else if (other.gameObject.CompareTag("Build"))
        {
            canPlaceBuild = false;
        }
        else if (other.gameObject.CompareTag("Environment"))
        {
            canPlaceBuild = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Palma"))
        {
            canPlaceBuild = false;
        }
        else if (other.gameObject.CompareTag("Build"))
        {
            canPlaceBuild = false;
        }
        else if (other.gameObject.CompareTag("Environment"))
        {
            canPlaceBuild = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Palma"))
        {
            canPlaceBuild = true;
        }
        else if (other.gameObject.CompareTag("Build"))
        {
            canPlaceBuild = true;
        }
        else if (other.gameObject.CompareTag("Environment"))
        {
            canPlaceBuild = true;
        }
    }
}
