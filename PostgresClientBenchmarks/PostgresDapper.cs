using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.CodeAnalysis;
using Npgsql;
using Npgsql.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

public class PostgresDapper : IDisposable
{
    private NpgsqlConnection _connection;

    public PostgresDapper()
    {
        var cs = $"Server=localhost;Port=5432;User Id=postgres;Password=p0stgres;Database=testdb;";
        _connection = new NpgsqlConnection(cs);
        _connection.Open();
    }

	public PostgresDapper(NpgsqlConnection npgsqlConnection)
	{
		_connection = npgsqlConnection;
	}

	public async Task CreateTable()
    {
await _connection.ExecuteAsync($"DROP TABLE IF EXISTS {Teacher.TableName}").ConfigureAwait(false);
await _connection.ExecuteAsync($"CREATE TABLE {Teacher.TableName}(id SERIAL PRIMARY KEY," +
    $"first_name VARCHAR(255), " +
    $"last_name VARCHAR(255)," +
    $"subject VARCHAR(255)," +
    $"salary INT)").ConfigureAwait(false);
    }


    public async Task Insert(Teacher teacher)
    {
    string sqlCommand = $"INSERT INTO {Teacher.TableName} (first_name, last_name, subject, salary) VALUES (@firstName, @lastName, @Subject, @salary)";

    var queryArguments = new
    {
        firstName = teacher.FirstName,
        lastName = teacher.LastName,
        subject = teacher.Subject,
        salary = teacher.Salary,
            
    };

    await _connection.ExecuteAsync(sqlCommand, queryArguments).ConfigureAwait(false);
    }

    public async Task DeleteById(int id)
    {
        string sqlCommand = $"DELETE FROM {Teacher.TableName} WHERE id = {id}";
        await _connection.ExecuteAsync(sqlCommand).ConfigureAwait(false);
    }

    public async Task UpdateById(int id, Teacher newValues)
    {
        string sqlCommand = $"UPDATE {Teacher.TableName}" +
            $" SET first_name='{newValues.FirstName}',last_name='{newValues.LastName}',subject='{newValues.Subject}',salary={newValues.Salary}" +
            $" WHERE id = {id}";
        await _connection.ExecuteAsync(sqlCommand).ConfigureAwait(false);
    }

	public async Task UpdateLastNameById(int id, string newLastName)
	{
		string sqlCommand = $"UPDATE {Teacher.TableName} SET last_name='{newLastName}' WHERE id = {id}";
		await _connection.ExecuteAsync(sqlCommand).ConfigureAwait(false);
	}

	public async Task<IEnumerable<Teacher>> GetBySubject(string subject)
    {
        string commandText = $"SELECT * FROM {Teacher.TableName} WHERE subject='{subject}'";
        var teachers = await _connection.QueryAsync<Teacher>(commandText).ConfigureAwait(false);

        return teachers;
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    


}
