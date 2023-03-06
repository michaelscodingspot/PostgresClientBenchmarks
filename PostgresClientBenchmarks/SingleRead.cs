using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

public class SingleRead
{
    private PostgresNpgsql _postgresNpgsql;
	private PostgresDapper _postgresDapper;
	private PostgresEF _postgresEF;
	// private Postgres _postgres;

	[GlobalSetup]
    public async Task Setup()
    {
        _postgresNpgsql = new PostgresNpgsql();
        await _postgresNpgsql.CreateTableAsync();
        await _postgresNpgsql.CreateIndexOnSubject();
        _postgresDapper = new PostgresDapper(_postgresNpgsql.GetConnection());
        _postgresEF = new PostgresEF();

        int items = 100_00;
        for (int i = 0; i < items; i++)
        {
            var item = Teacher.GetRandomTeacher();
            item.Id = i;
            _postgresNpgsql.InsertSync(item);
        }
    }



    [Benchmark]
    public async Task<int> NpgsqlRead()
    {
        return (await _postgresNpgsql.GetBySubject("S5").ConfigureAwait(false)).Count();
    }


    [Benchmark]
    public async Task<int> DapperRead()
    {
        
        return (await _postgresDapper.GetBySubject("S5").ConfigureAwait(false)).Count();
    }

    [Benchmark]
    public async Task<int> EFRead()
    {
        return (await _postgresEF.GetBySubject("S5").ConfigureAwait(false)).Count();
    }


}

