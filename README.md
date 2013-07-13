SqlBatchRunner
-----------

When you have many insert/update statements to run, it can take longer due to the communication overhead between your executable and SQL server.
For example,
```c#
    private void InsertOneByOne()
    {
        var ids = Enumerable.Range(1, 100);
        
        foreach (var id in ids)
            conn.Execute("insert into MyTable(id) values(@id)", new { id });
    }
```
can take a lot longer than
```c#
    private void InsertByBatch()
    {
        conn.Execute(@"
        insert into MyTable(id) values(1)
        insert into MyTable(id) values(2)
        insert into MyTable(id) values(3)
        ......
        insert into MyTable(id) values(100)
        ");
    }
```
But InsertByBatch() looks just too ugly for your clean code. How should we solve this problem?

SqlBatchRunner comes to rescue. The above statement can be written like this:
```c#
    private void InsertUsingSqlBatchRunner()
    {
        var ids = Enumerable.Range(1, 100);
        
        var batchRunner = new SqlBatchRunner(conn);
        
        foreach (var id in ids)
            batchRunner.RecordingConnection.Execute("insert into MyTable(id) values(@id)", new { id });
            
        batchRunner.Run();
    }
```
1. You instantiate ```SqlBatchRunner``` instance and provide ```IDbConnection``` to the constructor. 
2. When you call insert function, **use the batch runner's RecordingConnection** instead of real connection. 
3. At the end, you ask the batch runner to run everything at once.

Note
-----------
* Batch runner also splits entire statement into chunks so that you don't end up sending way too big query that SQL cannot parse at once.
* [Dapper] [1] provides a way to pass IEnumerable as a parameter, but internally it invokes SQL call on each insertion.
* It works only for Microsoft SQL Server. To extend, more ```IParameterValueConverter``` can be implemented.
* As I was using [Dapper] [1], I faced another problem. ```where MyColumn in @ids``` syntax passes each element of ```@ids``` as a separate parameter. Not only that it will be super slow as it gets bigger, but also SQL will raise an error if it goes over 2,100.
To solve this problem, I implemented ```TempIdTable``` that you can find in Test project.
* To test performance, try changing ```connectionString``` defined in ```TestDb.cs```. If your SQL server is local, SqlBatchRunner will make it slower. If your SQL server is remote, it will be faster.
[1]: https://code.google.com/p/dapper-dot-net/ "Dapper"
