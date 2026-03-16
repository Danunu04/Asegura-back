namespace Servicios.Singleton;

public class Singleton
{
    private static SesionUsuario _instancia;
    private static readonly object _lock = new object();

    public static SesionUsuario Instancia
    {
        get
        {
            lock (_lock)
            {
                if (_instancia == null)
                { _instancia = new SesionUsuario(); }
            }
            return _instancia;
        }
    }
}