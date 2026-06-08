using UnityEngine;

public class DichopticManager : MonoBehaviour
{
    void Start()
    {
        // Esperar un frame para que PerfilManager esté listo
        Invoke("ConfigurarOjos", 0.1f);
    }

    void ConfigurarOjos()
    {
        Camera camaraIzquierda = GameObject.Find("LeftEyeAnchor").GetComponent<Camera>();
        Camera camaraDerecha = GameObject.Find("RightEyeAnchor").GetComponent<Camera>();

        int layerDominante = LayerMask.NameToLayer("SoloOjoDominante");
        int layerDebil = LayerMask.NameToLayer("SoloOjoDebil");

        // Leer ojo dominante del perfil
        bool ojoDerecho = true;
        if (PerfilManager.Instance != null && PerfilManager.Instance.perfilActual != null)
        {
            ojoDerecho = PerfilManager.Instance.perfilActual.ojoDominanteDerecho;
            Debug.Log("✅ Ojo dominante leído del perfil: " + (ojoDerecho ? "Derecho" : "Izquierdo"));
        }
        else
        {
            Debug.LogWarning("⚠️ No hay perfil cargado, usando ojo derecho por defecto");
        }

        if (ojoDerecho)
        {
            camaraDerecha.cullingMask = (1 << layerDominante);
            camaraIzquierda.cullingMask = (1 << layerDebil);
        }
        else
        {
            camaraIzquierda.cullingMask = (1 << layerDominante);
            camaraDerecha.cullingMask = (1 << layerDebil);
        }
        if (layerDominante == -1 || layerDebil == -1)
        {
            Debug.LogError("❌ Layers no encontrados - verificar nombres exactos");
            return;
        }
    }
}