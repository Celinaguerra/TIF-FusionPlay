using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DichopticManager : MonoBehaviour
{
    public static DichopticManager Instance;

    [SerializeField] Shader shaderDichoptic;

    OVRCameraRig rig;
    bool dichopticActivo;
    bool ojoDerechoDominante = true;
    bool filtrandoRenderers;

    DichopticRenderer.MascaraOjo mascaraDominante;
    DichopticRenderer.MascaraOjo mascaraDebil;

    readonly List<Renderer> renderersDominantes = new List<Renderer>();
    readonly List<Renderer> renderersDebiles = new List<Renderer>();

    int capaDominante;
    int capaDebil;

    void Awake()
    {
        Instance = this;
        rig = GetComponent<OVRCameraRig>();
        if (rig == null)
            rig = FindAnyObjectByType<OVRCameraRig>();

        if (shaderDichoptic == null)
        {
            Material recurso = Resources.Load<Material>("DichopticMaterial");
            if (recurso != null)
                shaderDichoptic = recurso.shader;
        }

        if (shaderDichoptic == null)
            shaderDichoptic = Shader.Find("Fusionplay/DichopticUnlit");

        DichopticRenderer.InicializarShader(shaderDichoptic);

        capaDominante = LayerMask.NameToLayer("SoloOjoDominante");
        capaDebil = LayerMask.NameToLayer("SoloOjoDebil");
    }

    void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += AlIniciarCamara;
        RenderPipelineManager.endCameraRendering += AlTerminarCamara;
    }

    void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= AlIniciarCamara;
        RenderPipelineManager.endCameraRendering -= AlTerminarCamara;
    }

    void Start()
    {
        RestaurarVistaMenu();
    }

    void AlIniciarCamara(ScriptableRenderContext contexto, Camera camara)
    {
        if (!dichopticActivo || rig == null)
            return;

        if (!EsCamaraDeOjo(camara, out bool esIzquierdo))
            return;

        bool camaraDominante = ojoDerechoDominante ? !esIzquierdo : esIzquierdo;
        AplicarFiltroRenderers(camaraDominante);
        filtrandoRenderers = true;
    }

    void AlTerminarCamara(ScriptableRenderContext contexto, Camera camara)
    {
        if (!filtrandoRenderers)
            return;

        RestaurarTodosLosRenderers();
        filtrandoRenderers = false;
    }

    public void RestaurarVistaMenu()
    {
        dichopticActivo = false;
        RestaurarTodosLosRenderers();
        RestaurarShaders();
        ConfigurarCamarasCompletas();
        VRUISetup.RefrescarCanvasVR();
    }

    public void AplicarDichopticAlJuego()
    {
        StartCoroutine(AplicarConRetraso());
    }

    IEnumerator AplicarConRetraso()
    {
        yield return null;
        yield return null;
        ConfigurarOjos();
        VRUISetup.RefrescarCanvasVR();
    }

    void ConfigurarCamarasCompletas()
    {
        if (rig == null)
            rig = FindAnyObjectByType<OVRCameraRig>();

        if (rig == null)
            return;

        ConfigurarCamara(rig.leftEyeAnchor.GetComponent<Camera>(), StereoTargetEyeMask.Left);
        ConfigurarCamara(rig.rightEyeAnchor.GetComponent<Camera>(), StereoTargetEyeMask.Right);
        ConfigurarCamara(rig.centerEyeAnchor.GetComponent<Camera>(), StereoTargetEyeMask.Both);
    }

    void ConfigurarCamara(Camera camara, StereoTargetEyeMask ojo)
    {
        if (camara == null)
            return;

        camara.enabled = true;
        camara.cullingMask = -1;
        camara.stereoTargetEye = ojo;
    }

    public void ConfigurarOjos()
    {
        if (rig == null)
        {
            rig = FindAnyObjectByType<OVRCameraRig>();
            if (rig == null)
            {
                Debug.LogError("[Dichoptic] OVRCameraRig no encontrado.");
                return;
            }
        }

        ojoDerechoDominante = true;
        if (PerfilManager.Instance != null && PerfilManager.Instance.perfilActual != null)
            ojoDerechoDominante = PerfilManager.Instance.perfilActual.ojoDominanteDerecho;

        mascaraDominante = ojoDerechoDominante
            ? DichopticRenderer.MascaraOjo.Derecho
            : DichopticRenderer.MascaraOjo.Izquierdo;

        mascaraDebil = ojoDerechoDominante
            ? DichopticRenderer.MascaraOjo.Izquierdo
            : DichopticRenderer.MascaraOjo.Derecho;

        RegistrarRenderers();
        AplicarShadersDicoptic();
        ConfigurarCamarasCompletas();
        RestaurarTodosLosRenderers();

        dichopticActivo = true;

        Debug.Log("[Dichoptic] Filtro por ojo activo | Dominante="
            + mascaraDominante + " | Debil=" + mascaraDebil);
    }

    void RegistrarRenderers()
    {
        renderersDominantes.Clear();
        renderersDebiles.Clear();

        Renderer[] todos = FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Renderer renderer in todos)
        {
            int capa = renderer.gameObject.layer;
            if (capa == capaDominante)
                renderersDominantes.Add(renderer);
            else if (capa == capaDebil)
                renderersDebiles.Add(renderer);
        }
    }

    void AplicarFiltroRenderers(bool camaraDominante)
    {
        foreach (Renderer renderer in renderersDominantes)
        {
            if (renderer != null)
                renderer.enabled = camaraDominante;
        }

        foreach (Renderer renderer in renderersDebiles)
        {
            if (renderer != null)
                renderer.enabled = !camaraDominante;
        }
    }

    void RestaurarTodosLosRenderers()
    {
        foreach (Renderer renderer in renderersDominantes)
        {
            if (renderer != null)
                renderer.enabled = true;
        }

        foreach (Renderer renderer in renderersDebiles)
        {
            if (renderer != null)
                renderer.enabled = true;
        }
    }

    void AplicarShadersDicoptic()
    {
        foreach (Renderer renderer in renderersDominantes)
        {
            DichopticRenderer dic = ObtenerDichopticRenderer(renderer);
            dic.AplicarMascara(mascaraDominante);
        }

        foreach (Renderer renderer in renderersDebiles)
        {
            DichopticRenderer dic = ObtenerDichopticRenderer(renderer);
            dic.AplicarMascara(mascaraDebil);
        }
    }

    void RestaurarShaders()
    {
        DichopticRenderer[] todos = FindObjectsByType<DichopticRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (DichopticRenderer dic in todos)
            dic.Restaurar();
    }

    DichopticRenderer ObtenerDichopticRenderer(Renderer renderer)
    {
        DichopticRenderer dic = renderer.GetComponent<DichopticRenderer>();
        if (dic == null)
            dic = renderer.gameObject.AddComponent<DichopticRenderer>();
        return dic;
    }

    bool EsCamaraDeOjo(Camera camara, out bool esIzquierdo)
    {
        esIzquierdo = false;

        if (camara.transform == rig.leftEyeAnchor)
        {
            esIzquierdo = true;
            return true;
        }

        if (camara.transform == rig.rightEyeAnchor)
            return true;

        if (camara.stereoTargetEye == StereoTargetEyeMask.Left)
        {
            esIzquierdo = true;
            return true;
        }

        if (camara.stereoTargetEye == StereoTargetEyeMask.Right)
            return true;

        return false;
    }

    public void AplicarMascaraDebil(GameObject objeto)
    {
        if (capaDebil >= 0)
            objeto.layer = capaDebil;

        Renderer renderer = objeto.GetComponent<Renderer>();
        if (renderer != null && !renderersDebiles.Contains(renderer))
            renderersDebiles.Add(renderer);

        if (dichopticActivo)
        {
            DichopticRenderer dic = ObtenerDichopticRenderer(renderer);
            dic.AplicarMascara(mascaraDebil);
        }
    }
}
