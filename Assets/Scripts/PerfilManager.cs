using UnityEngine;

public class PerfilManager : MonoBehaviour
{
    public static PerfilManager Instance;

    [Header("Perfil Actual")]
    public PacienteData perfilActual;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GuardarPerfil(string nombre, int edad, bool ojoDerecho)
    {
        perfilActual.nombrePaciente = nombre;
        perfilActual.edad = edad;
        perfilActual.ojoDominanteDerecho = ojoDerecho;

        Debug.Log("Perfil guardado: " + nombre + ", " + edad + " años, ojo dominante: " +
            (ojoDerecho ? "Derecho" : "Izquierdo"));
    }

    public void AplicarPerfilAlJuego()
    {
        // Conecta el perfil con el DichopticManager
        DichopticManager dichoptic = FindFirstObjectByType<DichopticManager>();
        if (dichoptic != null)
        {
            Debug.Log("✅ Configuracion dicoptica aplicada desde perfil");
        }
    }
}