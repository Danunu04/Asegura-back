using BE;

namespace Servicios.Singleton;

public class SesionUsuario
{
    public BE_Usuario _usuario { get; set; }

        
    public BE_Usuario Usuario
    {
        get
        {
            return _usuario;
        }
    }

        
    public void _686DPLogIN(BE_Usuario usuario)
    {
        _usuario = usuario;
    }

        
    public void _686DPLogOut()
    {
        _usuario = null;
    }

        
    public bool _686DPIsLogged()
    {
        return _usuario != null;
    }
}