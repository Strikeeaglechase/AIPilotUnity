// Unity Only File

using TMPro;
using UnityEngine;

public class HUDRWRIcon : MonoBehaviour
{
    public TextMeshProUGUI symbol;
    public GameObject lockIcon;
    public int actorId;
    public bool isStillUsed = true;

    public void SetSymbol(string text)
    {
        symbol.text = text;
    }

    public void SetLocked(bool locked)
    {
        lockIcon.SetActive(locked);
    }
}
