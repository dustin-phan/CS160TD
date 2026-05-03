using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Platform : MonoBehaviour
{
    public static event Action<Platform> OnPlatformClicked;
    public static event Action<Tower> OnTowerClicked;
    [SerializeField] private LayerMask platformLayerMask;
    public static bool towerPanelOpen { get; set; } = false;
    public TowerData towerType;
    private GameObject currentTower;

    private void Update()
    {
        if (towerPanelOpen || Time.timeScale == 0f)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D raycastHit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, platformLayerMask);

            if (raycastHit.collider != null)
            {
                Platform platform = raycastHit.collider.GetComponent<Platform>();
                if (platform != null)
                {
                    OnPlatformClicked?.Invoke(platform);
                }
            }
        }
        else if(Mouse.current.rightButton.wasPressedThisFrame)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D raycastHit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, platformLayerMask);

            if (raycastHit.collider != null)
            {
                Platform platform = raycastHit.collider.GetComponent<Platform>();
                if ((platform != null) && (currentTower != null))
                {
                    OnTowerClicked?.Invoke(currentTower.GetComponent<Tower>());
                }
            }
        }
    }

    public void PlaceTower(TowerData data)
    {
        if(towerType)
        {
            Destroy(currentTower);
        }
        towerType = data;
        currentTower = currentTower = Instantiate(
            data.prefab, 
            transform.position + new Vector3(0f, 0.5f, 0f), 
            Quaternion.identity, 
            transform
        );
    }

    public void DestroyTower()
    {
        if(towerType)
        {
            Destroy(currentTower);
            towerType = null;
        }
    }
}
