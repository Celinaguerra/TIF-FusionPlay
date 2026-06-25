using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

[DefaultExecutionOrder(-100)]
public class VRHeadTrackingSetup : MonoBehaviour
{
    [Header("Posicion del jugador (spawn)")]
    [Tooltip("Desplaza el rig al iniciar. Y=arriba, Z=adelante.")]
    public Vector3 offsetPosicionJugador = new Vector3(0f, 0.15f, 0.25f);

    void Awake()
    {
        ConfigurarStereoOculus();
        FijarPosicionInicialRig();
        DisableStaticCameras();
        EnsureXRRunning();
    }

    void ConfigurarStereoOculus()
    {
        OVRManager manager = GetComponent<OVRManager>();
        if (manager == null)
            manager = FindAnyObjectByType<OVRManager>();

        if (manager == null)
            return;

        // En Meta XR SDK 201 el render mode se configura en OpenXR (Project Settings).
        manager.trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;
    }

    void FijarPosicionInicialRig()
    {
        transform.SetPositionAndRotation(offsetPosicionJugador, Quaternion.identity);
    }

    void DisableStaticCameras()
    {
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);

        foreach (Camera cam in cameras)
        {
            if (cam.transform.root.name == "OVRCameraRig")
                continue;

            cam.enabled = false;

            if (cam.CompareTag("MainCamera"))
                cam.tag = "Untagged";
        }
    }

    void EnsureXRRunning()
    {
        XRGeneralSettings settings = XRGeneralSettings.Instance;
        if (settings == null || settings.Manager == null)
        {
            Debug.LogError("[VR] XRGeneralSettings no encontrado.");
            return;
        }

        XRManagerSettings manager = settings.Manager;

        if (!manager.isInitializationComplete)
            manager.InitializeLoaderSync();

        if (manager.activeLoader == null)
        {
            Debug.LogError("[VR] OpenXR no inició. Revisá XR Plug-in Management en Android.");
            return;
        }

        if (!XRSettings.enabled)
            manager.StartSubsystems();
    }
}
