// Unity Only File

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HUDRWR : MonoBehaviour
{
    public RWR rwr;
    public GameObject rwrIconPrefab;

    private List<HUDRWRIcon> icons = new List<HUDRWRIcon>();

    public float rwrScale = 50_000f;

    void Update()
    {
        foreach (var icon in icons)
        {
            icon.isStillUsed = false;
        }

        foreach (var contact in rwr.contacts)
        {
            var icon = GetIconForContact(contact.actorId);
            if (icon == null)
            {
                var obj = Instantiate(rwrIconPrefab, transform);

                var iconScript = obj.GetComponent<HUDRWRIcon>();
                iconScript.actorId = contact.actorId;

                UpdateIcon(iconScript, contact);

                icons.Add(iconScript);
            }
            else
            {
                UpdateIcon(icon, contact);
            }
        }

        foreach (var icon in icons)
        {
            if (!icon.isStillUsed) Destroy(icon.gameObject);
        }

        icons = icons.Where(icon => icon != null && icon.isStillUsed).ToList();
    }

    private void UpdateIcon(HUDRWRIcon icon, RWRContact contact)
    {

        icon.isStillUsed = true;

        //var relPos = (contact.position - rwr.transform.position) / rwrScale;

        var relPos = rwr.transform.InverseTransformPoint(contact.position) / rwrScale;

        //var relPos = new Vector3(0, 0, 0);

        ((RectTransform)icon.transform).anchoredPosition = new Vector2(
            relPos.x * ((RectTransform)transform).rect.width,
            relPos.z * ((RectTransform)transform).rect.height
            );
    }

    private HUDRWRIcon GetIconForContact(int actorId)
    {
        return icons.Find(icon => icon.actorId == actorId);
    }
}
