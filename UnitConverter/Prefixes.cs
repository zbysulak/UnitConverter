namespace UnitConverter
{

    public class Prefixes
    {
        private struct Prefix
        {
            public string Name;
            public double Value;

            public Prefix(string name, double value)
            {
                Name = name;
                Value = value;
            }
        }

        private readonly Prefix[] prefixes = new[] {
                new Prefix("nano",  1e-12),
                new Prefix("micro", 1e-6),
                new Prefix("mili",  1e-3),
                new Prefix("centi", 1e-2),
                new Prefix("deci",  1e-1),
                new Prefix("deca",  1e1),
                new Prefix("hecto", 1e2),
                new Prefix("kilo",  1e3),
                new Prefix("mega",  1e6),
                new Prefix("giga",  1e9),
                new Prefix("tera",  1e12)
            };

        private Prefix? FindPrefix(string name)
        {
            var p = prefixes.SingleOrDefault(p => name.StartsWith(p.Name));
            if (p.Name == null) return null;
            return p;
        }

        public string GetBaseUnit(string unit)
        {
            Prefix? prefix = FindPrefix(unit);
            if (prefix is null) return unit;
            return unit[prefix.Value.Name.Length..]; // removes first n characters, where n is lenght of prefix name
        }

        public ValueUnit Normalize(ValueUnit value)
        {
            Prefix? prefix = FindPrefix(value.Unit);
            if (prefix is null) return value;
            var baseUnit = value.Unit[(prefix.Value.Name.Length)..]; // removes first n characters, where n is lenght of prefix name
            return new ValueUnit
            {
                Unit = baseUnit,
                Value = value.Value * prefix.Value.Value
            };
        }

        public ValueUnit Denormalize(double value, string to)
        {
            Prefix? prefix = FindPrefix(to);
            if (prefix is null) return new ValueUnit { Value = value, Unit = to };
            return new ValueUnit
            {
                Unit = to,
                Value = value / prefix.Value.Value
            };
        }
    }
}