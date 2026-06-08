using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("HUD")]
    public TextMeshProUGUI textPuntos;
    public TextMeshProUGUI textTiempo;
    public TextMeshProUGUI textNivel;

    [Header("Sesion")]
    public float duracionSesion = 300f;

    [Header("Dificultad")]
    public int aciertosParaSubirNivel = 3;

    private int puntos = 0;
    private float tiempoRestante;
    private bool sesionActiva = true;
    private int nivelActual = 1;
    private int aciertosConsecutivos = 0;

    // Configuracion por nivel
    private float[] velocidades = { 3f, 4.5f, 6f, 8f };
    private float[] tamanios = { 0.3f, 0.25f, 0.2f, 0.15f };

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        tiempoRestante = duracionSesion;
        sesionActiva = false; // Empieza pausado
        ActualizarHUD();
    }

    public void IniciarSesion()
    {
        sesionActiva = true;
    }

    public bool GetSesionActiva()
    {
        return sesionActiva;
    }

    void Update()
    {
        if (!sesionActiva) return;

        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            sesionActiva = false;
            Debug.Log("Sesion terminada!");
        }

        ActualizarHUD();
    }

    public void SumarPunto()
    {
        puntos++;
        aciertosConsecutivos++;
        VerificarSubidaNivel();
        ActualizarHUD();
    }

    public void RestarPunto()
    {
        puntos--;
        aciertosConsecutivos = 0; // Resetea racha
        ActualizarHUD();
    }

    void VerificarSubidaNivel()
    {
        if (aciertosConsecutivos >= aciertosParaSubirNivel && nivelActual < 4)
        {
            nivelActual++;
            aciertosConsecutivos = 0;
            Debug.Log("⬆️ Nivel subido a: " + nivelActual);
        }
    }

    public float GetVelocidadActual()
    {
        return velocidades[nivelActual - 1];
    }

    public float GetTamanioActual()
    {
        return tamanios[nivelActual - 1];
    }

    public int GetNivelActual()
    {
        return nivelActual;
    }

    void ActualizarHUD()
    {
        textPuntos.text = "Puntos: " + puntos;
        textNivel.text = "Nivel: " + nivelActual;

        int minutos = Mathf.FloorToInt(tiempoRestante / 60f);
        int segundos = Mathf.FloorToInt(tiempoRestante % 60f);
        textTiempo.text = string.Format("Tiempo: {0}:{1:00}", minutos, segundos);
    }
}