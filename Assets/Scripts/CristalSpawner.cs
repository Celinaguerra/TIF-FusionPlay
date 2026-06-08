using UnityEngine;

public class CristalSpawner : MonoBehaviour
{
    public GameObject cristalPrefab;
    public float tiempoEntreSpawns = 1.5f;
    private float timer;

    void Update()
    {
        if (!GameManager.Instance.GetSesionActiva()) return;

        timer += Time.deltaTime;

        if (timer >= tiempoEntreSpawns)
        {
            SpawnCristal();
            timer = 0f;
        }
    }

    void SpawnCristal()
    {
        Vector3 posicion = new Vector3(
            Random.Range(-1.5f, 1.5f),
            Random.Range(-1.5f, 1.5f),
            transform.position.z
        );

        GameObject nuevoCristal = Instantiate(cristalPrefab, posicion, Quaternion.identity);

        // Aplicar dificultad actual
        CristalMovimiento mov = nuevoCristal.GetComponent<CristalMovimiento>();
        if (mov != null)
        {
            mov.velocidad = GameManager.Instance.GetVelocidadActual();
            float tam = GameManager.Instance.GetTamanioActual();
            nuevoCristal.transform.localScale = new Vector3(tam, tam, tam);
        }
    }
}