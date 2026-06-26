using System;
using System.Collections.Generic;

[Serializable]
public class RegistroSesion
{
    public string fechaISO;
    public string nombrePaciente;
    public float duracionSegundos;
    public int aciertos;
    public int errores;
    public float precisionPorcentaje;
    public int nivelMaximo;
    public int puntosMaximos;

    public string FechaLegible()
    {
        if (DateTime.TryParse(fechaISO, out DateTime fecha))
            return fecha.ToString("dd/MM/yyyy HH:mm");
        return fechaISO;
    }

    public string DuracionLegible()
    {
        int min = (int)(duracionSegundos / 60f);
        int seg = (int)(duracionSegundos % 60f);
        return string.Format("{0}:{1:00}", min, seg);
    }
}

[Serializable]
public class HistorialGlobal
{
    public List<RegistroSesion> sesiones = new List<RegistroSesion>();
}
