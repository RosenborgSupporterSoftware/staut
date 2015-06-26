namespace Teller
{
    class Program
    {
        public static void Main(string[] args)
        {
            var teller = new Teller(new CommandLineOptions(args));
            teller.Run();
        }
    }
}
