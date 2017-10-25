﻿using NUnit.Framework;
using Omnifactotum.Annotations;

namespace Omnifactotum.Tests
{
    internal static class FakeUseHelper
    {
        public static void UseValue<T>([CanBeNull] this T value)
        {
            Assert.That(value, Is.Null | Is.AssignableTo<object>());
        }
    }
}