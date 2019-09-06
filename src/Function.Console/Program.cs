namespace Function.Console
{
    internal class Program
    {
        private static void Main(string[] _)
        {
            string buffer = "hi";
            var f = new FunctionHandler();

            string responseValue = f.Handle(buffer);

            if (responseValue != null)
            {
                System.Console.Write(responseValue);
            }
        }
    }
}
