﻿using Moq;
using Vim.Interpreter;
using Xunit;

namespace Vim.UnitTest
{
    public sealed class ExpressionInterpreterTest
    {
        private readonly ExpressionInterpreter _interpreter;
        private readonly Mock<IStatusUtil> _statusUtil;

        public ExpressionInterpreterTest()
        {
            _statusUtil = new Mock<IStatusUtil>(MockBehavior.Strict);
            _interpreter = new ExpressionInterpreter(_statusUtil.Object, null, null, null, null);
        }

        private VariableValue Run(string expr)
        {
            var parseResult = VimUtil.ParseExpression(expr);
            Assert.True(parseResult.IsSucceeded);
            return _interpreter.RunExpression(parseResult.AsSucceeded().Item);
        }

        private void Run(string expr, string expected)
        {
            var value = Run(expr);
            Assert.Equal(expected, value.AsString().Item);
        }

        private void Run(string expr, int number)
        {
            var value = Run(expr);
            Assert.Equal(number, value.AsNumber().Item);
        }

        /// <summary>
        /// Add two numbers together and test the result
        /// </summary>
        [Fact]
        public void Add_SimpleNumber()
        {
            Run("1 + 2", 3);
        }

        [Fact]
        public void Concat_two_strings()
        {
            Run("'vs' . 'vim'", "vsvim");
        }

        [Fact]
        public void Concat_two_integers()
        {
            Run("2 . 3", "23");
        }
    }
}
