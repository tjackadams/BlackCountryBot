using System;
using BlackCountryBot.Worker;

namespace BlackCountryBot.Tweet.Container
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string buffer = Console.In.ReadToEnd();
            var f = new FunctionHandler();

            string responseValue = f.Handle(buffer);

            if (responseValue != null)
            {
                Console.Write(responseValue);
            }
        }
    }
}
