namespace LRv2.Extentions;

public static class StackExtention
{
    public static void PopCount<T>(this Stack<T> stack, int count)
    {
        for(int i = 0; i < count; i++)
        {
            stack.Pop();
        }
    }
}
