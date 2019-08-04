using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Xunit;

namespace BlackCountryBot.IntegrationTests
{
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        private static readonly AsyncLock Mutex = new AsyncLock();

        private static bool _initialized;

        public virtual async Task InitializeAsync()
        {
            if (_initialized)
                return;

            using (await Mutex.LockAsync())
            {
                if (_initialized)
                    return;

                await SliceFixture.ResetCheckpoint();

                _initialized = true;
            }
        }

        public virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

       private  const string LoremIpsum = "lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut " +
            "labore et dolore magna aliqua ut enim ad minim veniam quis nostrud exercitation ullamco laboris nisi ut " +
            "aliquip ex ea commodo consequat duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore " +
            "eu fugiat nulla pariatur excepteur sint occaecat cupidatat non proident sunt in culpa qui officia deserunt ";

        public string GetString(int wordCount)
        {
            var loremIndex = 0;
            var splitLoremIpsum = LoremIpsum.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var startIndex = loremIndex + wordCount > splitLoremIpsum.Length ? 0 : loremIndex;
            loremIndex = startIndex + wordCount;
            return string.Join(" " , splitLoremIpsum.AsSpan().Slice(startIndex, loremIndex).ToArray());
        }
    }
}
