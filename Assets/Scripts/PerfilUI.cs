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

    private bool ojoDerecho = true;
    private TextMeshProUGUI textoBotonOjo;

    void Start()
    {
        textoBotonOjo = botonOjoDominante.GetComponentInChildren<TextMeshProUGUI>();
        botonOjoDominante.onClick.AddListener(CambiarOjoDominante);
        botonGuardar.onClick.AddListener(GuardarYJugar);

        // Ocultar HUD hasta que empiece el juego
        hudJuego.SetActive(false);
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

        // Guardar perfil
        int edad = int.Parse(inputEdad.text);
        PerfilManager.Instance.GuardarPerfil(inputNombre.text, edad, ojoDerecho);
        PerfilManager.Instance.AplicarPerfilAlJuego();

        // Mostrar juego y ocultar perfil
        panelPerfil.SetActive(false);
        hudJuego.SetActive(true);
        GameManager.Instance.IniciarSesion();

        Debug.Log("✅ Perfil guardado, iniciando juego!");
    }
}