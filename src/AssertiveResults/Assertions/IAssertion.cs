using System;
using System.Collections;

namespace AssertiveResults.Assertions
{
    public interface IAssertion
    {
        IAssert Satisfy(bool condition);
        IAssert NotSatisfy(bool condition);
        IAssert Null(object @object);
        IAssert NotNull(object @object);
        IAssert Empty(IEnumerable collection);
        IAssert NotEmpty(IEnumerable collection);
        IAssert Equal<T>(T former, T latter);
        IAssert NotEqual<T>(T former, T latter);
        IAssert StrictEqual<T>(IComparable<T> former, T latter);
        IAssert NotStrictEqual<T>(IComparable<T> former, T latter);
        IAssert Same(object former, object latter);
        IAssert NotSame(object former, object latter);
    }
}