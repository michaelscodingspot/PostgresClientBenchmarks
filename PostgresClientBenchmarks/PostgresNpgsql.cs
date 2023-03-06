using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.CodeAnalysis;
using Npgsql;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

public class PostgresNpgsql : IDisposable
{
    private NpgsqlConnection con;
    private NpgsqlCommand cmd;

    public PostgresNpgsql()
    {
        var cs = $"Server=localhost;Port=5432;User Id=postgres;Password=p0stgres;Database=testdb;";
        con = new NpgsqlConnection(cs);
        con.Open();
        cmd = new NpgsqlCommand();
        cmd.Connection = con;
    }

    public async Task CreateTableAsync()
    {
        cmd.CommandText = $"DROP TABLE IF EXISTS teachers";
        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
        cmd.CommandText = "CREATE TABLE teachers (id SERIAL PRIMARY KEY," +
            "first_name VARCHAR(255)," +
            "last_name VARCHAR(255)," +
            "subject VARCHAR(255)," +
            "salary INT)";
        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

	public async Task CreateIndexOnSubject()
	{
        string tableName = "teachers";
        cmd.CommandText = $"CREATE INDEX idx_sessionSubject{tableName} ON {tableName}(subject)";
        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
	}

	public async Task CreateIndexOnId()
	{
		string tableName = "teachers";
		cmd.CommandText = $"CREATE INDEX idx_sessionId{tableName} ON {tableName}(id)";
		await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
	}

	public void CreateTable()
	{
		cmd.CommandText = $"DROP TABLE IF EXISTS teachers";
		cmd.ExecuteNonQuery();
		cmd.CommandText = "CREATE TABLE teachers (id SERIAL PRIMARY KEY," +
			"first_name VARCHAR(255)," +
			"last_name VARCHAR(255)," +
			"subject VARCHAR(255)," +
			"salary INT)";
		cmd.ExecuteNonQuery();
	}


	public async Task Insert()
    {
        //cmd.CommandText = $"INSERT INTO teachers (first_name, last_name, subject, salary) VALUES ('Severus', 'Snape', 'Potions', 10000)";
        //await cmd.ExecuteNonQueryAsync();

        // or 
        cmd.CommandText = $"INSERT INTO teachers (first_name, last_name, subject, salary) VALUES (@firstName, @lastName, @subject, @salary)";
        cmd.Parameters.AddWithValue("firstName", "Severus");
        cmd.Parameters.AddWithValue("lastName", "Snape");
        cmd.Parameters.AddWithValue("subject", "Potions");
        cmd.Parameters.AddWithValue("salary", 10000);
        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

public async Task Insert(Teacher teacher)
{
    cmd.CommandText = $"INSERT INTO teachers (first_name, last_name, subject, salary) VALUES (@firstName, @lastName, @subject, @salary)";
    cmd.Parameters.AddWithValue("firstName", teacher.FirstName);
    cmd.Parameters.AddWithValue("lastName", teacher.LastName);
    cmd.Parameters.AddWithValue("subject", teacher.Subject);
    cmd.Parameters.AddWithValue("salary", teacher.Salary);
    await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
}

	public void InsertSync(Teacher teacher)
	{
		//cmd.CommandText = $"INSERT INTO teachers (first_name, last_name, subject, salary) VALUES ('Severus', 'Snape', 'Potions', 10000)";
		//await cmd.ExecuteNonQueryAsync();

		// or 
		cmd.CommandText = $"INSERT INTO teachers (first_name, last_name, subject, salary) VALUES (@firstName, @lastName, @subject, @salary)";
		cmd.Parameters.AddWithValue("firstName", teacher.FirstName);
		cmd.Parameters.AddWithValue("lastName", teacher.LastName);
		cmd.Parameters.AddWithValue("subject", teacher.Subject);
		cmd.Parameters.AddWithValue("salary", teacher.Salary);
		cmd.ExecuteNonQuery();
	}

	public void InsertSyncWithId(Teacher teacher)
	{
		//cmd.CommandText = $"INSERT INTO teachers (first_name, last_name, subject, salary) VALUES ('Severus', 'Snape', 'Potions', 10000)";
		//await cmd.ExecuteNonQueryAsync();

		// or 
		cmd.CommandText = $"INSERT INTO teachers (id, first_name, last_name, subject, salary) VALUES (@id, @firstName, @lastName, @subject, @salary)";
		cmd.Parameters.AddWithValue("id", teacher.Id);
		cmd.Parameters.AddWithValue("firstName", teacher.FirstName);
		cmd.Parameters.AddWithValue("lastName", teacher.LastName);
		cmd.Parameters.AddWithValue("subject", teacher.Subject);
		cmd.Parameters.AddWithValue("salary", teacher.Salary);
		cmd.ExecuteNonQuery();
	}

	public void InsertSyncRawSQL(Teacher teacher)
	{
		cmd.CommandText = $"INSERT INTO teachers (first_name, last_name, subject, salary) VALUES ('{teacher.FirstName}', '{teacher.LastName}', '{teacher.Subject}', {teacher.Salary})";
		cmd.ExecuteNonQuery();	
	}

	public async Task DeleteById(int id)
    {
        cmd.CommandText = $"DELETE FROM teachers WHERE id = {id}";
        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public async Task UpdateById(int id, Teacher newValues)
    {
        cmd.CommandText = $"UPDATE teachers" +
            $" SET first_name='{newValues.FirstName}'," +
            $"last_name='{newValues.LastName}'," +
            $"subject='{newValues.Subject}'," +
            $"salary={newValues.Salary}" +
            $" WHERE id = {id}";
        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

public async Task UpdateLastNameById(int id, string newLastName)
{
	//cmd.CommandText = $"UPDATE teachers" +
	//	$" SET last_name='{newLastName}' WHERE id = {id}";
	cmd.CommandText = $"UPDATE {Teacher.TableName} SET last_name='{newLastName}' WHERE id = {id}";
	await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
}

	public async Task<IEnumerable<Teacher>> GetBySubject(string subject)
    {
        cmd.CommandText = $"SELECT * FROM teachers WHERE subject='{subject}'";
        NpgsqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
        var result = new List<Teacher>();
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            result.Add(new Teacher(
                id: (int)reader["id"],
                first_name: reader[1] as string, // column index can be used
                last_name: reader.GetString(2), // another syntax option
                subject: reader["subject"] as string,
                salary: (int)reader["salary"]));

        }
        await reader.CloseAsync().ConfigureAwait(false);
        return result;
    }

	public IEnumerable<Teacher> GetBySubjectSync(string subject)
	{
		cmd.CommandText = $"SELECT * FROM teachers WHERE subject='{subject}'";
		NpgsqlDataReader reader = cmd.ExecuteReader();
		var result = new List<Teacher>();
		while (reader.Read())
		{
			result.Add(new Teacher(
				id: (int)reader["id"],
				first_name: reader[1] as string, // column index can be used
				last_name: reader.GetString(2), // another syntax option
				subject: reader["subject"] as string,
				salary: (int)reader["salary"]));

		}
        reader.Close();
		return result;
	}

	public void Dispose()
    {
        con.Dispose();
    }

	internal NpgsqlConnection GetConnection()
	{
		return con;
	}

    public void BulkInsertRegular(IEnumerable<Teacher> teachers)
    {
		// Define the SQL query for the batch insert


		// Create a NpgsqlCommand object to execute the query
		cmd.Parameters.Clear();
		cmd.CommandText = "INSERT INTO teachers (first_name, last_name, subject, salary) VALUES (@firstName, @lastName, @subject, @salary)";

		foreach (var teacher in teachers)
		{
			// Add the parameters to the command object
			cmd.Parameters.AddWithValue("firstName", teacher.FirstName);
			cmd.Parameters.AddWithValue("lastName", teacher.LastName);
			cmd.Parameters.AddWithValue("subject", teacher.Subject);
			cmd.Parameters.AddWithValue("salary", teacher.Salary);

			// Add more parameters and execute the command multiple times to insert multiple items in a batch
			cmd.ExecuteNonQuery();

			// Clear the parameters for the next batch
			cmd.Parameters.Clear();
		}
	}






	public void BulkInsertRegular2(IEnumerable<Teacher> teachers)
	{
		
		cmd.CommandText = "INSERT INTO teachers (first_name, last_name, subject, salary) VALUES " +
			(String.Join(',',teachers.Select(t => $"('{t.FirstName}','{t.LastName}','{t.Subject}',{t.Salary})")));

		cmd.ExecuteNonQuery();
	}



	public void BulkInsertInTransaction(IEnumerable<Teacher> teachers)
	{
		using var transaction = con.BeginTransaction();

		var sql = "INSERT INTO teachers (first_name, last_name, subject, salary) VALUES (@firstName, @lastName, @subject, @salary)";

		// Create a NpgsqlCommand object to execute the query
		using var command = new NpgsqlCommand(sql, con, transaction);
		foreach (var teacher in teachers)
		{
			// Add the parameters to the command object
			command.Parameters.AddWithValue("firstName", teacher.FirstName);
			command.Parameters.AddWithValue("lastName", teacher.LastName);
			command.Parameters.AddWithValue("subject", teacher.Subject);
			command.Parameters.AddWithValue("salary", teacher.Salary);

			// Add more parameters and execute the command multiple times to insert multiple items in a batch
			command.ExecuteNonQuery();

			// Clear the parameters for the next batch
			command.Parameters.Clear();
		}
		transaction.Commit();

	}


	// bulk insert items with Npgsql
	public async Task BulkInsertBinaryImporter(IEnumerable<Teacher> teachers)
    {
		using (var writer = con.BeginBinaryImport("COPY teachers (first_name, last_name, subject, salary) FROM STDIN (FORMAT BINARY)"))
        {
			foreach (var teacher in teachers)
            {
				await writer.StartRowAsync().ConfigureAwait(false);
				await writer.WriteAsync(teacher.FirstName, NpgsqlTypes.NpgsqlDbType.Varchar).ConfigureAwait(false);
				await writer.WriteAsync(teacher.LastName, NpgsqlTypes.NpgsqlDbType.Varchar).ConfigureAwait(false);
				await writer.WriteAsync(teacher.Subject, NpgsqlTypes.NpgsqlDbType.Varchar).ConfigureAwait(false);
				await writer.WriteAsync(teacher.Salary, NpgsqlTypes.NpgsqlDbType.Integer).ConfigureAwait(false);
			}
			await writer.CompleteAsync().ConfigureAwait(false);
		}
	}

	public async Task BulkInsertBinaryFormatterAndTransaction(IEnumerable<Teacher> teachers)
	{
		using var transaction = con.BeginTransaction();


		using (var writer = con.BeginBinaryImport("COPY teachers (first_name, last_name, subject, salary) FROM STDIN (FORMAT BINARY)"))
		{
			foreach (var teacher in teachers)
			{
				await writer.StartRowAsync().ConfigureAwait(false);
				await writer.WriteAsync(teacher.FirstName, NpgsqlTypes.NpgsqlDbType.Varchar).ConfigureAwait(false);
				await writer.WriteAsync(teacher.LastName, NpgsqlTypes.NpgsqlDbType.Varchar).ConfigureAwait(false);
				await writer.WriteAsync(teacher.Subject, NpgsqlTypes.NpgsqlDbType.Varchar).ConfigureAwait(false);
				await writer.WriteAsync(teacher.Salary, NpgsqlTypes.NpgsqlDbType.Integer).ConfigureAwait(false);
			}
			await writer.CompleteAsync().ConfigureAwait(false);
		}

		// Commit the transaction
		transaction.Commit();
	}


}
