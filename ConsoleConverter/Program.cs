// See https://aka.ms/new-console-template for more information

var running = true;
var convertor = new UnitConverter.Converter();
Console.WriteLine("Possible commands are:\nq - exits program\n[number] [unit]\\n[unit] - converts value from 1st unit to 2nd");
Console.WriteLine(convertor.GetHelp());
do
{
    Console.WriteLine("Type command:");

    var from = Console.ReadLine();
    if (from == "q")
    {
        running = false;
        continue;
    }
    var to = Console.ReadLine();
    Console.WriteLine(convertor.Convert(from, to));
}
while (running);

Console.WriteLine("Bye!");