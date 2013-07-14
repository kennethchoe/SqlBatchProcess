SqlBatchRunner
-----------

When you have many insert/update statements to run, it can take longer due to the communication overhead.

For example,
```c#
    private void InsertRecords(IDbConnection conn)
    {
        var ids = Enumerable.Range(1, 100);
        
        foreach (var id in ids)
            conn.Execute("insert into MyTable(id) values(@id)", new { id });
    }
```
can take longer in actual environment than you tested locally, because these SQL statements are invoked 
line by line.

```sql
    insert into MyTable(id) values(1)
    insert into MyTable(id) values(2)
    insert into MyTable(id) values(3)
    ......
    insert into MyTable(id) values(100)
```

How can we change our code to emit SQL code to the SQL server by batch? Do we need to change entire 
```InsertRecords()``` to do string concatenation and emit at once? That is likely a lot of work and not so clean code.

SqlBatchRunner comes to rescue. You can change the caller of ```InsertRecords()``` to do this:
```c#
    private void CallInsertRecords(IDbConnection conn)
    {
        var batchRunner = new SqlBatchRunner(conn);
        InsertRecords(batchRunner.RecordingConnection);
        batchRunner.Run();
    }
```
1. You instantiate ```SqlBatchRunner``` with ```IDbConnection``` as a parameter. 
2. When you call insert function, **use the batch runner's RecordingConnection** instead of real connection. 
3. At the end, you ask the batch runner to run everything at once.

Batch runner also splits entire statement into chunks so that you don't end up sending way too big query that SQL cannot parse at once.

Note
-----------
* [Dapper] [1] provides a way to pass IEnumerable as a parameter, but internally it invokes SQL call on each insertion. So it does not solve this problem.
* For now, SqlBatchRunner works only for Microsoft SQL Server. To extend, more ```IParameterValueConverter``` can be implemented.
* To test performance, try changing ```connectionString``` defined in ```TestDb.cs```. If your SQL server is local, SqlBatchRunner will make it slower. If your SQL server is remote, it will be much faster.

TempIdTable
-----------
As I was using [Dapper] [1], I faced another problem. 

When you write SQL passing array of objects like 
```where MyColumn in @ids```, each element of ```@ids``` is defined as as a separate SQL parameter. 
Not only that it will be super slow as it gets bigger, but also SQL will raise an error if it goes over 2,100.

To solve this problem, I implemented ```TempIdTable``` that you can find in Test project.

You use it like this:
```c#
    using (new TempIdTable(conn, ids, "#temp_table"))
    {
        conn.Execute("delete * from TableA where id in (select id from #temp_table)");
    }
```
```TempIdTable``` will create the temp table, insert ids into the temp table using SqlBatchRunner, and 
drop the temp table table when ```using``` ends.
[1]: https://code.google.com/p/dapper-dot-net/ "Dapper"
