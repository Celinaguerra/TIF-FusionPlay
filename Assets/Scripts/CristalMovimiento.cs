using UnityEngine;

public class CristalMovimiento : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 1f;
    [Tooltip("Distancia al punto destino para frenar. Usar ~0.15, NO 2.")]
    public float distanciaDetencion = 0.15f;

    [Header("Aceleracion progresiva")]
    public float factorVelocidadMaxima = 1.8f;
    public float tiempoParaVelocidadMax = 5f;

    [Header("Colores")]
    public Color[] colores = { Color.red, Color.blue, Color.green };
    public float tiempoParaMostrarColor = 0.3f;

    [Header("Timer")]
    public float tiempoLimite = 8f;

    private bool detenido;
    private bool agarrado;
    private Vector3 puntoDestino;
    private Vector3 posicionDetenido;
    private Color colorAsignado;
    private float timerColor;
    private float timerLimite;
    private bool colorMostrado;
    private float tiempoViaje;
    private float velocidadBase;
    private float velocidadActual;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    public void ConfigurarTrayectoria(Vector3 destino)
    {
        puntoDestino = destino;
    }

    public void ConfigurarVelocidad(float baseVel)
    {
        velocidadBase = baseVel;
        velocidadActual = baseVel * 0.4f;
        velocidad = baseVel;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.EstaPausada())
            return;

        if (agarrado)
            return;

        if (!detenido)
        {
            tiempoViaje += Time.deltaTime;
            float progreso = Mathf.Clamp01(tiempoViaje / tiempoParaVelocidadMax);
            float velocidadMin = velocidadBase * 0.4f;
            float velocidadMax = velocidadBase * factorVelocidadMaxima;
            velocidadActual = Mathf.Lerp(velocidadMin, velocidadMax, progreso);

            transform.position = Vector3.MoveTowards(
                transform.position,
                puntoDestino,
                velocidadActual * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, puntoDestino) <= distanciaDetencion)
                DetenerYAsignarColor();
        }
        else if (!colorMostrado)
        {
            timerColor += Time.deltaTime;
            if (timerColor >= tiempoParaMostrarColor)
                MostrarColor();
        }
        else
        {
            timerLimite += Time.deltaTime;

            if (timerLimite >= tiempoLimite * 0.7f)
            {
                float parpadeo = Mathf.PingPong(Time.time * 5f, 1f);
                AplicarColorAlRenderer(Color.Lerp(colorAsignado, Color.white, parpadeo));
            }

            if (timerLimite >= tiempoLimite)
                Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (GameManager.Instance == null || !GameManager.Instance.GetSesionActiva())
            return;

        if (CristalSpawner.Instance != null)
            CristalSpawner.Instance.RegistrarCristalDestruido(this);
    }

    void DetenerYAsignarColor()
    {
        detenido = true;
        posicionDetenido = transform.position;
        transform.position = posicionDetenido;
        AsignarColor();

        if (tiempoParaMostrarColor <= 0f)
            MostrarColor();
    }

    void AsignarColor()
    {
        colorAsignado = colores[Random.Range(0, colores.Length)];
        AplicarColorAlRenderer(Color.white);
    }

    void MostrarColor()
    {
        colorMostrado = true;
        AplicarColorAlRenderer(colorAsignado);
    }

    void AplicarColorAlRenderer(Color color)
    {
        Material mat = rend.sharedMaterial;
        if (mat == null)
            return;

        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);
        else
            mat.color = color;
    }

    public Color GetColorAsignado()
    {
        return colorAsignado;
    }

    public bool ColorMostrado()
    {
        return colorMostrado;
    }

    public bool CanGrab()
    {
        return detenido && colorMostrado && !agarrado;
    }

    public void Grab()
    {
        if (!CanGrab())
            return;

        agarrado = true;
    }

    public void UpdateHoldPosition(Vector3 posicion)
    {
        if (!agarrado)
            return;

        transform.position = posicion;
    }

    public void Release()
    {
        agarrado = false;
        posicionDetenido = transform.position;
    }
}
