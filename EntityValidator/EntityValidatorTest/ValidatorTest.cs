namespace EntityValidatorTest
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using EntityValidator;
    using Newtonsoft.Json;
    using System.Reflection;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using System.Collections;

    [TestClass]
    public class ValidatorTest
    {
        [TestMethod]
        public void Test_AnonymousTypeObject()
        {
            var validator = new Validator();
            var expression = "(age<=11.1)&&(Department==test)&&(T==true)";
            var instance = new { Age = 11.1, Department = "test", T = true };

            var result = validator.Validate(expression, instance);
            Console.WriteLine(result);
            Assert.AreEqual(true, result.Success);

        }

        class Foo
        {
            public decimal Age { get; set; }
            public string Department { get; set; }
        }

        [TestMethod]
        public void Test_StrongTypeObject()
        {
            var validator = new Validator();
            var expression = "(age>=11.1)&&(Department==test)";
            var instance = new Foo { Age = 11.1M, Department = "test" };

            var result = validator.Validate(expression, instance);
            Console.WriteLine(result);
            Assert.AreEqual(true, result.Success);

        }

        [TestMethod]
        public void Test_DateTime_Fields()
        {
            var validator = new Validator();
            var expression = "(begin>=2019-1-1)&&((age=1)||(timeout=10))";
            var instance = new { begin = new DateTime(2019, 1, 1), age=-1, timeout=10 };

            var result = validator.Validate(expression, instance);
            Console.WriteLine(result);
            Assert.AreEqual(true, result.Success);

        }

        [TestMethod]
        public void Test_FromJsonString()
        {
            var validator = new Validator();
            var expression = "(age>=11.1)&&(Department==test)";

            var jsonString = "{'age': 11.1, 'Department': 'test'}";

            var definition = new { age = 0M, Department = string.Empty };
            var instance = JsonConvert.DeserializeAnonymousType(jsonString, definition);
            Console.WriteLine(instance);

            var result = validator.Validate(expression, instance);
            Console.WriteLine(result);
            Assert.AreEqual(true, result.Success);

        }

        [TestMethod]
        [ExpectedException(typeof(ResolveStringExpressionExpception))]
        public void Test_Expect_ResolveStringExpressionExpception()
        {
            var validator = new Validator();
            var expression = "age<=11.1)&&(Department==test)&&(T==true";
            var instance = new { Age = 11.1, Department = "test", T = true };

            var result = validator.Validate(expression, instance);
            Console.WriteLine(result);
            Assert.AreEqual(true, result.Success);

        }

        [TestMethod]
        [ExpectedException(typeof(EntityPropertyNotFoundException))]
        public void Test_Expect_EntityPropertyNotFoundException()
        {
            //  字符串表达里包涵的属性T1, 在实体中找不到
            var validator = new Validator();
            var expression = "(age<=11.1)&&(Department==test)&&(T1==true)";
            var instance = new { Age = 11.1, Department = "test", T = true };

            var result = validator.Validate(expression, instance);
            Console.WriteLine(result);
            Assert.AreEqual(true, result.Success);

        }

        [TestMethod]
        public void Test_BinaryTreeCreator()
        {
            var binaryTreeCreator = new BinaryTreeCreator();
            var expression = @"(age>11.1)
&&(Department=test)
&&(T1<12)
&&(T2>=1)
&&(T3<=1)";

            var root = binaryTreeCreator.CreateBinaryTree(expression);
            binaryTreeCreator.Print(root);
        }
    }
}

