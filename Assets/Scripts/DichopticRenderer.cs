using UnityEngine;

[DisallowMultipleComponent]
public class DichopticRenderer : MonoBehaviour
{
    public enum MascaraOjo
    {
        Izquierdo = 1,
        Derecho = 2,
        Ambos = 3
    }

    static Shader shaderDichoptic;
    static Material plantillaMaterial;

    Renderer renderizador;
    Material materialOriginal;
    Material materialDichoptic;
    MascaraOjo mascaraActual = MascaraOjo.Ambos;
    bool usandoDichoptic;

    void Awake()
    {
        renderizador = GetComponent<Renderer>();
        if (renderizador != null)
            materialOriginal = renderizador.sharedMaterial;
    }

    public static void InicializarShader(Shader shader)
    {
        if (shader == null)
            return;

        shaderDichoptic = shader;
        if (plantillaMaterial == null)
            plantillaMaterial = new Material(shaderDichoptic);
    }

    public void AplicarMascara(MascaraOjo mascara)
    {
        if (renderizador == null)
            renderizador = GetComponent<Renderer>();

        if (renderizador == null)
            return;

        if (mascara == MascaraOjo.Ambos)
        {
            Restaurar();
            return;
        }

        if (!AsegurarShader() || !shaderDichoptic.isSupported)
        {
            Debug.LogWarning("[Dichoptic] Shader no soportado en " + gameObject.name);
            return;
        }

        if (materialDichoptic == null)
        {
            if (plantillaMaterial != null)
                materialDichoptic = new Material(plantillaMaterial);
            else
                materialDichoptic = new Material(shaderDichoptic);

            CopiarColorDesdeOriginal();
        }

        mascaraActual = mascara;
        materialDichoptic.SetInt("_EyeMask", (int)mascara);
        materialDichoptic.enableInstancing = true;
        renderizador.sharedMaterial = materialDichoptic;
        usandoDichoptic = true;
    }

    public void ReforzarMascara()
    {
        if (!usandoDichoptic || renderizador == null || materialDichoptic == null)
            return;

        if (renderizador.sharedMaterial != materialDichoptic)
            renderizador.sharedMaterial = materialDichoptic;

        materialDichoptic.SetInt("_EyeMask", (int)mascaraActual);
    }

    public void Restaurar()
    {
        if (renderizador == null || !usandoDichoptic)
            return;

        if (materialOriginal != null)
            renderizador.sharedMaterial = materialOriginal;

        mascaraActual = MascaraOjo.Ambos;
        usandoDichoptic = false;
    }

    bool AsegurarShader()
    {
        if (shaderDichoptic != null)
            return true;

        Material recurso = Resources.Load<Material>("DichopticMaterial");
        if (recurso != null)
        {
            shaderDichoptic = recurso.shader;
            plantillaMaterial = recurso;
            return true;
        }

        shaderDichoptic = Shader.Find("Fusionplay/DichopticUnlit");
        if (shaderDichoptic == null)
        {
            Debug.LogError("[Dichoptic] Shader no encontrado.");
            return false;
        }

        return true;
    }

    void CopiarColorDesdeOriginal()
    {
        Color color = Color.white;
        if (materialOriginal != null)
        {
            if (materialOriginal.HasProperty("_BaseColor"))
                color = materialOriginal.GetColor("_BaseColor");
            else if (materialOriginal.HasProperty("_Color"))
                color = materialOriginal.GetColor("_Color");
        }

        materialDichoptic.SetColor("_BaseColor", color);
    }

    void OnDestroy()
    {
        if (materialDichoptic != null)
            Destroy(materialDichoptic);
    }
}
