using Microsoft.EntityFrameworkCore;

namespace hotzerver.pilkki.calendar.Data;

public static class DbSchemaUpdater
{
    public static void EnsureLatestSchema(PilkkiDbContext db)
    {
        if (!ColumnExists(db, "Unavailabilities", "Priority"))
        {
            db.Database.ExecuteSqlRaw("ALTER TABLE Unavailabilities ADD COLUMN Priority INTEGER NOT NULL DEFAULT 1;");
        }
    }

    private static bool ColumnExists(PilkkiDbContext db, string tableName, string columnName)
    {
        var connection = db.Database.GetDbConnection();
        var wasClosed = connection.State != System.Data.ConnectionState.Open;

        if (wasClosed)
        {
            connection.Open();
        }

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info({tableName});";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (string.Equals(reader[1]?.ToString(), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        finally
        {
            if (wasClosed)
            {
                connection.Close();
            }
        }
    }
}
