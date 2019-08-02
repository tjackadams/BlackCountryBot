using System;

namespace Bot.FunctionalTests.Infrastructure.Helpers
{
    public static class ContentHelpers
    {
        private const string LoremIpsum = "lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut " +
     "labore et dolore magna aliqua ut enim ad minim veniam quis nostrud exercitation ullamco laboris nisi ut " +
     "aliquip ex ea commodo consequat duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore " +
     "eu fugiat nulla pariatur excepteur sint occaecat cupidatat non proident sunt in culpa qui officia deserunt ";

        public static string GetWords()
        {
            int max = LoremIpsum.Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;

            return GetWords(max);
        }
        public static string GetWords(int wordCount)
        {
            int loremIndex = 0;
            string[] splitLoremIpsum = LoremIpsum.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            int startIndex = loremIndex + wordCount > splitLoremIpsum.Length ? 0 : loremIndex;
            loremIndex = startIndex + wordCount;
            return string.Join(" ", splitLoremIpsum.AsSpan().Slice(startIndex, loremIndex).ToArray());
        }
    }
}
