using System;
using CommonUtils.Misc;

namespace CommonUtils.UtilsForTest
{
    public class TestFuncs
    {
        private TestException _testExceptionForCancel;
        private Func<TestCounter, CommandResult>[] _testFuncs;

        public TestException TestExceptionForCancel => _testExceptionForCancel ??
                                                       (_testExceptionForCancel =
                                                           new TestException("Test Exception for Counter"));

        public Func<TestCounter, CommandResult>[] _ => _testFuncs ?? (_testFuncs =
            new Func<TestCounter, CommandResult>[]
            {
                SucessMethod,
                CancelMethod,
                SucessMethod,
                CancelMethod
            });

        public CommandResult SucessMethod(TestCounter counter)
        {
            counter.Increment();
            return new CommandResult();
        }

        public CommandResult CancelMethod(TestCounter counter)
        {
            counter.Increment();
            return new CommandResult().Cancel(TestExceptionForCancel);
        }
    }
}