namespace Function.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string buffer = "hi";
            FunctionHandler f = new FunctionHandler();

            string responseValue = f.Handle(buffer);

            if (responseValue != null)
            {
                System.Console.Write(responseValue);
            }
        }
    }
}
