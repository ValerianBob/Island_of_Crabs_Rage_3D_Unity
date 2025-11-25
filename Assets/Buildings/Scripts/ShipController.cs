using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipController : MonoBehaviour
{
    [SerializeField] private Camera CutSceneCamera;

    [SerializeField] private GameObject ShipStage1;
    [SerializeField] private GameObject ShipStage2;
    [SerializeField] private GameObject ShipStage3;

    [SerializeField] private TextMeshProUGUI WoodBoardAmountText;
    [SerializeField] private TextMeshProUGUI StoneAmountText;
    [SerializeField] private TextMeshProUGUI IronAmountText;

    [SerializeField] private GameObject UICanvas;
    [SerializeField] private GameObject CutSceneCanvas;

    public static event Action OnGameOver;

    public int CurrentWoodBoardAmount = 0;
    public int CurrentStoneAmount = 0;
    public int CurrentIronAmount = 0;

    public int WoodBoardNeed = 10000;
    public int StoneNeed = 10000;
    public int IronNeed = 5000;

    private bool _changeStage;

    public bool isGameOver;

    private void Start()
    {
        WoodBoardAmountText.text = $"{CurrentWoodBoardAmount}/{WoodBoardNeed}";
        StoneAmountText.text = $"{CurrentStoneAmount}/{StoneNeed}";
        IronAmountText.text = $"{CurrentIronAmount}/{IronNeed}";
    }

    private void Update()
    {
        if ((CurrentWoodBoardAmount > 0 || CurrentStoneAmount > 0 || CurrentIronAmount > 0) && !_changeStage)
        {
            ShipStage1.SetActive(false);
            ShipStage2.SetActive(true);
            _changeStage = true;
        }
        else if ((CurrentWoodBoardAmount >= WoodBoardNeed && CurrentStoneAmount >= StoneNeed && CurrentIronAmount >= IronNeed) && !isGameOver)
        {
            isGameOver = true;

            ShipStage1.SetActive(false);
            ShipStage2.SetActive(false);
            ShipStage3.SetActive(true);

            EndGame();
        }

        if (isGameOver)
        {
            ShipStage3.transform.Translate(Vector3.up * 3f * Time.deltaTime);
        }
    }
    
    public void UpdateShipConditionUI()
    {
        WoodBoardAmountText.text = $"{CurrentWoodBoardAmount}/{WoodBoardNeed}";
        StoneAmountText.text = $"{CurrentStoneAmount}/{StoneNeed}";
        IronAmountText.text = $"{CurrentIronAmount}/{IronNeed}";
    }

    private void EndGame()
    {
        UICanvas.SetActive(false);

        CutSceneCanvas.SetActive(true);
        CutSceneCamera.gameObject.SetActive(true);

        OnGameOver?.Invoke();

        StartCoroutine("CastSceneEndGame");
    }

    private IEnumerator CastSceneEndGame()
    {
        yield return new WaitForSeconds(10f);

        CursorVisabilityController.Instance.SetCursorVisability(true);

        SceneManager.LoadScene(0);
    }
}
