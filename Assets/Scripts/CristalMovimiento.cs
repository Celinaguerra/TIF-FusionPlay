using UnityEngine;

public class CristalMovimiento : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;
    public float distanciaDetencion = 2f;

    [Header("Vibracion")]
    public float intensidadVibracion = 0.05f;
    public float frecuenciaVibracion = 10f;

    [Header("Colores")]
    public Color[] colores = { Color.red, Color.blue, Color.green };
    public float tiempoParaMostrarColor = 1f;

    [Header("Timer")]
    public float tiempoLimite = 5f;

    private bool detenido = false;
    private Vector3 posicionDetenido;
    private Color colorAsignado;
    private float timerColor = 0f;
    private float timerLimite = 0f;
    private bool colorMostrado = false;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        if (!detenido)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                Vector3.zero,
                velocidad * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, Vector3.zero) <= distanciaDetencion)
            {
                detenido = true;
                posicionDetenido = transform.position;
                AsignarColor();
            }
        }
        else
        {
            Vibrar();

            if (!colorMostrado)
            {
                timerColor += Time.deltaTime;
                if (timerColor >= tiempoParaMostrarColor)
                {
                    MostrarColor();
                }
            }
            else
            {
                // Contar tiempo limite
                timerLimite += Time.deltaTime;

                // Efecto visual: el cristal parpadea cuando queda poco tiempo
                if (timerLimite >= tiempoLimite * 0.7f)
                {
                    float parpadeo = Mathf.PingPong(Time.time * 5f, 1f);
                    rend.material.color = Color.Lerp(colorAsignado, Color.white, parpadeo);
                }

                if (timerLimite >= tiempoLimite)
                {
                    Debug.Log("⏱️ Tiempo agotado - Error!");
                    Destroy(gameObject);
                }
            }
        }
    }

    void Vibrar()
    {
        float offsetX = (Mathf.PerlinNoise(Time.time * frecuenciaVibracion, 0f) - 0.5f) * 2f * intensidadVibracion;
        float offsetY = (Mathf.PerlinNoise(0f, Time.time * frecuenciaVibracion) - 0.5f) * 2f * intensidadVibracion;
        transform.position = posicionDetenido + new Vector3(offsetX, offsetY, 0);
    }

    void AsignarColor()
    {
        colorAsignado = colores[Random.Range(0, colores.Length)];
        rend.material.color = Color.white;
    }

    void MostrarColor()
    {
        colorMostrado = true;
        rend.material.color = colorAsignado;
    }

    public Color GetColorAsignado()
    {
        return colorAsignado;
    }

    public bool ColorMostrado()
    {
        return colorMostrado;
    }
}