using MySql.Data.MySqlClient;
using System;

namespace SalonDeBelleza.src.services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string TestConnection()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open(); // Intenta abrir la conexión
                    return "Conexión a la base de datos exitosa.";
                }
            }
            catch (MySqlException ex)
            {
                return $"Error al conectar a la base de datos: {ex.Message}";
            }
        }
    }
}