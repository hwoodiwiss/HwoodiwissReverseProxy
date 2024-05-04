namespace HwoodiwissReverseProxy.Tests.Integration.Assertions;

public static class ShouldContainAllAssertion
{
    public static void ShouldContainAll(this string actual, string[] expected)
    {
        foreach (string item in expected)
        {
            actual.ShouldContain(item);
        }
    }

    public static void ShouldContainAll<TItem>(this IEnumerable<TItem> actual, TItem[] expected)
    {
        List<TItem> actualList = actual.ToList();
        foreach (TItem? item in expected)
        {
            actualList.ShouldContain(item);
        }
    }
}
