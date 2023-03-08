using BenchmarkDotNet.Running;

Console.WriteLine("Starting");

BenchmarkRunner.Run<SingleInsert>();

//var pg = new SingleUpdate();
//await pg.Setup();

//await pg.EFUpdate();

//pg.BulkInsertRegular(new[] {Teacher.GetRandomTeacher(), Teacher.GetRandomTeacher()});
//var res  =await r.bulk

//var res = await new PostgresEF().GetBySubject("S5");

//Console.WriteLine(res.ToString());
