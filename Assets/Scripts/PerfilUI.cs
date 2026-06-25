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

        VRUISetup setup = FindAnyObjectByType<VRUISetup>();
        if (setup != null)
            setup.ReaplicarHUD();

        VRUISetup.RefrescarCanvasVR();
        PerfilManager.Instance.AplicarPerfilAlJuego();
        GameManager.Instance.IniciarSesion();

        Debug.Log("✅ Perfil guardado, iniciando juego!");
    }
}