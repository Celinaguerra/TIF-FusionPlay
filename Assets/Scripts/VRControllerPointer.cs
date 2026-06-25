using UnityEngine;
using UnityEngine.EventSystems;

public class VRControllerPointer : MonoBehaviour, OVRInputModule.InputSource
{
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    public LineRenderer laser;
    public float laserLength = 4f;

    void Awake()
    {
        if (laser == null)
            laser = CrearLaser();
    }

    void OnEnable()
    {
        OVRInputModule.TrackInputSource(this);
    }

    void OnDisable()
    {
        OVRInputModule.UntrackInputSource(this);
    }

    LineRenderer CrearLaser()
    {
        GameObject laserObj = new GameObject("LaserUI");
        laserObj.transform.SetParent(transform, false);
        laserObj.transform.localPosition = Vector3.zero;
        laserObj.transform.localRotation = Quaternion.identity;

        LineRenderer line = laserObj.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = 0.004f;
        line.endWidth = 0.002f;
        line.useWorldSpace = false;
        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, Vector3.forward * laserLength);

        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = controller == OVRInput.Controller.LTouch
            ? new Color(0.2f, 0.7f, 1f, 0.9f)
            : new Color(1f, 0.4f, 0.2f, 0.9f);
        line.material = mat;
        line.startColor = mat.color;
        line.endColor = mat.color;
        return line;
    }

    public bool IsPressed()
    {
        return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller);
    }

    public bool IsReleased()
    {
        return OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controller);
    }

    public Transform GetPointerRayTransform()
    {
        return transform;
    }

    public bool IsValid()
    {
        return this != null;
    }

    public bool IsActive()
    {
        return OVRInput.IsControllerConnected(controller);
    }

    public OVRPlugin.Hand GetHand()
    {
        return controller == OVRInput.Controller.LTouch
            ? OVRPlugin.Hand.HandLeft
            : OVRPlugin.Hand.HandRight;
    }

    public void UpdatePointerRay(OVRInputRayData rayData)
    {
        if (laser == null)
            return;

        laser.enabled = IsActive();
        float length = rayData.IsOverCanvas ? rayData.DistanceToCanvas : laserLength;
        laser.SetPosition(1, Vector3.forward * length);

        Color baseColor = controller == OVRInput.Controller.LTouch
            ? new Color(0.2f, 0.7f, 1f, 0.9f)
            : new Color(1f, 0.4f, 0.2f, 0.9f);
        Color activeColor = Color.Lerp(baseColor, Color.white, rayData.ActivationStrength);
        laser.startColor = activeColor;
        laser.endColor = activeColor;
    }
}