using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HammerController : MonoBehaviour
{
    [SerializeField] private Camera Camera;
    [SerializeField] private GameObject[] BuildingsPrefabs;
    [SerializeField] private Terrain Terrain;

    private GameObject _currentBuildPrefab;

    private Dictionary<Material, Color> _originalColors = new Dictionary<Material, Color>();

    private float buildDistance = 3f;

    private float alpha = 0.5f;

    public bool isBuilding = false;

    private void Update()
    {
        if (!isBuilding || _currentBuildPrefab == null)
        {
            return;
        }

        if (isBuilding)
        {
            Vector3 ForwardPos = Camera.transform.position + Camera.transform.forward * buildDistance;

            float terrainY = Terrain.SampleHeight(ForwardPos);

            Vector3 offset = Camera.transform.right * 2f + Camera.transform.forward * 2f;

            _currentBuildPrefab.transform.position = new Vector3(ForwardPos.x + offset.x, terrainY, ForwardPos.z + offset.z);
            _currentBuildPrefab.transform.rotation = Quaternion.Euler(0f, Camera.transform.eulerAngles.y, 0f);

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                RestoreMaterialColors();

                _currentBuildPrefab = Instantiate(BuildingsPrefabs[0], transform.position, BuildingsPrefabs[0].transform.rotation);

                SetPreviewMaterial(_currentBuildPrefab, Color.green, alpha);

                Debug.Log("Build was placed");
            }
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
                        _originalColors[mat] = mat.GetColor("_BaseColor");
                    else if (mat.HasProperty("_Color"))
                        _originalColors[mat] = mat.GetColor("_Color");
                }

                if (mat.HasProperty("_Surface"))
                {
                    mat.SetFloat("_Surface", 1f); // 0 = Opaque, 1 = Transparent
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                    // These two keywords are critical
                    mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    mat.DisableKeyword("_SURFACE_TYPE_OPAQUE");

                    // These control blending — must be set manually in URP
                    mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetFloat("_ZWrite", 0.0f);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                }

                // Set color with alpha
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
            original.a = 1f; // ensure full opacity

            if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", original);
            }                
            else if (mat.HasProperty("_Color"))
            {
                mat.SetColor("_Color", original);
            }

            mat.SetFloat("_Surface", 0f); // 0 = Opaque
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            mat.EnableKeyword("_SURFACE_TYPE_OPAQUE");
            mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.SetFloat("_ZWrite", 1f);
            mat.DisableKeyword("_ALPHABLEND_ON");
        }

        _originalColors.Clear(); // optional, if you only need it once
    }

    private void OnEnable()
    {
        isBuilding = true;

        _currentBuildPrefab = Instantiate(BuildingsPrefabs[0], transform.position, BuildingsPrefabs[0].transform.rotation);

        SetPreviewMaterial(_currentBuildPrefab, Color.green, alpha);

        Debug.Log("I take hammer");
    }

    private void OnDisable()
    {
        isBuilding = false;
        
        Destroy(_currentBuildPrefab);

        Debug.Log("I hide the hammer");
    }
}
