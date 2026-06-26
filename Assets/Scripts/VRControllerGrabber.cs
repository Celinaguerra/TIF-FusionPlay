using UnityEngine;

public class VRControllerGrabber : MonoBehaviour
{
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    public float radioAgarre = 0.5f;
    public float alcanceRayo = 0.6f;

    [Header("Distancia al agarrar")]
    [Tooltip("Distancia inicial del cristal respecto al mando (no pegado a la cara).")]
    public float distanciaInicial = 0.7f;
    public float distanciaMinima = 0.35f;
    public float distanciaMaxima = 2.5f;
    [Tooltip("Velocidad con la que el thumbstick aleja/acerca el cristal.")]
    public float velocidadAlejar = 3.5f;

    CristalMovimiento cristalAgarrado;
    float distanciaActual;

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.GetSesionActiva())
            return;

        bool mantieneAgarre = BotonAgarrePresionado();

        if (cristalAgarrado != null)
        {
            if (!mantieneAgarre)
            {
                cristalAgarrado.Release();
                cristalAgarrado = null;
            }
            else
            {
                ActualizarPosicionAgarre();
            }
            return;
        }

        if (!BotonAgarreIniciado())
            return;

        CristalMovimiento cristal = BuscarCristalCercano();
        if (cristal != null)
            IniciarAgarre(cristal);
    }

    void IniciarAgarre(CristalMovimiento cristal)
    {
        Vector3 alCristal = cristal.transform.position - transform.position;
        float distanciaProyectada = Vector3.Dot(alCristal, transform.forward);

        distanciaActual = distanciaProyectada >= distanciaMinima
            ? distanciaProyectada
            : distanciaInicial;

        distanciaActual = Mathf.Clamp(distanciaActual, distanciaMinima, distanciaMaxima);

        cristal.Grab();
        cristalAgarrado = cristal;
        ActualizarPosicionAgarre();
    }

    void ActualizarPosicionAgarre()
    {
        Vector2 stick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller);
        distanciaActual += stick.y * velocidadAlejar * Time.deltaTime;
        distanciaActual = Mathf.Clamp(distanciaActual, distanciaMinima, distanciaMaxima);

        Vector3 posicion = transform.position + transform.forward * distanciaActual;
        cristalAgarrado.UpdateHoldPosition(posicion);
    }

    bool BotonAgarrePresionado()
    {
        return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller)
            || OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, controller);
    }

    bool BotonAgarreIniciado()
    {
        return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller)
            || OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller);
    }

    CristalMovimiento BuscarCristalCercano()
    {
        CristalMovimiento mejor = null;
        float mejorDistancia = float.MaxValue;

        Collider[] hits = Physics.OverlapSphere(transform.position, radioAgarre);
        foreach (Collider hit in hits)
        {
            CristalMovimiento cristal = hit.GetComponent<CristalMovimiento>();
            if (cristal == null || !cristal.CanGrab())
                continue;

            float distancia = Vector3.Distance(transform.position, hit.transform.position);
            if (distancia < mejorDistancia)
            {
                mejorDistancia = distancia;
                mejor = cristal;
            }
        }

        if (mejor != null)
            return mejor;

        RaycastHit[] rayHits = Physics.SphereCastAll(
            transform.position,
            radioAgarre * 0.5f,
            transform.forward,
            alcanceRayo
        );

        foreach (RaycastHit hit in rayHits)
        {
            CristalMovimiento cristal = hit.collider.GetComponent<CristalMovimiento>();
            if (cristal == null || !cristal.CanGrab())
                continue;

            if (hit.distance < mejorDistancia)
            {
                mejorDistancia = hit.distance;
                mejor = cristal;
            }
        }

        return mejor;
    }
}
