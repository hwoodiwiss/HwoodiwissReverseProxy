namespace HwoodiwissReverseProxy.Tests.Integration.Assertions;

public static class ShouldContainAllAssertion
{
    public static void ShouldContainAll(this string actual, string[] expected)
    {
        foreach (var item in expected)
        {
            actual.ShouldContain(item);
        }
    }
    
    public static void ShouldContainAll<TItem>(this IEnumerable<TItem> actual, TItem[] expected)
    {
        var actualList = actual.ToList();
        foreach (var item in expected)
        {
            actualList.ShouldContain(item);
        }
    }
}
