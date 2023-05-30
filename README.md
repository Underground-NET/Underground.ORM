# Underground.ORM

![logo](doc/logo.png)  
[![Production](https://github.com/Underground-NET/Underground.ORM/actions/workflows/production.yml/badge.svg?branch=master)](https://github.com/Underground-NET/Underground.ORM/actions/workflows/production.yml)
[![Development](https://github.com/Underground-NET/Underground.ORM/actions/workflows/development.yml/badge.svg?branch=develop)](https://github.com/Underground-NET/Underground.ORM/actions/workflows/development.yml)

### Example code:

##### Initialization:
```C#
MySqlConnectionStringBuilder sb = new();

sb.Server = "localhost";
sb.Port = 3306;
sb.UserID = "root";
sb.Password = "12345678";
sb.Database = "underground_orm_tests";
sb.AllowUserVariables = true;

var orm = new OrmEngine(sb) { EnsureCreateDatabase = true };
```

##### Method declared in CSharp to translate:
```c#
[MySqlFunctionScope("MySqlFunctionName")]
private int SumMethodFunction(int a, int b)
{
    return a + b;
}
```

##### Build the function statement:
```c#
var builtFunction = orm.BuildFunctionCreateStatement<int, int, int>(SumMethodFunction);
```

##### Update function in database:
```c#
await orm.UpdateDatabaseAsync(builtFunction);
```

##### Result in MySql:
```mysql
CREATE FUNCTION `MySqlFunctionName` (`a` INT, `b` INT)
RETURNS INT
BEGIN
  RETURN `a`+`b`;
END;
```

##### Invoke method in server-side
```C#
var result = await orm.RunFunctionAsync(SumMethodFunction, 10, 5);
```

