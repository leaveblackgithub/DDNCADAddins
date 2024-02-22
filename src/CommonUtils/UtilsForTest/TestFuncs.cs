using System;
using CommonUtils.Misc;

namespace CommonUtils.UtilsForTest
{
    public class TestFuncs
    {
        private static TestException _testExceptionForCancel;
        private Func<TestCounter, CommandResult>[] _testFuncs;

        public static TestException TestExceptionForCancel => _testExceptionForCancel ??
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

        public  static CommandResult SucessMethod(TestCounter counter)
        {
            return TypicalMethod(counter, false);
        }

        public static  CommandResult CancelMethod(TestCounter counter)
        {
            return TypicalMethod(counter, true);
        }
        public static CommandResult TypicalMethod(TestCounter counter,bool throwException=false)
        {
            var result = new CommandResult();
            try
            {
                counter.Increment();
                if (throwException) throw TestExceptionForCancel;
            }
            catch (Exception e)
            {
                result.Cancel(e);
            }
            return result;
        }

    }
}