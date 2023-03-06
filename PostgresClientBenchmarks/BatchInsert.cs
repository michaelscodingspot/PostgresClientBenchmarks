using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

public class BatchInsert
{
    private PostgresNpgsql _postgresNpgsql;
	private PostgresDapper _postgresDapper;
	private PostgresEF _postgresEF;
	private List<Teacher> _teachers;
    private int _id = 0;
	//const int iterations = 5000;
	// private Postgres _postgres;
	const int numTeachers = 1000;

	[GlobalSetup]
    public async Task Setup()
	{
		GenerateRandomTeachers();

		_postgresNpgsql = new PostgresNpgsql();
		_postgresDapper = new PostgresDapper(_postgresNpgsql.GetConnection());
		_postgresEF = new PostgresEF();

		await _postgresNpgsql.CreateTableAsync();
	}

	private void GenerateRandomTeachers()
	{
		_teachers = new List<Teacher>();
		for (int i = 0; i < numTeachers; i++)
		{
			_teachers.Add(Teacher.GetRandomTeacher());

		}
	}

	[IterationSetup]
    public void MyIterationSetup()
    {
        //Console.WriteLine("\n\n-------------------------------------------------Resetting table\n");
        if (_id % 1_000_000 == 0) {
            _postgresNpgsql.CreateTable();
        }
        _id = 0;
    }

    [Benchmark]
    public void NpgsqlInsertRegular()
    {
        _postgresNpgsql.BulkInsertRegular(_teachers);
        _id++;
    }

	[Benchmark]
	public void NpgsqlInsertRegular2()
	{
		_postgresNpgsql.BulkInsertRegular2(_teachers);
		_id++;
	}

	[Benchmark]
	public void NpgsqlInsertInTransaction()
	{
		_postgresNpgsql.BulkInsertInTransaction(_teachers);
		_id++;
	}

	[Benchmark]
	public async Task NpgsqlInsertBinaryImporter()
	{
		await _postgresNpgsql.BulkInsertBinaryImporter(_teachers);
		_id++;
	}

	//[Benchmark]
	//public async Task NpgsqlInsertBinaryImporterAndTransaction()
	//{
	//	await _postgresNpgsql.BulkInsertBinaryFormatterAndTransaction(_teachers);
	//	_id++;
	//}





}

