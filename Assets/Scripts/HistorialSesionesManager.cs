using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class HistorialSesionesManager : MonoBehaviour
{
    public static HistorialSesionesManager Instance;

    const string ArchivoHistorial = "historial_sesiones.json";

    HistorialGlobal historial = new HistorialGlobal();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Cargar();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegistrarSesion(RegistroSesion registro)
    {
        if (registro == null)
            return;

        historial.sesiones.Add(registro);
        Guardar();
        Debug.Log("[Historial] Sesion guardada: " + registro.FechaLegible()
            + " | Precision " + registro.precisionPorcentaje.ToString("F0") + "%");
    }

    public List<RegistroSesion> ObtenerSesionesPaciente(string nombrePaciente)
    {
        if (string.IsNullOrEmpty(nombrePaciente))
            return new List<RegistroSesion>();

        return historial.sesiones
            .Where(s => s.nombrePaciente == nombrePaciente)
            .OrderByDescending(s => s.fechaISO)
            .ToList();
    }

    public string FormatearHistorial(string nombrePaciente)
    {
        List<RegistroSesion> sesiones = ObtenerSesionesPaciente(nombrePaciente);
        if (sesiones.Count == 0)
            return "Sin sesiones registradas para este paciente.";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("HISTORIAL DE SESIONES");
        sb.AppendLine("Paciente: " + nombrePaciente);
        sb.AppendLine("Total: " + sesiones.Count + " sesiones");
        sb.AppendLine();

        foreach (RegistroSesion s in sesiones)
        {
            sb.AppendLine(s.FechaLegible());
            sb.AppendLine("  Duracion: " + s.DuracionLegible()
                + " | Precision: " + s.precisionPorcentaje.ToString("F0") + "%");
            sb.AppendLine("  Aciertos: " + s.aciertos
                + " | Errores: " + s.errores
                + " | Nivel max: " + s.nivelMaximo
                + " | Puntos max: " + s.puntosMaximos);
            sb.AppendLine();
        }

        return sb.ToString();
    }

    void Cargar()
    {
        string ruta = ObtenerRutaArchivo();
        if (!File.Exists(ruta))
            return;

        try
        {
            string json = File.ReadAllText(ruta);
            HistorialGlobal cargado = JsonUtility.FromJson<HistorialGlobal>(json);
            if (cargado != null && cargado.sesiones != null)
                historial = cargado;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[Historial] No se pudo cargar: " + e.Message);
        }
    }

    void Guardar()
    {
        try
        {
            string json = JsonUtility.ToJson(historial, true);
            File.WriteAllText(ObtenerRutaArchivo(), json);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[Historial] No se pudo guardar: " + e.Message);
        }
    }

    static string ObtenerRutaArchivo()
    {
        return Path.Combine(Application.persistentDataPath, ArchivoHistorial);
    }
}
