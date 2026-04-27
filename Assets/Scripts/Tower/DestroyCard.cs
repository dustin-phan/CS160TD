using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DestroyCard : MonoBehaviour
{
    public static event Action onDestroySelected;

    public void DestroyTower()
    {
        onDestroySelected?.Invoke();
    }
}
