using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HammerController : MonoBehaviour
{
    [SerializeField] private Camera Camera;
    [SerializeField] private GameObject[] BuildingsPrefabs;
    [SerializeField] private Terrain Terrain;
    [SerializeField] private GameObject HelpKeysText;

    private GameObject _currentBuildPrefab;

    private Dictionary<Material, Color> _originalColors = new Dictionary<Material, Color>();

    private float buildDistance = 4f;

    private float alpha = 0.5f;

    private float scroll = 0f;
    private float currentRotationY = 0f;
    private float rotationSpeed = 10f;

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

            ChangeBuild();

            PlaceBuild();
        }
    }

    private void MoveBuild()
    {
        Vector3 ForwardPos = Camera.transform.position + Camera.transform.forward * buildDistance;
        float terrainY = Terrain.SampleHeight(ForwardPos);
        Vector3 offset = Camera.transform.right * 2f + Camera.transform.forward * 2f;

        float objectHeight = _currentBuildPrefab.GetComponentInChildren<Renderer>().bounds.size.y;

        _currentBuildPrefab.transform.position = new Vector3(ForwardPos.x, terrainY + objectHeight / 2, ForwardPos.z);

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

    private void ChangeBuild()
    {
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {

        }
        else if (Keyboard.current.xKey.wasPressedThisFrame)
        {

        }
    }

    private void PlaceBuild()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            RestoreMaterialColors();

            _currentBuildPrefab = Instantiate(BuildingsPrefabs[0], transform.position, BuildingsPrefabs[0].transform.rotation);

            SetPreviewMaterial(_currentBuildPrefab, Color.green, alpha);

            Debug.Log("Build was placed");
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

    private void OnEnable()
    {
        isBuilding = true;

        HelpKeysText.SetActive(true);

        _currentBuildPrefab = Instantiate(BuildingsPrefabs[2], transform.position, BuildingsPrefabs[2].transform.rotation);

        SetPreviewMaterial(_currentBuildPrefab, Color.green, alpha);

        Debug.Log("I take hammer");
    }

    private void OnDisable()
    {
        isBuilding = false;

        HelpKeysText.SetActive(false);

        Destroy(_currentBuildPrefab);

        Debug.Log("I hide the hammer");
    }
}
