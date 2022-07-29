using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitConverter
{

    /// <summary>
    /// Structure containing value and unit
    /// It is public only for testing purposes!
    /// </summary>
    public struct ValueUnit
    {
        public double Value;
        public string Unit;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <exception cref="ArgumentException"></exception>
        public ValueUnit(string str)
        {
            var parts = str.Split(" ");
            if (parts.Length != 2) throw new ArgumentException("Argument has to have 2 parts");
            this.Unit = parts[1];

            if (double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
            {
                this.Value = val;
            }
            else
            {
                throw new ArgumentException($"1st part is not double ('{parts[0]}')");
            }
        }

        public override string? ToString()
        {
            return $"{Value.ToString(CultureInfo.InvariantCulture)} {Unit}";
        }
    }

    public class Converter
    {
        private IList<IUnitConverter> converters;

        private Prefixes prefixes;

        public Converter()
        {
            prefixes = new Prefixes();
            converters = new List<IUnitConverter>();

            this.RegisterConverter(new MeterConverter());
            this.RegisterConverter(new FeetConverter());
            this.RegisterConverter(new InchConverter());
            this.RegisterConverter(new BitConverter());
            this.RegisterConverter(new ByteConverter());
            this.RegisterConverter(new CelsiusConverter());
            this.RegisterConverter(new FahrenheitConverter());
        }
        public string Convert(string from, string to)
        {
            from = from.Trim().ToLower();
            to = to.Trim().ToLower();
            var v = prefixes.Normalize(new ValueUnit(from));
            var baseUnitTo = prefixes.GetBaseUnit(to);
            if (v.Unit == to) return v.ToString();
            var c1 = converters.SingleOrDefault(c => c.Name == v.Unit);
            var c2 = converters.SingleOrDefault(c => c.Name == baseUnitTo);
            if (c1 is null) throw new ArgumentException($"Converter for unit '{v.Unit}' was not found");
            if (c2 is null) throw new ArgumentException($"Converter for unit '{baseUnitTo}' was not found");
            if (c1.Type != c2.Type) throw new ArgumentException("Units are not of same type");

            var convertedValue = c2.ConvertFromBase(c1.ConvertToBase(v.Value));
            var result = prefixes.Denormalize(convertedValue, to);
            return result.ToString();
        }

        public void RegisterConverter(IUnitConverter converter)
        {
            // check for duplicity
            if (converters.Any(c => c.Name == converter.Name))
                throw new ArgumentException($"Converter with name '{converter.Name}' is already registered");
            // check if base exists for this unit
            if (!converter.IsBaseUnit && !converters.Any(c => c.Type == converter.Type && c.IsBaseUnit))
                throw new ArgumentException($"You have to register base unit converter of type '{converter.Type}' first");
            if (converter.IsBaseUnit && converters.Any(c => c.Type == converter.Type && c.IsBaseUnit))
                throw new ArgumentException($"You can only register one base unit per type");
            converters.Add(converter);
        }

        public string GetHelp()
        {
            var sb = new StringBuilder("Registered converters:\n");
            foreach (var g in converters.GroupBy(c => c.Type))
            {
                sb.Append($"{g.Key}:\n");
                foreach (var c in g.OrderByDescending(cc => cc.IsBaseUnit))
                {
                    sb.Append($"\t{c.Name}{(c.IsBaseUnit ? " (base)" : "")}\n");
                }
            }
            return sb.ToString();
        }
    }

    public interface IUnitConverter
    {
        bool IsBaseUnit { get; }
        string Type { get; }
        string Name { get; }
        double ConvertToBase(double value);
        double ConvertFromBase(double value);
    }

    #region Converters

    #region Length
    internal class MeterConverter : BaseUnitConverter
    {
        public override string Type => "length";
    }

    internal class InchConverter : RatioUnitConverter
    {
        public InchConverter() : base(0.0254) { }

        public override string Type => "length";
    }

    internal class FeetConverter : RatioUnitConverter
    {
        public FeetConverter() : base(0.3048) { }

        public override string Type => "length";
    }
    #endregion

    #region Data units
    internal class BitConverter : BaseUnitConverter
    {
        public override string Type => "data";
    }

    internal class ByteConverter : RatioUnitConverter
    {
        public ByteConverter() : base(8) { }

        public override string Type => "data";
    }
    #endregion

    #region Temperature
    internal class CelsiusConverter : BaseUnitConverter
    {
        public override string Type => "temperature";
    }

    internal class FahrenheitConverter : IUnitConverter
    {
        public FahrenheitConverter() { }

        public string Type => "temperature";

        public bool IsBaseUnit => false;

        public string Name => "fahrenheit";

        public double ConvertFromBase(double value)
        {
            return value * 1.8 + 32;
        }

        public double ConvertToBase(double value)
        {
            return (value - 32) * 5 / 9;
        }
    }
    #endregion

    public abstract class BaseUnitConverter : IUnitConverter
    {
        public abstract string Type { get; }
        public string Name => this.GetType().Name[..^("Converter".Length)].ToLower();

        public bool IsBaseUnit => true;

        public double ConvertFromBase(double value)
        {
            return value;
        }

        public double ConvertToBase(double value)
        {
            return value;
        }
    }

    public abstract class RatioUnitConverter : IUnitConverter
    {
        private readonly double baseRatio;

        protected RatioUnitConverter(double baseRatio)
        {
            this.baseRatio = baseRatio;
        }

        public abstract string Type { get; }
        public string Name => this.GetType().Name[..^("Converter".Length)].ToLower();

        public bool IsBaseUnit => false;

        public double ConvertFromBase(double value)
        {
            return value / baseRatio;
        }

        public double ConvertToBase(double value)
        {
            return baseRatio * value;
        }
    }

    #endregion
}
