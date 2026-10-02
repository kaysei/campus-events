using System.Data;
using Microsoft.Data.SqlClient;

namespace CampusEvents.Backend;

public class RegistrationService
{
    private readonly string _connectionString;

    public RegistrationService(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string is required.", nameof(connectionString));
        }

        _connectionString = connectionString;
    }

    public string? GetUserRegistration(string inputEmail)
    {
        if (string.IsNullOrWhiteSpace(inputEmail))
        {
            throw new ArgumentException("Email is required.", nameof(inputEmail));
        }

        const string sql =
            "SELECT TOP (1) r.RegistrationId " +
            "FROM dbo.Registrations AS r " +
            "INNER JOIN dbo.Users AS u ON u.UserId = r.UserId " +
            "WHERE u.Email = @Email";

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 254).Value = inputEmail.Trim();

        conn.Open();
        var result = cmd.ExecuteScalar();
        return result?.ToString();
    }
}

