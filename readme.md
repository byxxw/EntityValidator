## EntityValidator
### 介绍
使用字符串模式校验一个实体对象的属性值是否符合规范  
**不依赖任何第三方库**


### 示例
``` C#
var validator = new Validator();
var expression = "(begin>=2019-1-1)&&((age=1)||(timeout=10))";
var instance = new { begin = new DateTime(2019, 1, 1), age=-1, timeout=10 };

var result = validator.Validate(expression, instance);
Console.WriteLine(result);
Assert.AreEqual(true, result.Success);
```

### .Net支持
当前版本只支持.Net4.6.1

### 字符串模式说明
1. 布尔表达式支持

表达式 | 说明
---|---
&&|与运算
\|\| | 或运算

2. 判断表达式

表达式 | 说明
---|---
==|等于
\>|大于
\>=|大于等于
\<|小于
\<=|小于等于