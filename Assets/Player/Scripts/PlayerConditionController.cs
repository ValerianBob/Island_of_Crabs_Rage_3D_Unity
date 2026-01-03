using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerConditionController : MonoBehaviour
{
    public static event Action Dead;
    public static event Action Respawn;

    [SerializeField] private GameObject DeadWindow;
    [SerializeField] private TextMeshProUGUI RespawnTime;

    [SerializeField] private Slider HelathBar;
    [SerializeField] private Slider HungerBar;
    [SerializeField] private Slider ThirstBar;

    private int GettingSustenanceSpeed = 5;
    private int DyingFromSustenanceSpeed = 1;

    private float NextTimeGetSustenance = 0f;
    private float NextTimeDyingFromSustenance = 0f;

    private float _maxConditions = 100f;

    public float Health = 100f;
    public float Hunger = 100f;
    public float Thirs = 100f;

    private int _timeToRespawn = 10;

    private bool _isDead = false;

    private void Update()
    {
        HelathBar.value = Health;
        HungerBar.value = Hunger;
        ThirstBar.value = Thirs;

        if (Time.time > NextTimeGetSustenance)
        {
            NextTimeGetSustenance = Time.time + GettingSustenanceSpeed;

            GettingHungerAndThirsty(1);
        }

        if (Time.time > NextTimeDyingFromSustenance)
        {
            NextTimeDyingFromSustenance = Time.time + DyingFromSustenanceSpeed;

            if (Hunger == 0)
            {
                ChangeHealth(1, true);
            }
            if (Thirs == 0)
            {
                ChangeHealth(1, true);
            }
        }

        if (Health <= 0 && !_isDead)
        {
            _isDead = true;

            Dead?.Invoke();
            StartCoroutine("RespawnCount");
        }
    }

    public void ChangeHealth(int number, bool isHurting)
    {
        if (isHurting)
        {
            Health -= number;
            if (Health < 0)
            {
                Health = 0;
            }
        }
        else
        {
            Health += number;
            if (Health > _maxConditions)
            {
                Health = _maxConditions;
            }
        }
    }

    private void GettingHungerAndThirsty(int number)
    {
        Hunger -= number;

        if (Hunger < 0)
        {
            Hunger = 0;
        }

        if (Hunger > _maxConditions)
        {
            Hunger = _maxConditions;
        }

        Thirs -= number;

        if (Thirs < 0)
        {
            Thirs = 0;
        }

        if (Thirs > _maxConditions)
        {
            Thirs = _maxConditions;
        }
    }

    public void AddWater(int number)
    {
        Thirs += number;
        if (Thirs > _maxConditions)
        {
            Thirs = _maxConditions;
        }
    }

    private IEnumerator RespawnCount()
    {
        DeadWindow.SetActive(true);

        int countdown = _timeToRespawn;
        RespawnTime.text = countdown.ToString();

        while (countdown > 0)
        {
            RespawnTime.text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        DeadWindow.SetActive(false);

        Health = 50;
        Hunger = 50;
        Thirs = 50;

        _isDead = false;

        Respawn?.Invoke();
    }

    public void ChangeHunger(int number)
    {
        Hunger += number;
    }
}
