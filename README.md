# Underground.ORM

![logo](doc/logo.png)  
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate/?hosted_button_id=83F8YMBRNSEQN)
[![NuGet stable version](https://badgen.net/nuget/v/Underground.ORM)](https://www.nuget.org/packages/Underground.ORM)
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

##### Method declared in CSharp syntax to translate:
```c#
[MySqlFunctionScope("MySqlFunctionName")]
private int SumMethodFunction(int a, int b)
{
    // Simples
    int var1;
    int? var2 = null;
    int var3 = 1;

    // Usando expressões
    int var4 = 1 + a;
    int var5 = 1 + a + b;
    int var6 = (1 + (a - b)) * 3;

    // Usando múltiplas variáveis
    int var7, var8, var9 = 4;
    int var10 = 1, var11 = 2, var12 = 3;
    int? var13 = 4, var14 = 5, var15 = null;
    int? var16 = null, var17, var18 = 9;
    int var19 = a, var20 = b;

    // Usando conversões cast
    int var21 = (int)1;
    int var22 = (int)1 + (int)2;
    int var23 = (int)1 + ((int)2 - ((int)3 + 5) - (int)3);

    // Usando conversões cast estranhas
    int var24 = (int)(((5)));
    int var25 = (int)(1 + 2 + 3);
    int var26 = (int)(((1 + 2 + 3))) - 2;
    int var27 = (int)(((1 + (2) + ((3))))) - ((int)((2)));

    int result = var3 + var4 + var5 + var6 + var20;

    return result;
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

##### Invoke method in server-side:
```C#
var result = await orm.RunFunctionAsync(SumMethodFunction, 10, 5);
```

##### Result in MySql syntax:
```mysql
CREATE FUNCTION `MySqlFunctionName` (`a` INT, `b` INT)
    RETURNS INT
BEGIN
    # Simples
    DECLARE `var1` INT;
    DECLARE `var2` INT DEFAULT null;
    DECLARE `var3` INT DEFAULT 1;
    
    # Usando expressões
    DECLARE `var4` INT DEFAULT 1+`a`;
    DECLARE `var5` INT DEFAULT 1+`a`+`b`;
    DECLARE `var6` INT DEFAULT (1+(`a`-`b`))*3;
    
    # Usando múltiplas variáveis
    DECLARE `var7` INT;
    DECLARE `var8` INT;
    DECLARE `var9` INT DEFAULT 4;
    DECLARE `var10` INT DEFAULT 1;
    DECLARE `var11` INT DEFAULT 2;
    DECLARE `var12` INT DEFAULT 3;
    DECLARE `var13` INT DEFAULT 4;
    DECLARE `var14` INT DEFAULT 5;
    DECLARE `var15` INT DEFAULT null;
    DECLARE `var16` INT DEFAULT null;
    DECLARE `var17` INT;
    DECLARE `var18` INT DEFAULT 9;
    DECLARE `var19` INT DEFAULT `a`;
    DECLARE `var20` INT DEFAULT `b`;
    
    # Usando conversões cast
    DECLARE `var21` INT DEFAULT CAST(1 AS SIGNED INT);
    DECLARE `var22` INT DEFAULT CAST(1 AS SIGNED INT)+CAST(2 AS SIGNED INT);
    DECLARE `var23` INT DEFAULT CAST(1 AS SIGNED INT)+(CAST(2 AS SIGNED INT)-(CAST(3 AS SIGNED INT)+5)-CAST(3 AS SIGNED INT));
    
    # Usando conversões cast estranhas
    DECLARE `var24` INT DEFAULT CAST((((5))) AS SIGNED INT);
    DECLARE `var25` INT DEFAULT CAST((1+2+3) AS SIGNED INT);
    DECLARE `var26` INT DEFAULT CAST((((1+2+3))) AS SIGNED INT)-2;
    DECLARE `var27` INT DEFAULT CAST((((1+(2)+((3))))) AS SIGNED INT)-(CAST(((2)) AS SIGNED INT));
    
    DECLARE `result` INT DEFAULT `var3`+`var4`+`var5`+`var6`+`var20`;
    
    RETURN `result`;
END;
```



