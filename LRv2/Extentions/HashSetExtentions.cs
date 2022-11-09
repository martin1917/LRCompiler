namespace LRv2.Extentions;

public static class HashSetExtentions
{
    public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            hashSet.Add(item);
        }
    }
}
