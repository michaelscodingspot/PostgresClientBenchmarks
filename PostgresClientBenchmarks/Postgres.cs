using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Npgsql;

class Postgres
{
    public static readonly string ConnectionString  = $"Server=localhost;Port=5432;User Id=postgres;Password=p0stgres;Database=testdb;";

    public NpgsqlConnection _con;
    private NpgsqlCommand _cmd;

    private const string rowDisplayText = "DT";//VARCHAR(255),
    private const string rowRawValueJson = "RAW"; //VARCHAR(255),
    private const string rowFormulaR1C1Json = "FM";// VARCHAR(255),
    private const string rowNumberFormat = "NF";//VARCHAR(255),
    private const string rowNumberFormatCategory = "NFC"; //VARCHAR(255),
    private const string rowLinkedDataTypeClassificationId = "L"; //VARCHAR(255),
    private const string rowSessionId = "S"; //VARCHAR(255),
    private const string rowMergedCellInfo = "MC"; //INT,
    private const string rowXlcol = "CL";//INT,
    private const string rowXlrow = "RW";//INT,
    private const string rowWorksheetId = "W"; //INT

    public void InitNewTable(string tableName = "cells")
    {
        InitConnection();
        CreateTable(tableName);
    }

    public void InitWithoutCreatingTable()
    {
        InitConnection();
    }

    private void InitConnection()
    {
        
        _con = new NpgsqlConnection(connectionString: ConnectionString);
        _con.Open();

        _cmd = new NpgsqlCommand();

        _cmd.Connection = _con;
    }

    
    public object[][] ReadAllRows()
    {
        string sql = "SELECT * FROM cells";
        using var cmd = new NpgsqlCommand(sql, _con);

        using NpgsqlDataReader rdr = cmd.ExecuteReader();
        object[][] res = new object[1_000_000][];
        object[] v = new object[16];
        int i = 0;
        while (rdr.Read())
        {
            rdr.GetValues(v);
            res[i] = v;
            i++;
            //if (i % 100_000 == 0)
            //{
            //    Console.WriteLine(String.Join(", ", v));
            //}
        }
        return res;
    }

    public void CreateTable(string tableName = "cells")
    {
        DropTable(tableName);
        ExecuteCmd(_cmd, $"CREATE TABLE {tableName}(id SERIAL PRIMARY KEY," +
$"{rowDisplayText} VARCHAR(255), " +
$"{rowRawValueJson} VARCHAR(255)," +
$"{rowFormulaR1C1Json} VARCHAR(255)," +
$"{rowNumberFormat} VARCHAR(255)," +
$"{rowNumberFormatCategory} VARCHAR(255)," +
$"{rowLinkedDataTypeClassificationId} VARCHAR(255)," +
$"{rowSessionId} VARCHAR(255)," +
$"{rowMergedCellInfo} INT," +
$"{rowXlcol} INT," +
$"{rowXlrow} INT," +
$"{rowWorksheetId} INT" +
$")"
);

        // Console.WriteLine($"Table {tableName} created");
    }

    public NpgsqlCommand GetCommand()
    {
        return _cmd;
    }

    public void DropTable(string tableName = "cells")
    {
        ExecuteCmd(_cmd, $"DROP TABLE IF EXISTS {tableName}");
    }

    public async Task DropTableAsync(string tableName = "cells")
    {
        await ExecuteCmdAsync(_cmd, $"DROP TABLE IF EXISTS {tableName}").ConfigureAwait(false);
    }

    public void CreateIndex(string tableName = "cells")
    {
        ExecuteCmd(_cmd, $"CREATE INDEX idx_sessionId{tableName} ON {tableName}({rowSessionId})");
        ExecuteCmd(_cmd, $"CREATE INDEX idx_col{tableName} ON {tableName}({rowXlcol})");
        ExecuteCmd(_cmd, $"CREATE INDEX idx_row{tableName} ON {tableName}({rowXlrow})");
    }



    //private static void AddRows(NpgsqlCommand cmd)
    //{
    //    AddRow(cmd, "displayText1", "rawValueJson1", "formulaR1C1Json", "numberFormat", "numberFormatCategory","linkedDataTypeClassificationId", mergedCellInfo: 1, xlcol: 2, xlrow: 3, worksheetId: 4);
    //    //ExecuteCmd(cmd, "INSERT INTO cells(name, price) VALUES('Mercedes',57127)");

    //    //ExecuteCmd(cmd, "INSERT INTO cells(name, price) VALUES('Skoda',9000)");
    //    //ExecuteCmd(cmd, "INSERT INTO cells(name, price) VALUES('Volvo',29000)");
    //    //ExecuteCmd(cmd, "INSERT INTO cells(name, price) VALUES('Bentley',350000)");
    //    //ExecuteCmd(cmd, "INSERT INTO cells(name, price) VALUES('Citroen',21000)");
    //    //ExecuteCmd(cmd, "INSERT INTO cells(name, price) VALUES('Hummer',41400)");
    //    //ExecuteCmd(cmd, "INSERT INTO cells(name, price) VALUES('Volkswagen',21600)");


    //}

    
    private static void AddRow(NpgsqlCommand cmd, string displayText,string rawValueJson,string formulaR1C1Json,string numberFormat,string numberFormatCategory,
        string linkedDataTypeClassificationId,int mergedCellInfo,int xlcol,int xlrow,int worksheetId)
    {
        ExecuteCmd(cmd, $"INSERT INTO cells(displayText, rawValueJson, formulaR1C1Json, numberFormat, numberFormatCategory, linkedDataTypeClassificationId, mergedCellInfo, xlcol, xlrow, worksheetId) " +
            $"VALUES('{displayText}','{rawValueJson}','{formulaR1C1Json}','{numberFormat}','{numberFormatCategory}','{linkedDataTypeClassificationId}',{mergedCellInfo},{xlcol},{xlrow},{worksheetId})");
    }

    private static void PrintVersion(NpgsqlConnection con)
    {
        var sql = "SELECT version()";
        var cmd = new NpgsqlCommand(sql, con);

        var version = cmd.ExecuteScalar().ToString();
        Console.WriteLine($"PostgreSQL version: {version}");
    }


    private static void ExecuteCmd(NpgsqlCommand cmd, string command)
    {
        cmd.CommandText = command;
        cmd.ExecuteNonQuery();
    }

    private static async Task ExecuteCmdAsync(NpgsqlCommand cmd, string command)
    {
        cmd.CommandText = command;
        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    
    
}