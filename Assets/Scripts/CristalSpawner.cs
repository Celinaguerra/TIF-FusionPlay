using UnityEngine;

public class CristalSpawner : MonoBehaviour
{
    public static CristalSpawner Instance;

    public GameObject cristalPrefab;

    [Header("Distancia al jugador")]
    [Tooltip("Metros frente a la cara donde se detiene el cristal (fijo al spawn).")]
    public float distanciaAlJugador = 1.2f;
    public float alturaDestino = 1f;

    [Header("Spawn en fondo del tunel")]
    public Transform referenciaTunel;

    [Header("Ritmo")]
    public float delayEntreCristales = 0.8f;

    CristalMovimiento cristalActivo;
    float timerEspera;
    bool spawnsActivos;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!spawnsActivos || !GameManager.Instance.GetSesionActiva())
            return;

        if (cristalActivo != null)
            return;

        if (timerEspera > 0f)
        {
            timerEspera -= Time.deltaTime;
            return;
        }

        SpawnCristal();
    }

    public void IniciarSpawns()
    {
        spawnsActivos = true;
        cristalActivo = null;
        timerEspera = 0f;
    }

    public void ReanudarSpawns()
    {
        spawnsActivos = true;
        SincronizarCristalActivo();
    }

    public void DetenerSpawns()
    {
        spawnsActivos = false;
    }

    public void LimpiarCristales()
    {
        spawnsActivos = false;
        cristalActivo = null;
        timerEspera = 0f;

        CristalMovimiento[] cristales = FindObjectsByType<CristalMovimiento>(FindObjectsSortMode.None);
        foreach (CristalMovimiento cristal in cristales)
            Destroy(cristal.gameObject);
    }

    void SincronizarCristalActivo()
    {
        if (cristalActivo != null)
            return;

        CristalMovimiento[] cristales = FindObjectsByType<CristalMovimiento>(FindObjectsSortMode.None);
        if (cristales.Length > 0)
            cristalActivo = cristales[0];
    }

    public void RegistrarCristalDestruido(CristalMovimiento cristal)
    {
        if (cristalActivo != cristal)
            return;

        cristalActivo = null;
        timerEspera = delayEntreCristales;
    }

    void SpawnCristal()
    {
        if (cristalPrefab == null || cristalActivo != null)
            return;

        Vector3 destino = CalcularDestinoFijo();
        Vector3 spawn = CalcularSpawnEnLinea(destino);

        GameObject nuevoCristal = Instantiate(cristalPrefab, spawn, Quaternion.identity);

        CristalMovimiento mov = nuevoCristal.GetComponent<CristalMovimiento>();
        if (mov != null)
        {
            mov.ConfigurarTrayectoria(destino);
            mov.ConfigurarVelocidad(GameManager.Instance.GetVelocidadActual());

            float tam = GameManager.Instance.GetTamanioActual();
            nuevoCristal.transform.localScale = new Vector3(tam, tam, tam);
            cristalActivo = mov;
        }

        if (DichopticManager.Instance != null)
            DichopticManager.Instance.AplicarMascaraDebil(nuevoCristal);
    }

    Vector3 CalcularDestinoFijo()
    {
        Transform ojo = GameObject.Find("CenterEyeAnchor")?.transform;
        if (ojo != null)
            return ojo.position + ojo.forward * distanciaAlJugador;

        return new Vector3(0f, alturaDestino, distanciaAlJugador);
    }

    Vector3 CalcularSpawnEnLinea(Vector3 destino)
    {
        Transform tunel = referenciaTunel != null
            ? referenciaTunel
            : GameObject.Find("Tunel")?.transform;

        if (tunel == null)
            return destino + Vector3.forward * 8f;

        CapsuleCollider capsule = tunel.GetComponent<CapsuleCollider>();
        float mitadLargo = capsule != null
            ? capsule.height * 0.5f * tunel.lossyScale.y
            : tunel.lossyScale.y;

        Vector3 fondoTunel = tunel.position + Vector3.forward * mitadLargo;
        return new Vector3(destino.x, destino.y, fondoTunel.z);
    }
}
