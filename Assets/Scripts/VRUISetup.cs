using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class VRUISetup : MonoBehaviour
{
    [Header("Perfil")]
    public float escalaCanvasPerfil = 0.004f;
    public Vector3 offsetPerfil = new Vector3(0f, -0.05f, 1.5f);
    [Tooltip("Sube los campos del formulario en pixeles.")]
    public float offsetFormularioY = 70f;

    void Start()
    {
        ConfigurarEventSystem();
        ConfigurarCanvasPerfil();
        ConfigurarTeclado();
        ConfigurarCanvasHUD();
        ConfigurarPunteros();
    }

    void ConfigurarEventSystem()
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null)
            return;

        foreach (Behaviour modulo in eventSystem.GetComponents<Behaviour>())
        {
            if (modulo is InputSystemUIInputModule || modulo is StandaloneInputModule)
                modulo.enabled = false;
        }

        OVRInputModule ovrModulo = eventSystem.GetComponent<OVRInputModule>();
        if (ovrModulo == null)
            ovrModulo = eventSystem.gameObject.AddComponent<OVRInputModule>();

        ovrModulo.allowActivationOnMobileDevice = true;
        ovrModulo.enabled = true;
    }

    void ConfigurarCanvasPerfil()
    {
        Canvas canvas = GameObject.Find("CanvasPerfil")?.GetComponent<Canvas>();
        if (canvas == null)
            return;

        PrepararCanvas(canvas);

        RectTransform rect = canvas.GetComponent<RectTransform>();
        rect.localScale = Vector3.one * escalaCanvasPerfil;

        VRUIPositioner posicionador = canvas.GetComponent<VRUIPositioner>();
        if (posicionador == null)
            posicionador = canvas.gameObject.AddComponent<VRUIPositioner>();

        posicionador.offsetLocal = offsetPerfil;
        posicionador.AplicarPosicion();

        GameObject panel = GameObject.Find("PanelPerfil");
        if (panel != null)
            SubirCamposPerfil(panel.transform);
    }

    void SubirCamposPerfil(Transform panel)
    {
        string[] nombres = { "Titulo", "InputNombre", "InputEdad", "BotonOjoDominante", "BotonGuardar" };

        foreach (string nombre in nombres)
        {
            Transform hijo = panel.Find(nombre);
            if (hijo == null)
                continue;

            RectTransform rect = hijo.GetComponent<RectTransform>();
            if (rect != null)
                rect.anchoredPosition += new Vector2(0f, offsetFormularioY);
        }
    }

    void ConfigurarTeclado()
    {
        GameObject panel = GameObject.Find("PanelPerfil");
        if (panel == null)
            return;

        if (panel.GetComponent<TecladoVR>() == null)
            panel.AddComponent<TecladoVR>();
    }

    void ConfigurarCanvasHUD()
    {
        Canvas canvas = GameObject.Find("HUD")?.GetComponent<Canvas>();
        if (canvas == null)
            return;

        PrepararCanvas(canvas);
    }

    void PrepararCanvas(Canvas canvas)
    {
        AsignarCamaraCanvas(canvas);
        AsegurarLayerUI(canvas.gameObject);

        GraphicRaycaster grafico = canvas.GetComponent<GraphicRaycaster>();
        if (grafico != null)
            grafico.enabled = false;

        if (canvas.GetComponent<OVRRaycaster>() == null)
            canvas.gameObject.AddComponent<OVRRaycaster>();
    }

    public static void RefrescarCanvasVR()
    {
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Canvas canvas in canvases)
        {
            if (canvas.renderMode != RenderMode.WorldSpace)
                continue;

            canvas.worldCamera = null;
            AsegurarLayerUI(canvas.gameObject);
        }
    }

    static void AsignarCamaraCanvas(Canvas canvas)
    {
        canvas.worldCamera = null;
        AsegurarLayerUI(canvas.gameObject);
    }

    static void AsegurarLayerUI(GameObject objeto)
    {
        int layerUI = LayerMask.NameToLayer("UI");
        if (layerUI == -1)
            return;

        objeto.layer = layerUI;

        foreach (Transform hijo in objeto.transform)
            AsegurarLayerUI(hijo.gameObject);
    }

    void ConfigurarPunteros()
    {
        CrearPuntero("RightHandAnchor", OVRInput.Controller.RTouch);
        CrearPuntero("LeftHandAnchor", OVRInput.Controller.LTouch);
    }

    void CrearPuntero(string nombreAncla, OVRInput.Controller controller)
    {
        Transform ancla = BuscarHijo(transform, nombreAncla);
        if (ancla == null)
        {
            Debug.LogWarning("[VR] No se encontro " + nombreAncla);
            return;
        }

        if (ancla.GetComponent<VRControllerPointer>() != null)
            return;

        VRControllerPointer puntero = ancla.gameObject.AddComponent<VRControllerPointer>();
        puntero.controller = controller;

        if (ancla.Find("ControllerVisual") == null)
        {
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.name = "ControllerVisual";
            visual.transform.SetParent(ancla, false);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = Vector3.one * 0.08f;
            Destroy(visual.GetComponent<Collider>());
        }
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