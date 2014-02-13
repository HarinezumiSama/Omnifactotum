using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture]
    public sealed class OmnifactotumExpressionExtensionsTests
    {
        #region Tests

        [Test]
        public void TestGetMethod()
        {
            {
                Expression<Func<object, string>> expression = obj => obj.ToString();
                var method = expression.GetMethod();
                Assert.That(method, Is.Not.Null);
                Assert.That(method.Name, Is.EqualTo("ToString"));
                Assert.That(method.DeclaringType, Is.EqualTo(typeof(object)));
            }

            {
                Expression<Action<OmnifactotumExpressionExtensionsTests>> expression = obj => obj.TestGetMethod();
                var method = expression.GetMethod();
                Assert.That(method, Is.Not.Null);
                Assert.That(method.Name, Is.EqualTo(MethodBase.GetCurrentMethod().Name));
                Assert.That(method.DeclaringType, Is.EqualTo(GetType()));
            }
        }

        #endregion
    }
}