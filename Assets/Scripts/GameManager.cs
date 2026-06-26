using System;
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

    private int puntos;
    private float tiempoRestante;
    private bool sesionIniciada;
    private bool sesionPausada;
    private bool sesionFinalizada;
    private int nivelActual = 1;
    private int nivelMaximo = 1;
    private int aciertosConsecutivos;

    private int totalAciertos;
    private int totalErrores;
    private int puntosMaximos;

    private float[] velocidades = { 0.9f, 1.2f, 1.6f, 2.2f };
    private float[] tamanios = { 0.3f, 0.25f, 0.2f, 0.15f };

    private float bonusVelocidad;
    private const float incrementoPorAcierto = 0.08f;
    private const float velocidadMaxima = 3f;

    SesionUI sesionUI;

    void Awake()
    {
        Instance = this;
        sesionUI = GetComponent<SesionUI>();
        if (sesionUI == null)
            sesionUI = gameObject.AddComponent<SesionUI>();
    }

    void Start()
    {
        tiempoRestante = duracionSesion;
        sesionIniciada = false;
        sesionPausada = false;
        sesionFinalizada = false;
        ActualizarHUD();
    }

    public void ConfigurarUI(RectTransform hud, RectTransform panelPerfil)
    {
        if (sesionUI != null)
            sesionUI.Configurar(hud, panelPerfil);
    }

    public void IniciarSesion()
    {
        ReiniciarEstadisticasSesion();
        sesionIniciada = true;
        sesionPausada = false;
        sesionFinalizada = false;
        bonusVelocidad = 0f;

        if (CristalSpawner.Instance != null)
            CristalSpawner.Instance.IniciarSpawns();

        if (sesionUI != null)
            sesionUI.ActualizarUI();
    }

    void ReiniciarEstadisticasSesion()
    {
        puntos = 0;
        nivelActual = 1;
        nivelMaximo = 1;
        aciertosConsecutivos = 0;
        totalAciertos = 0;
        totalErrores = 0;
        puntosMaximos = 0;
        tiempoRestante = duracionSesion;
        bonusVelocidad = 0f;
        ActualizarHUD();
    }

    public bool GetSesionActiva()
    {
        return sesionIniciada && !sesionPausada && !sesionFinalizada && tiempoRestante > 0f;
    }

    public bool SesionEnCurso()
    {
        return sesionIniciada && !sesionFinalizada;
    }

    public bool EstaPausada()
    {
        return sesionPausada;
    }

    public void PausarSesion()
    {
        if (!sesionIniciada || sesionFinalizada)
            return;

        sesionPausada = true;

        if (CristalSpawner.Instance != null)
            CristalSpawner.Instance.DetenerSpawns();

        if (sesionUI != null)
            sesionUI.ActualizarUI();
    }

    public void ReanudarSesion()
    {
        if (!sesionIniciada || sesionFinalizada)
            return;

        sesionPausada = false;

        if (CristalSpawner.Instance != null)
            CristalSpawner.Instance.ReanudarSpawns();

        if (sesionUI != null)
            sesionUI.ActualizarUI();
    }

    void Update()
    {
        if (!GetSesionActiva())
            return;

        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0f)
            FinalizarSesion();

        ActualizarHUD();
    }

    void FinalizarSesion()
    {
        if (sesionFinalizada)
            return;

        tiempoRestante = 0f;
        sesionFinalizada = true;
        sesionPausada = false;
        sesionIniciada = false;

        if (CristalSpawner.Instance != null)
        {
            CristalSpawner.Instance.DetenerSpawns();
            CristalSpawner.Instance.LimpiarCristales();
        }

        GuardarRegistroSesion();

        if (sesionUI != null)
            sesionUI.AlTerminarSesion();

        PerfilUI perfilUI = FindFirstObjectByType<PerfilUI>();
        if (perfilUI != null)
            perfilUI.VolverAlMenuInicial();

        ActualizarHUD();
        Debug.Log("Sesion terminada, guardada y vuelta al menu.");
    }

    void GuardarRegistroSesion()
    {
        if (HistorialSesionesManager.Instance == null)
            return;

        string nombre = PerfilManager.Instance != null && PerfilManager.Instance.perfilActual != null
            ? PerfilManager.Instance.perfilActual.nombrePaciente
            : "Paciente";

        int intentos = totalAciertos + totalErrores;
        float precision = intentos > 0 ? (totalAciertos * 100f / intentos) : 0f;

        RegistroSesion registro = new RegistroSesion
        {
            fechaISO = DateTime.Now.ToString("o"),
            nombrePaciente = nombre,
            duracionSegundos = duracionSesion - tiempoRestante,
            aciertos = totalAciertos,
            errores = totalErrores,
            precisionPorcentaje = precision,
            nivelMaximo = nivelMaximo,
            puntosMaximos = puntosMaximos
        };

        HistorialSesionesManager.Instance.RegistrarSesion(registro);
    }

    public void RegistrarAcierto()
    {
        if (!GetSesionActiva())
            return;

        totalAciertos++;
        puntos++;
        aciertosConsecutivos++;
        puntosMaximos = Mathf.Max(puntosMaximos, puntos);
        bonusVelocidad = Mathf.Min(bonusVelocidad + incrementoPorAcierto, velocidadMaxima);
        VerificarSubidaNivel();
        ActualizarHUD();
    }

    public void RegistrarError()
    {
        if (!GetSesionActiva())
            return;

        totalErrores++;
        puntos--;
        aciertosConsecutivos = 0;
        puntosMaximos = Mathf.Max(puntosMaximos, puntos);
        ActualizarHUD();
    }

    void VerificarSubidaNivel()
    {
        if (aciertosConsecutivos >= aciertosParaSubirNivel && nivelActual < 4)
        {
            nivelActual++;
            nivelMaximo = Mathf.Max(nivelMaximo, nivelActual);
            aciertosConsecutivos = 0;
            Debug.Log("Nivel subido a: " + nivelActual);
        }
    }

    public float GetVelocidadActual()
    {
        return Mathf.Min(velocidades[nivelActual - 1] + bonusVelocidad, velocidadMaxima);
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
        if (textPuntos != null)
            textPuntos.text = "Puntos: " + puntos;
        if (textNivel != null)
            textNivel.text = "Nivel: " + nivelActual;

        if (textTiempo == null)
            return;

        if (sesionPausada)
        {
            textTiempo.text = "PAUSA";
            return;
        }

        if (sesionFinalizada)
        {
            textTiempo.text = "Tiempo: 0:00";
            return;
        }

        int minutos = Mathf.FloorToInt(tiempoRestante / 60f);
        int segundos = Mathf.FloorToInt(tiempoRestante % 60f);
        textTiempo.text = string.Format("Tiempo: {0}:{1:00}", minutos, segundos);
    }
}
