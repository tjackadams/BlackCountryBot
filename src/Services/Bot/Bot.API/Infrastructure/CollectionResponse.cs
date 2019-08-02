using System.Collections.Generic;

namespace Bot.API.Infrastructure
{
    public class CollectionResponse
    {
        public CollectionResponse(IEnumerable<object> data)
        {
            Data = data;
        }

        public IEnumerable<object> Data { get; }
    }
}
