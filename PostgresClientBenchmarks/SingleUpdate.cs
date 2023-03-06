using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

public class SingleUpdate
{
    private PostgresNpgsql _postgresNpgsql;
	private PostgresDapper _postgresDapper;
	private PostgresEF _postgresEF;
    // private Postgres _postgres;
    private int _postfix = 0;

	[GlobalSetup]
    public async Task Setup()
    {
        _postgresNpgsql = new PostgresNpgsql();
        await _postgresNpgsql.CreateTableAsync();
        await _postgresNpgsql.CreateIndexOnSubject();
        await _postgresNpgsql.CreateIndexOnId();
        _postgresDapper = new PostgresDapper(_postgresNpgsql.GetConnection());
        _postgresEF = new PostgresEF();

        int items = 10_000;
        for (int i = 0; i < items; i++)
        {
            var item = Teacher.GetRandomTeacher();
            item.Id = i;
            _postgresNpgsql.InsertSync(item);
        }
    }



    [Benchmark]
    public async Task DapperUpdate()
    {
        _postfix = (_postfix + 1) % 10000;
        await _postgresDapper.UpdateLastNameById(100, $"Smith{_postfix}").ConfigureAwait(false);
    }

    [Benchmark]
    public async Task NpgsqlUpdate()
    {
		_postfix = (_postfix + 1) % 10000;
		await _postgresNpgsql.UpdateLastNameById(100, $"Smith{_postfix}").ConfigureAwait(false);
    }



    [Benchmark]
    public async Task EFUpdate()
    {
		_postfix = (_postfix + 1) % 10000;
		await _postgresEF.UpdateLastNameById(100, $"Smith{_postfix}").ConfigureAwait(false);
	}


}

