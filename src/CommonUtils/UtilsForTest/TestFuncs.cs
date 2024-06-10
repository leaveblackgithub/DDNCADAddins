using System;
using CommonUtils.Misc;

namespace CommonUtils.UtilsForTest
{
    public class TestFuncs
    {
        private static TestException _testExceptionForCancel;
        private Func<TestCounter, FuncResult>[] _testFuncs;

        public static TestException TestExceptionForCancel => _testExceptionForCancel ??
                                                              (_testExceptionForCancel =
                                                                  new TestException("Test Exception for Counter"));

        public Func<TestCounter, FuncResult>[] _ => _testFuncs ?? (_testFuncs =
            new Func<TestCounter, FuncResult>[]
            {
                SucessMethod,
                CancelMethod,
                SucessMethod,
                CancelMethod
            });

        public static FuncResult SucessMethod(TestCounter counter)
        {
            return TypicalMethod(counter);
        }

        public static FuncResult CancelMethod(TestCounter counter)
        {
            return TypicalMethod(counter, true);
        }

        public static FuncResult TypicalMethod(TestCounter counter, bool throwException = false)
        {
            var result = new FuncResult();
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