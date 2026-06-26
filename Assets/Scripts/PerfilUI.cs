using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PerfilUI : MonoBehaviour
{
    [Header("Campos")]
    public TMP_InputField inputNombre;
    public TMP_InputField inputEdad;

    [Header("Botones")]
    public Button botonOjoDominante;
    public Button botonGuardar;

    [Header("Paneles")]
    public GameObject panelPerfil;
    public GameObject hudJuego;

    Contenedor[] contenedoresJuego;

    private bool ojoDerecho = true;
    private TextMeshProUGUI textoBotonOjo;

    void Start()
    {
        textoBotonOjo = botonOjoDominante.GetComponentInChildren<TextMeshProUGUI>();
        botonOjoDominante.onClick.AddListener(CambiarOjoDominante);
        botonGuardar.onClick.AddListener(GuardarYJugar);

        hudJuego.SetActive(false);
        OcultarContenedores();

        RectTransform hudRect = hudJuego.GetComponent<RectTransform>();
        RectTransform panelRect = panelPerfil.GetComponent<RectTransform>();
        if (GameManager.Instance != null)
            GameManager.Instance.ConfigurarUI(hudRect, panelRect);
    }

    void OcultarContenedores()
    {
        contenedoresJuego = FindObjectsByType<Contenedor>(FindObjectsSortMode.None);
        foreach (Contenedor contenedor in contenedoresJuego)
            contenedor.gameObject.SetActive(false);
    }

    void MostrarContenedores()
    {
        if (contenedoresJuego == null)
            contenedoresJuego = FindObjectsByType<Contenedor>(FindObjectsSortMode.None);

        foreach (Contenedor contenedor in contenedoresJuego)
            contenedor.gameObject.SetActive(true);
    }

    void CambiarOjoDominante()
    {
        ojoDerecho = !ojoDerecho;
        textoBotonOjo.text = "Ojo Dominante: " + (ojoDerecho ? "Derecho" : "Izquierdo");
    }

    void GuardarYJugar()
    {
        // Validar campos
        if (string.IsNullOrEmpty(inputNombre.text))
        {
            Debug.LogWarning("⚠️ Ingresá el nombre del paciente");
            return;
        }

        if (string.IsNullOrEmpty(inputEdad.text))
        {
            Debug.LogWarning("⚠️ Ingresá la edad del paciente");
            return;
        }

        int edad = int.Parse(inputEdad.text);
        PerfilManager.Instance.GuardarPerfil(inputNombre.text, edad, ojoDerecho);

        panelPerfil.SetActive(false);
        hudJuego.SetActive(true);
        MostrarContenedores();

        RectTransform hudRect = hudJuego.GetComponent<RectTransform>();
        RectTransform panelRect = panelPerfil.GetComponent<RectTransform>();

        VRUISetup setup = FindAnyObjectByType<VRUISetup>();
        if (setup != null)
        {
            setup.ReaplicarHUD();
            setup.PrepararHUDInteractivo();
        }

        VRUISetup.RefrescarCanvasVR();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ConfigurarUI(hudRect, panelRect);
        }
        PerfilManager.Instance.AplicarPerfilAlJuego();
        GameManager.Instance.IniciarSesion();

        Debug.Log("✅ Perfil guardado, iniciando juego!");
    }

    public void VolverAlMenuInicial()
    {
        if (hudJuego != null)
            hudJuego.SetActive(false);

        if (panelPerfil != null)
            panelPerfil.SetActive(true);

        OcultarContenedores();

        DichopticManager dichoptic = FindFirstObjectByType<DichopticManager>();
        if (dichoptic != null)
            dichoptic.RestaurarVistaMenu();

        SesionUI sesionUI = FindFirstObjectByType<SesionUI>();
        if (sesionUI != null)
            sesionUI.RestaurarMenuPerfil();

        VRUISetup setup = FindAnyObjectByType<VRUISetup>();
        if (setup != null)
            setup.RestaurarMenuPerfil();
        else
            VRUISetup.RefrescarCanvasVR();

        VRHeadTrackingSetup headSetup = FindAnyObjectByType<VRHeadTrackingSetup>();
        if (headSetup != null)
            headSetup.ReaplicarPosicionSpawn();
    }
}