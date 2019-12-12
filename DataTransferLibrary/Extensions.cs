using System.Collections.Generic;
using System.Linq;

namespace DataTransferLibrary
{ 
    public static class Extensions
    {
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public static bool AnyNotNull<TSource>(this IEnumerable<TSource> source)
        {   
            return source != null && source.Any();
        }


        public static bool IsCountZero<TSource>(this IEnumerable<TSource> source)
        {
            return source.Count() == 0;
        }


    }
}
