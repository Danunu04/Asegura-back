namespace BE;

public class BE_Usuario
{
    public int DP686_DNI { get; set; }
    public string DP686_Nombre { get; set; }
    public string DP686_Apellido { get; set; }
    public string DP686_Email { get; set; }
    public int Empresa { get; set; }
    public int DP686_Rol { get; set; }
    public string DP686_Usuario { get; set; }
    public string DP686_Contraseña { get; set; }
    public bool DP686_Activo { get; set; }
    public bool DP686_Bloqueado { get; set; }
    public bool DP686_CambiarContraseña { get; set; }
    
    public string DP686_Idioma { get; set; }
    
    public BE_Usuario CrearUsuario(int dni, string nombre, string apellido, string email, int rol, string usuario, string contraseña, bool activo, bool bloqueado, bool cambiarcontra, int empresa)
    {
        
        BE_Usuario nuevoUsuario = new BE_Usuario();

        nuevoUsuario.DP686_DNI = dni;
        nuevoUsuario.DP686_Nombre = nombre;
        nuevoUsuario.DP686_Apellido = apellido;
        nuevoUsuario.DP686_Email = email;
        nuevoUsuario.DP686_Rol = rol;
        nuevoUsuario.DP686_Usuario = usuario;
        nuevoUsuario.DP686_Contraseña = contraseña;
        nuevoUsuario.DP686_Activo = activo;
        nuevoUsuario.DP686_Bloqueado = bloqueado;
        nuevoUsuario.DP686_CambiarContraseña = cambiarcontra;
        nuevoUsuario.DP686_Idioma = "Es";
        nuevoUsuario.Empresa = empresa;

        return nuevoUsuario;
    }

    public BE_Usuario Usr_Login(string usuario, string contrasena, string idioma, bool activo, bool bloqueado, int empresa)
    {
        BE_Usuario usrLogin = new();
        usrLogin.DP686_Usuario = usuario;
        usrLogin.DP686_Contraseña = contrasena;
        usrLogin.DP686_Bloqueado = bloqueado;
        usrLogin.DP686_Activo = activo;
        usrLogin.Empresa = empresa;
        
        return usrLogin;
    }

    public BE_Usuario usrIngreso(string usuario, string contrasena)
    {
        BE_Usuario usrIngreso = new();
        usrIngreso.DP686_Usuario = usuario;
        usrIngreso.DP686_Contraseña = contrasena;
        return usrIngreso;
    }
}