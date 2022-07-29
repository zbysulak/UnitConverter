using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitConverter.Tests
{
    [TestClass()]
    public class ConverterTests
    {
        private Converter _converter;

        public ConverterTests()
        {
            _converter = new Converter();
        }

        [TestMethod("Wrong units")]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertTest_WrongUnits()
        {
            _converter.Convert("1 stuff", "meter");
        }

        [TestMethod("Incompatible types")]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertTest_IncompatibleTypes()
        {
            _converter.Convert("1 meter", "celsius");
        }

        [TestMethod("One base unit per type")]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterConverterTest_SecondBaseUnit()
        {
            _converter.RegisterConverter(new BadKelvinConverter());
        }

        [TestMethod("Add new converter for existing type")]
        public void RegisterConverterTest_NewForExisting()
        {
            _converter.RegisterConverter(new KelvinConverter());

            var result = new ValueUnit(_converter.Convert("1 kelvin", "fahrenheit"));
            Assert.AreEqual(-457.87, result.Value, 0.00001);
        }

        [TestMethod("Negative conversion")]
        public void Converting_Negative()
        {
            TestConversion("-1.11 meter", "feet", -3.64173228);
        }

        [TestMethod("Tests of converting")]
        public void Converting()
        {
            TestConversion("1.11 meter", "feet", 3.64173228);
            TestConversion("333 inch", "feet", 27.75);
            TestConversion("0.8 feet", "inch", 9.6);
            TestConversion("2048 bit", "byte", 256);
            TestConversion("1024 byte", "bit", 8192);
            TestConversion("1024 celsius", "fahrenheit", 1875.2);
            TestConversion("7 fahrenheit", "celsius", -13.8888889);
        }

        private void TestConversion(string from, string to, double expectedResult)
        {
            var result = new ValueUnit(_converter.Convert(from, to));
            Assert.AreEqual(expectedResult, result.Value, 0.00001, $"converting '{from}' -> '{to}'");
        }

        [TestMethod("Volume converter")]
        public void RegisterConverterTest_Volume()
        {
            _converter.RegisterConverter(new LiterConverter());
            _converter.RegisterConverter(new PintConverter());
            _converter.RegisterConverter(new CubicInchConverter());

            var result = new ValueUnit(_converter.Convert("1 liter", "pint"));
            Assert.AreEqual(2.11337642, result.Value, 1e-5, "liter to pint failed");

            result = new ValueUnit(_converter.Convert("1 liter", "cubicinch"));
            Assert.AreEqual(61.0237441, result.Value, 1e-5, "liter to cubicinch failed");

            result = new ValueUnit(_converter.Convert("4 cubicinch", "pint"));
            Assert.AreEqual(0.138528139, result.Value, 1e-5, "cubicinch to pint failed");
        }

        #region Tester converters
        private class KelvinConverter : IUnitConverter
        {
            public virtual bool IsBaseUnit => false;

            public string Type => "temperature";

            public string Name => "kelvin";

            public double ConvertFromBase(double value)
            {
                return value + 273.15;
            }

            public double ConvertToBase(double value)
            {
                return value - 273.15;
            }
        }
        private class BadKelvinConverter : KelvinConverter
        {
            public override bool IsBaseUnit => true;
        }

        private class LiterConverter : BaseUnitConverter
        {
            public override string Type => "volume";
        }

        private class PintConverter : RatioUnitConverter
        {
            public PintConverter() : base(1 / 2.11337642) { }

            public override string Type => "volume";
        }

        private class CubicInchConverter : RatioUnitConverter
        {
            public CubicInchConverter() : base(1 / 61.0237441) { }

            public override string Type => "volume";
        }
        #endregion 
    }
}