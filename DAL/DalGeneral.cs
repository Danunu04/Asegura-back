using System.Collections;
using System.Data;
using Microsoft.Data.Sqlite;

namespace DAL;
/// <summary>
/// Clase general de acceso a datos para SQLite.
/// Thread-safe mediante instancias por request (no singleton).
/// Multitenant: todas las operaciones reciben tenant_id como parámetro obligatorio.
/// </summary>
public class DalGeneral
{
    
    // ------------------------------------------------------------------
    // Campos privados
    // ------------------------------------------------------------------

    /// <summary>Cadena de conexión construida desde variables de entorno.</summary>
    private readonly string _cadena_conexion;

    // ------------------------------------------------------------------
    // Constructor
    // ------------------------------------------------------------------

    /// <summary>
    /// Inicializa el DAL leyendo la ruta de la base de datos desde la variable
    /// de entorno DB_PATH. Si no está definida, usa "aseguraya.db" en el
    /// directorio de trabajo actual.
    /// </summary>
    public DalGeneral()
    {
        string db_path = Environment.GetEnvironmentVariable("DB_PATH") ?? "aseguraya.db";
        _cadena_conexion = $"Data Source={db_path};";
    }
        // ------------------------------------------------------------------
    // Métodos privados de apoyo
    // ------------------------------------------------------------------

    /// <summary>
    /// Crea y abre una nueva conexión SQLite.
    /// Cada llamada genera su propia conexión para garantizar thread-safety.
    /// </summary>
    private SqliteConnection AbrirConexion()
    {
        var conexion = new SqliteConnection(_cadena_conexion);
        conexion.Open();
        return conexion;
    }

    /// <summary>
    /// Agrega los parámetros de la lista al comando SQLite.
    /// </summary>
    /// <param name="cmd">Comando al que se agregarán los parámetros.</param>
    /// <param name="parametros">Lista de SqliteParameter. Puede ser null.</param>
    private static void AgregarParametros(SqliteCommand cmd, ArrayList? parametros)
    {
        if (parametros == null) return;

        foreach (SqliteParameter param in parametros)
        {
            cmd.Parameters.Add(new SqliteParameter(param.ParameterName, param.Value ?? DBNull.Value));
        }
    }

    // ------------------------------------------------------------------
    // Métodos públicos
    // ------------------------------------------------------------------

    /// <summary>
    /// Ejecuta una consulta SELECT y retorna los resultados en un DataTable.
    /// </summary>
    /// <param name="consulta">Consulta SQL con placeholders (@param).</param>
    /// <param name="parametros">Lista de SqliteParameter. Debe incluir @tenant_id.</param>
    /// <returns>DataTable con los resultados.</returns>
    /// <exception cref="Exception">Error SQL o inesperado.</exception>
    public DataTable Consultar(string consulta, ArrayList? parametros)
    {
        var dt = new DataTable();

        try
        {
            using var conexion = AbrirConexion();
            using var cmd = new SqliteCommand(consulta, conexion);

            AgregarParametros(cmd, parametros);

            using var reader = cmd.ExecuteReader();
            dt.Load(reader);
        }
        catch (SqliteException ex)
        {
            throw new Exception($"⚠️ Error SQLite en consulta: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"🛑 Error inesperado en consulta: {ex.Message}", ex);
        }

        return dt;
    }

    /// <summary>
    /// Ejecuta una instrucción INSERT, UPDATE o DELETE.
    /// </summary>
    /// <param name="consulta">Instrucción SQL con placeholders (@param).</param>
    /// <param name="parametros">Lista de SqliteParameter. Debe incluir @tenant_id.</param>
    /// <exception cref="Exception">Error SQL o inesperado.</exception>
    public void Guardar(string consulta, ArrayList? parametros)
    {
        try
        {
            using var conexion = AbrirConexion();
            using var cmd = new SqliteCommand(consulta, conexion);

            AgregarParametros(cmd, parametros);
            cmd.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            throw new Exception($"⚠️ Error SQLite al guardar: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"🛑 Error inesperado al guardar: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Ejecuta una instrucción SQL y retorna el primer valor de la primera fila.
    /// Útil para COUNT, MAX, o recuperar un ID recién insertado.
    /// </summary>
    /// <param name="consulta">Instrucción SQL con placeholders (@param).</param>
    /// <param name="parametros">Lista de SqliteParameter. Debe incluir @tenant_id.</param>
    /// <returns>Valor escalar o null.</returns>
    /// <exception cref="Exception">Error SQL o inesperado.</exception>
    public object? Escalar(string consulta, ArrayList? parametros)
    {
        try
        {
            using var conexion = AbrirConexion();
            using var cmd = new SqliteCommand(consulta, conexion);

            AgregarParametros(cmd, parametros);
            return cmd.ExecuteScalar();
        }
        catch (SqliteException ex)
        {
            throw new Exception($"⚠️ Error SQLite (escalar): {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"🛑 Error inesperado en escalar: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Ejecuta múltiples instrucciones SQL dentro de una transacción atómica.
    /// Si alguna falla, se hace rollback de todas.
    /// </summary>
    /// <param name="operaciones">
    /// Lista de tuplas (consulta, parametros) a ejecutar en orden.
    /// </param>
    /// <exception cref="Exception">Error SQL o inesperado. Se garantiza rollback.</exception>
    public void EjecutarTransaccion(List<(string consulta, ArrayList? parametros)> operaciones)
    {
        using var conexion = AbrirConexion();
        using var transaccion = conexion.BeginTransaction();

        try
        {
            foreach (var (consulta, parametros) in operaciones)
            {
                using var cmd = new SqliteCommand(consulta, conexion, transaccion);
                AgregarParametros(cmd, parametros);
                cmd.ExecuteNonQuery();
            }

            transaccion.Commit();
        }
        catch (SqliteException ex)
        {
            transaccion.Rollback();
            throw new Exception($"⚠️ Error SQLite en transacción (rollback aplicado): {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            transaccion.Rollback();
            throw new Exception($"🛑 Error inesperado en transacción (rollback aplicado): {ex.Message}", ex);
        }
    }
}