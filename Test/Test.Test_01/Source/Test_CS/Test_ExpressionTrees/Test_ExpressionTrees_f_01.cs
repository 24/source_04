using System;
using System.Linq.Expressions;
using pb;

namespace Test.Test_CS.Test_ExpressionTrees
{
    public static class Test_ExpressionTrees_f_01
    {
        public static void Test_ExpressionTrees_01()
        {
            Trace.WriteLine("create expression from lambda expression :");
            Trace.WriteLine("num => num < 5;");
            Expression<Func<int, bool>> lambda = num => num < 5;
            //lambda.
            Func<int, bool> func = lambda.Compile();
            Trace.WriteLine("result : func(1) = {0}", func(1));
            Trace.WriteLine("result : func(9) = {0}", func(9));
        }

        public static void Test_ExpressionTrees_02()
        {
            Trace.WriteLine("create expression manually :");
            Trace.WriteLine("num => num < 5;");
            // Manually build the expression tree for 
            // the lambda expression num => num < 5.
            ParameterExpression numParam = Expression.Parameter(typeof(int), "num");
            ConstantExpression five = Expression.Constant(5, typeof(int));
            BinaryExpression numLessThanFive = Expression.LessThan(numParam, five);
            Expression<Func<int, bool>> lambda = Expression.Lambda<Func<int, bool>>(numLessThanFive, new ParameterExpression[] { numParam });
            Func<int, bool> func = lambda.Compile();
            Trace.WriteLine("result : func(1) = {0}", func(1));
            Trace.WriteLine("result : func(9) = {0}", func(9));
        }

        public static void Test_ExpressionTrees_03()
        {

            // Add the following using directive to your code file:
            // using System.Linq.Expressions;

            // Create an expression tree.
            Expression<Func<int, bool>> exprTree = num => num < 5;

            // Decompose the expression tree.
            ParameterExpression param = (ParameterExpression)exprTree.Parameters[0];
            BinaryExpression operation = (BinaryExpression)exprTree.Body;
            ParameterExpression left = (ParameterExpression)operation.Left;
            ConstantExpression right = (ConstantExpression)operation.Right;

            Trace.WriteLine("expression : {0}", exprTree);
            Trace.WriteLine("Decomposed expression : {0} => {1} {2} {3}",
                              param.Name, left.Name, operation.NodeType, right.Value);

            // This code produces the following output:

            // Decomposed expression: num => num LessThan 5            
        }


    }
}
