using UnityEngine;

public class VRUIPositioner : MonoBehaviour
{
    public Vector3 offsetLocal = new Vector3(0f, -0.35f, 1.5f);

    void Start()
    {
        AplicarPosicion();
    }

    public void AplicarPosicion()
    {
        Transform ojo = GameObject.Find("CenterEyeAnchor")?.transform;
        if (ojo == null)
            return;

        transform.SetParent(ojo, false);
        transform.localPosition = offsetLocal;
        transform.localRotation = Quaternion.identity;
    }
}
