using System;
using AssertiveResults.Assertions;

namespace AssertiveResults.Contracts
{
    public interface IBreak
    {
        IResult Assert(Action<IAssertion> assert);
    }
}