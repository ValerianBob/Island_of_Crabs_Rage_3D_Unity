using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HammerController : MonoBehaviour
{
    [SerializeField] private InventoryController _inventoryController;

    [SerializeField] private GameObject FurnaceUI;
    [SerializeField] private GameObject CraftUI;

    [System.Serializable]
    private struct BuildBluePrint
    {
        public GameObject BuildPrefab;

        public int WoodQuantity;
        public int StoneQuantity;
        public int IronQuantity;

        public TextMeshProUGUI WoodText;
        public TextMeshProUGUI StoneText;
        public TextMeshProUGUI IronText;
    }

    [SerializeField] private BuildBluePrint[] BuildingsBluePrints;

    [SerializeField] private Camera Camera;
    [SerializeField] private Terrain Terrain;
    [SerializeField] private GameObject HelpKeysText;
    [SerializeField] private GameObject BuildBluePrintInfo;

    [SerializeField] private Texture EmptyIcon;

    private PlaceBuildBlockController _placeBuildBlockController;

    private GameObject _currentBuildPrefab;

    private Dictionary<Material, Color> _originalColors = new Dictionary<Material, Color>();

    private int _currentBuildIndex = 0;

    private float alpha = 0.5f;

    //Position :
    private Vector3 ForwardPos;

    private float terrainY;
    private float buildDistance = 4f;

    // Scroll :
    private float scroll = 0f;
    private float currentRotationY = 0f;
    private float rotationSpeed = 10f;

    public bool canPlaceBuild = true;
    public bool isBuilding = false;

    private void Update()
    {
        if (!isBuilding || _currentBuildPrefab == null)
        {
            return;
        }

        GetMouseWheelRotation();

        if (isBuilding)
        {
            MoveBuild();

            SelectBuild();

            PlaceBuild();
        }

        if (_placeBuildBlockController != null)
        {
            canPlaceBuild = GetPlcaeBuildBlock().canPlaceBuild;

            if (!canPlaceBuild)
            {
                SetPreviewMaterial(_currentBuildPrefab, Color.red, alpha);
            }
            else
            {
                SetPreviewMaterial(_currentBuildPrefab, Color.green, alpha);
            }
        }
    }

    private void MoveBuild()
    {
        ForwardPos = Camera.transform.position + Camera.transform.forward * buildDistance;

        terrainY = Terrain.SampleHeight(ForwardPos);

        _currentBuildPrefab.transform.position = new Vector3(ForwardPos.x, terrainY, ForwardPos.z);
        _currentBuildPrefab.transform.rotation = Quaternion.Euler(0f, Camera.transform.eulerAngles.y + currentRotationY, 0f);
    }

    private void GetMouseWheelRotation()
    {
        scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0)
        {
            currentRotationY += scroll * rotationSpeed;
        }
    }

    private void SelectBuild()
    {
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            if (_currentBuildIndex > 0)
            {
                ChangeBuild(-1);
            }
        }
        else if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            if (_currentBuildIndex < BuildingsBluePrints.Length - 1)
            {
                ChangeBuild(1);
            }
        }
    }

    private void ChangeBuild(int index)
    {
        _currentBuildIndex += index;

        RestoreMaterialColors();
        Destroy(_currentBuildPrefab);

        _currentBuildPrefab = Instantiate(BuildingsBluePrints[_currentBuildIndex].BuildPrefab,
            new Vector3(ForwardPos.x, terrainY),
            BuildingsBluePrints[_currentBuildIndex].BuildPrefab.transform.rotation);

        _placeBuildBlockController = GetPlcaeBuildBlock();

        SetCostText(_currentBuildIndex);

        SetPreviewMaterial(_currentBuildPrefab, Color.green, alpha);
    }


    private void PlaceBuild()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame && canPlaceBuild)
        {
            TryPlaceBuild(_currentBuildIndex);
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame && !canPlaceBuild)
        {
            Notifications.Instance.CreateNotification("Can't Place Build here", Color.red);
        }
    }

    private void SetPreviewMaterial(GameObject buildObject, Color color, float alpha)
    {
        Renderer[] renderers = buildObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                if (!_originalColors.ContainsKey(mat))
                {
                    if (mat.HasProperty("_BaseColor"))
                    {
                        _originalColors[mat] = mat.GetColor("_BaseColor");
                    }
                    else if (mat.HasProperty("_Color"))
                    {
                        _originalColors[mat] = mat.GetColor("_Color");
                    }
                }

                if (mat.HasProperty("_Surface"))
                {
                    mat.SetFloat("_Surface", 1f);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                    mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    mat.DisableKeyword("_SURFACE_TYPE_OPAQUE");

                    mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetFloat("_ZWrite", 0.0f);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                }

                Color newColor = new Color(color.r, color.g, color.b, alpha);

                if (mat.HasProperty("_BaseColor"))
                {
                    mat.SetColor("_BaseColor", newColor);
                }
                else if (mat.HasProperty("_Color"))
                {
                    mat.SetColor("_Color", newColor);
                }
            }
        }
    }

    private void RestoreMaterialColors()
    {
        foreach (var color in _originalColors)
        {
            Material mat = color.Key;
            Color original = color.Value;
            original.a = 1f;

            if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", original);
            }
            else if (mat.HasProperty("_Color"))
            {
                mat.SetColor("_Color", original);
            }

            mat.SetFloat("_Surface", 0f);
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            mat.EnableKeyword("_SURFACE_TYPE_OPAQUE");
            mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.SetFloat("_ZWrite", 1f);
            mat.DisableKeyword("_ALPHABLEND_ON");
        }

        _originalColors.Clear();
    }

    private void SetCostText(int index)
    {
        BuildingsBluePrints[index].WoodText.text = BuildingsBluePrints[index].WoodQuantity.ToString();

        BuildingsBluePrints[index].StoneText.text = BuildingsBluePrints[index].StoneQuantity.ToString();

        BuildingsBluePrints[index].IronText.text = BuildingsBluePrints[index].IronQuantity.ToString();
    }

    private void TryPlaceBuild(int index)
    {
        int remainingWoods = BuildingsBluePrints[index].WoodQuantity;
        int remainingStones = BuildingsBluePrints[index].StoneQuantity;
        int remainingIrons = BuildingsBluePrints[index].IronQuantity;

        int availableWoods = 0;
        int availableStones = 0;
        int availableIrons = 0;

        for (int i = 0; i < _inventoryController.Slots.Length; i++)
        {
            if (_inventoryController.Slots[i].Item == null)
            {
                continue;
            }

            if (_inventoryController.Slots[i].Item.ItemName == "Wood")
            {
                availableWoods += _inventoryController.Slots[i].Quantity;
            }
            else if (_inventoryController.Slots[i].Item.ItemName == "Stone")
            {
                availableStones += _inventoryController.Slots[i].Quantity;
            }
            else if (_inventoryController.Slots[i].Item.ItemName == "Iron")
            {
                availableIrons += _inventoryController.Slots[i].Quantity;
            }
        }

        if (availableWoods < remainingWoods || availableStones < remainingStones || availableIrons < remainingIrons)
        {
            Notifications.Instance.CreateNotification("Not enough resources", Color.red);
            return;
        }

        List<int> itemsIndexesToDelete = new List<int>();

        for (int i = 0; i < _inventoryController.Slots.Length; i++)
        {
            if (BuildingsBluePrints[index].WoodQuantity > 0 && remainingWoods != 0)
            {
                if (_inventoryController.Slots[i].Item != null)
                {
                    if (_inventoryController.Slots[i].Item.ItemName == "Wood")
                    {
                        remainingWoods = RemoveResourceFromSlot(i, remainingWoods, itemsIndexesToDelete);
                    }
                }
            }

            if (BuildingsBluePrints[index].StoneQuantity > 0 && remainingStones != 0)
            {
                if (_inventoryController.Slots[i].Item != null)
                {
                    if (_inventoryController.Slots[i].Item.ItemName == "Stone")
                    {
                        remainingStones = RemoveResourceFromSlot(i, remainingStones, itemsIndexesToDelete);
                    }
                }
            }

            if (BuildingsBluePrints[index].IronQuantity > 0 && remainingIrons != 0)
            {
                if (_inventoryController.Slots[i].Item != null)
                {
                    if (_inventoryController.Slots[i].Item.ItemName == "Iron")
                    {
                        remainingIrons = RemoveResourceFromSlot(i, remainingIrons, itemsIndexesToDelete);
                    }
                }
            }
        }

        if (remainingWoods == 0 && remainingStones == 0 && remainingIrons == 0)
        {
            _inventoryController.ClearSlots(itemsIndexesToDelete);

            RestoreMaterialColors();

            Collider[] Colliders = _currentBuildPrefab.transform.GetChild(0).GetComponents<Collider>();

            foreach (Collider collider in Colliders)
            {
                collider.isTrigger = false;
            }

            _currentBuildPrefab = Instantiate(BuildingsBluePrints[index].BuildPrefab,
                new Vector3(ForwardPos.x, terrainY),
                BuildingsBluePrints[index].BuildPrefab.transform.rotation);

            _placeBuildBlockController = GetPlcaeBuildBlock();

            SoundsController.Instance.PlayInstruments(3, transform.position);

            SetPreviewMaterial(_currentBuildPrefab, Color.green, alpha);
        }
    }

    private int RemoveResourceFromSlot(int i, int remainingAmount, List<int> itemsIndexesToDelete)
    {
        if (remainingAmount > _inventoryController.Slots[i].Quantity)
        {
            itemsIndexesToDelete.Add(i);
            remainingAmount -= _inventoryController.Slots[i].Quantity;
        }
        else
        {
            _inventoryController.Slots[i].Quantity -= remainingAmount;
            _inventoryController.Slots[i].QuantityText.text = _inventoryController.Slots[i].Quantity.ToString();

            remainingAmount = 0;

            if (_inventoryController.Slots[i].Quantity == 0)
            {
                _inventoryController.Slots[i].Item = null;
                _inventoryController.Slots[i].ItemIcon.texture = EmptyIcon;
            }
        }

        return remainingAmount;
    }

    private PlaceBuildBlockController GetPlcaeBuildBlock()
    {
        PlaceBuildBlockController BuildModel = _currentBuildPrefab.transform.GetChild(0).gameObject.GetComponent<PlaceBuildBlockController>();

        return BuildModel;
    }

    private void OnEnable()
    {
        isBuilding = true;

        HelpKeysText.SetActive(true);
        BuildBluePrintInfo.SetActive(true);
        SetCostText(_currentBuildIndex);

        _currentBuildPrefab = Instantiate(BuildingsBluePrints[_currentBuildIndex].BuildPrefab,
            new Vector3(ForwardPos.x, terrainY),
            BuildingsBluePrints[_currentBuildIndex].BuildPrefab.transform.rotation);

        _placeBuildBlockController = GetPlcaeBuildBlock();

        SetPreviewMaterial(_currentBuildPrefab, Color.green, alpha);
    }

    private void OnDisable()
    {
        isBuilding = false;

        HelpKeysText.SetActive(false);
        BuildBluePrintInfo.SetActive(false);

        _placeBuildBlockController = null;

        Destroy(_currentBuildPrefab);
    }
}
