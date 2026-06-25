using UnityEngine;

public class VRGrabSetup : MonoBehaviour
{
    void Awake()
    {
        AgregarGrabber(OVRInput.Controller.LTouch,
            "LeftHandOnControllerAnchor",
            "LeftControllerInHandAnchor",
            "LeftHandAnchor");

        AgregarGrabber(OVRInput.Controller.RTouch,
            "RightHandOnControllerAnchor",
            "RightControllerInHandAnchor",
            "RightHandAnchor");
    }

    void AgregarGrabber(OVRInput.Controller controller, params string[] nombresAncla)
    {
        Transform ancla = null;
        foreach (string nombre in nombresAncla)
        {
            ancla = BuscarHijo(transform, nombre);
            if (ancla != null)
                break;
        }

        if (ancla == null)
        {
            Debug.LogWarning("[VR] No se encontro ancla de mano para " + controller);
            return;
        }

        if (ancla.GetComponent<VRControllerGrabber>() != null)
            return;

        VRControllerGrabber grabber = ancla.gameObject.AddComponent<VRControllerGrabber>();
        grabber.controller = controller;
    }

    Transform BuscarHijo(Transform padre, string nombre)
    {
        if (padre.name == nombre)
            return padre;

        foreach (Transform hijo in padre)
        {
            Transform encontrado = BuscarHijo(hijo, nombre);
            if (encontrado != null)
                return encontrado;
        }

        return null;
    }
}
