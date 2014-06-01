using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture]
    public sealed class FixedSizeDictionaryTests
    {
        #region Tests

        [Test]
        public void TestBasicScenario()
        {
            const string FalseValue = "NO";
            const string TrueValue = "yes";
            const string AnotherTrueValue = "t.r.u.e.";

            var dictionary = new FixedSizeDictionary<bool, string, BooleanDeterminant>();

            Assert.That(dictionary.Keys, Is.Not.Null);
            Assert.That(dictionary.Values, Is.Not.Null);

            Assert.That(dictionary.Count, Is.EqualTo(0));
            Assert.That(dictionary.Keys.Count, Is.EqualTo(0));
            Assert.That(dictionary.Keys.Contains(false), Is.False);
            Assert.That(dictionary.Keys.Contains(true), Is.False);
            Assert.That(dictionary.Values.Count, Is.EqualTo(0));
            Assert.That(dictionary.Values.Contains(TrueValue), Is.False);
            Assert.That(dictionary.Values.Contains(AnotherTrueValue), Is.False);
            Assert.That(dictionary.Values.Contains(FalseValue), Is.False);
            Assert.That(dictionary.ContainsKey(false), Is.False);
            Assert.That(dictionary.ContainsKey(true), Is.False);
            Assert.That(() => dictionary[false], Throws.TypeOf<KeyNotFoundException>());
            Assert.That(() => dictionary[true], Throws.TypeOf<KeyNotFoundException>());

            dictionary.Add(true, TrueValue);
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary.Keys.Count, Is.EqualTo(1));
            Assert.That(dictionary.Keys.Contains(false), Is.False);
            Assert.That(dictionary.Keys.Contains(true), Is.True);
            Assert.That(dictionary.Values.Count, Is.EqualTo(1));
            Assert.That(dictionary.Values.Contains(TrueValue), Is.True);
            Assert.That(dictionary.Values.Contains(AnotherTrueValue), Is.False);
            Assert.That(dictionary.Values.Contains(FalseValue), Is.False);
            Assert.That(dictionary.ContainsKey(false), Is.False);
            Assert.That(dictionary.ContainsKey(true), Is.True);
            Assert.That(() => dictionary[false], Throws.TypeOf<KeyNotFoundException>());
            Assert.That(dictionary[true], Is.EqualTo(TrueValue));

            dictionary[true] = AnotherTrueValue;
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary.Keys.Count, Is.EqualTo(1));
            Assert.That(dictionary.Keys.Contains(false), Is.False);
            Assert.That(dictionary.Keys.Contains(true), Is.True);
            Assert.That(dictionary.Values.Count, Is.EqualTo(1));
            Assert.That(dictionary.Values.Contains(TrueValue), Is.False);
            Assert.That(dictionary.Values.Contains(AnotherTrueValue), Is.True);
            Assert.That(dictionary.Values.Contains(FalseValue), Is.False);
            Assert.That(dictionary.ContainsKey(false), Is.False);
            Assert.That(dictionary.ContainsKey(true), Is.True);
            Assert.That(() => dictionary[false], Throws.TypeOf<KeyNotFoundException>());
            Assert.That(dictionary[true], Is.EqualTo(AnotherTrueValue));

            Assert.That(() => dictionary.Add(true, TrueValue), Throws.ArgumentException);
            Assert.That(() => dictionary.Add(true, AnotherTrueValue), Throws.ArgumentException);
            Assert.That(() => dictionary.Add(true, "anything"), Throws.ArgumentException);

            dictionary.Add(false, FalseValue);
            Assert.That(dictionary.Count, Is.EqualTo(2));
            Assert.That(dictionary.Keys.Count, Is.EqualTo(2));
            Assert.That(dictionary.Keys.Contains(false), Is.True);
            Assert.That(dictionary.Keys.Contains(true), Is.True);
            Assert.That(dictionary.Values.Count, Is.EqualTo(2));
            Assert.That(dictionary.Values.Contains(TrueValue), Is.False);
            Assert.That(dictionary.Values.Contains(AnotherTrueValue), Is.True);
            Assert.That(dictionary.Values.Contains(FalseValue), Is.True);
            Assert.That(dictionary.ContainsKey(false), Is.True);
            Assert.That(dictionary.ContainsKey(true), Is.True);
            Assert.That(dictionary[false], Is.EqualTo(FalseValue));
            Assert.That(dictionary[true], Is.EqualTo(AnotherTrueValue));

            var removed = dictionary.Remove(true);
            Assert.That(removed, Is.True);
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary.Keys.Count, Is.EqualTo(1));
            Assert.That(dictionary.Keys.Contains(false), Is.True);
            Assert.That(dictionary.Keys.Contains(true), Is.False);
            Assert.That(dictionary.Values.Count, Is.EqualTo(1));
            Assert.That(dictionary.Values.Contains(TrueValue), Is.False);
            Assert.That(dictionary.Values.Contains(AnotherTrueValue), Is.False);
            Assert.That(dictionary.Values.Contains(FalseValue), Is.True);
            Assert.That(dictionary.ContainsKey(false), Is.True);
            Assert.That(dictionary.ContainsKey(true), Is.False);
            Assert.That(() => dictionary[true], Throws.TypeOf<KeyNotFoundException>());
            Assert.That(dictionary[false], Is.EqualTo(FalseValue));

            dictionary.Clear();
            Assert.That(dictionary.Count, Is.EqualTo(0));
            Assert.That(dictionary.Keys, Is.Not.Null);
            Assert.That(dictionary.Keys.Contains(false), Is.False);
            Assert.That(dictionary.Keys.Contains(true), Is.False);
            Assert.That(dictionary.Keys.Count, Is.EqualTo(0));
            Assert.That(dictionary.Values, Is.Not.Null);
            Assert.That(dictionary.Values.Count, Is.EqualTo(0));
            Assert.That(dictionary.Values.Contains(TrueValue), Is.False);
            Assert.That(dictionary.Values.Contains(AnotherTrueValue), Is.False);
            Assert.That(dictionary.Values.Contains(FalseValue), Is.False);
            Assert.That(dictionary.ContainsKey(false), Is.False);
            Assert.That(dictionary.ContainsKey(true), Is.False);
            Assert.That(() => dictionary[false], Throws.TypeOf<KeyNotFoundException>());
            Assert.That(() => dictionary[true], Throws.TypeOf<KeyNotFoundException>());

            dictionary[true] = TrueValue;
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary.Keys.Count, Is.EqualTo(1));
            Assert.That(dictionary.Keys.Contains(false), Is.False);
            Assert.That(dictionary.Keys.Contains(true), Is.True);
            Assert.That(dictionary.Values.Count, Is.EqualTo(1));
            Assert.That(dictionary.Values.Contains(TrueValue), Is.True);
            Assert.That(dictionary.Values.Contains(AnotherTrueValue), Is.False);
            Assert.That(dictionary.Values.Contains(FalseValue), Is.False);
            Assert.That(dictionary.ContainsKey(false), Is.False);
            Assert.That(dictionary.ContainsKey(true), Is.True);
            Assert.That(() => dictionary[false], Throws.TypeOf<KeyNotFoundException>());
            Assert.That(dictionary[true], Is.EqualTo(TrueValue));
        }

        #endregion

        #region BooleanDeterminant Class

        private sealed class BooleanDeterminant : FixedSizeDictionaryDeterminant<bool>
        {
            #region Public Properties

            public override int Size
            {
                get
                {
                    return 2;
                }
            }

            #endregion

            #region Public Methods

            public override int GetIndex(bool key)
            {
                return key ? 1 : 0;
            }

            public override bool GetKey(int index)
            {
                switch (index)
                {
                    case 0:
                        return false;

                    case 1:
                        return true;

                    default:
                        throw new InvalidOperationException();
                }
            }

            #endregion
        }

        #endregion
    }
}