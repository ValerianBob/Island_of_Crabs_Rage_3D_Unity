using UnityEngine;

public class WaterCollectionController : MonoBehaviour
{
    public string ObjectName;
    public string Info;

    public int Water;

    private int WaterCollectingSpeed = 1;

    private float NextTime = 0;

    private void Update()
    {
        if (Time.time > NextTime)
        {
            NextTime = Time.time + WaterCollectingSpeed;

            Water += 1;
            ObjectName = $"Water {Water} ml";
        }
    }

    public void DrinkWater(PlayerConditionController water, int number)
    {
        if (number > Water)
        {
            water.AddWater(Water);

            Water = 0;
            ObjectName = $"Water {Water} ml";
        }
        else
        {
            water.AddWater(number);

            Water -= number;
            ObjectName = $"Water {Water} ml";
        }
    }
}
