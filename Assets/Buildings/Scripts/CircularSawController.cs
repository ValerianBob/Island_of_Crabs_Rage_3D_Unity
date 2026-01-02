using System.Collections;
using UnityEngine;

public class CircularSawController : MonoBehaviour
{
    [SerializeField] private GameObject WoodBoardPrefab;

    [SerializeField] private GameObject DropPoint;

    [SerializeField] private Texture EmptyIcon;

    [System.Serializable]
    public struct CircularSawSlot
    {
        public ItemData Item;
        public int Quantity;
    }

    public CircularSawSlot WoodSlot;

    public string ObjectName;
    public string Info;

    private float _cutSpeed = 1.0f;
    private float _currentTime = 5f;

    private Coroutine CuttingWoodCoroutine;

    private bool isCutting = false;

    public bool isCirculatSawSelected = false;

    private void Update()
    {
        if (isCirculatSawSelected)
        {
            UIManager.Instance.CutTimeText.text = _currentTime.ToString();
        }

        if (WoodSlot.Quantity <= 0 && isCutting)
        {
            StopCoroutine(CuttingWoodCoroutine);

            _currentTime = 5f;
            isCutting = false;
        }
    }

    public void CutWood()
    {
        if (WoodSlot.Quantity > 0 && !isCutting)
        {
            CuttingWoodCoroutine = StartCoroutine(CuttingWood());

            SoundsController.Instance.PlayBuilds(0, transform.position);
        }
    }

    private IEnumerator CuttingWood()
    {
        isCutting = true;

        while (_currentTime != 0f)
        {
            yield return new WaitForSeconds(_cutSpeed);

            _currentTime -= 1f;
        }

        DropWoodBoard();
    }

    private void DropWoodBoard()
    {
        GameObject tempWoodBoard = Instantiate(WoodBoardPrefab, DropPoint.transform.position, Quaternion.identity);

        tempWoodBoard.GetComponent<ItemController>().Quantity = WoodSlot.Quantity;

        WoodSlot.Item = null;
        WoodSlot.Quantity = 0;

        UIManager.Instance.WoodCiruclarSaw.ItemIcon.texture = EmptyIcon;
        UIManager.Instance.WoodCiruclarSaw.QuantityText.text = "0";

        isCutting = false;
        _currentTime = 5f;
    }
}
