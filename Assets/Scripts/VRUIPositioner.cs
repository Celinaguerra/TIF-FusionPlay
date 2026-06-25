using UnityEngine;

public class VRUIPositioner : MonoBehaviour
{
    public Vector3 offsetLocal = new Vector3(0f, -0.35f, 1.5f);

    void OnEnable()
    {
        AplicarPosicion();
    }

    public void AplicarPosicion()
    {
        Transform ojo = BuscarCenterEyeAnchor();
        if (ojo == null)
            return;

        RectTransform rect = transform as RectTransform;
        Vector3 escala = rect != null ? rect.localScale : transform.localScale;

        transform.SetParent(ojo, false);
        transform.localRotation = Quaternion.identity;

        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localScale = escala;
            rect.anchoredPosition3D = offsetLocal;
            return;
        }

        transform.localScale = escala;
        transform.localPosition = offsetLocal;
    }

    static Transform BuscarCenterEyeAnchor()
    {
        GameObject ojo = GameObject.Find("CenterEyeAnchor");
        return ojo != null ? ojo.transform : null;
    }
}
