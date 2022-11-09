namespace LRv2.Extentions;

public static class ArrayExtentions
{
    public static IEnumerable<int> IndexesOf<T>(this T[] array, T item)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(item))
            {
                yield return i;
            }
        }
    }
}
