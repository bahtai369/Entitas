using System;
using System.Collections.Generic;
using System.Text;

namespace Kent.Entitas
{
    /*
    @brief 配對器
    @note 靜態部分
    */
    public partial class Matcher<T>
    {
        /*
        @brief 旗標緩存
        */
        private static readonly FlagsUtil flagsBuf = new FlagsUtil();

        /*
        @brief 合併來源
        @return 合併後的編號
        */
        private static int Merge(params int[] complexIds)
        {
            flagsBuf.Clear();

            foreach (var id in complexIds)
                flagsBuf.Set(id, true);

            return flagsBuf.Id;
        }

        /*
        @brief 合併來源
        @return 合併後的編號
        */
        private static int Merge(params IMatcher<T>[] matchers)
        {
            flagsBuf.Clear();

            foreach (var matcher in matchers)
                flagsBuf.Set(matcher.MixComplexId, true);

            return flagsBuf.Id;
        }

        /*
        @brief 取全符合比對器
        */
        public static IAllMatcher<T> GetAllMatcher(params int[] complexIds)
        {
            var matcher = new Matcher<T>();
            matcher.AllOf(complexIds);

            return matcher;
        }

        /*
        @brief 取全符合比對器
        */
        public static IAllMatcher<T> GetAllMatcher(params IMatcher<T>[] matchers)
        {
            var matcher = new Matcher<T>();
            matcher.AllOf(matchers);

            return matcher;
        }

        /*
        @brief 取任一符合比對器
        */
        public static IAnyMatcher<T> GetAnyMatcher(params int[] complexIds)
        {
            var matcher = new Matcher<T>();
            matcher.AnyOf(complexIds);

            return matcher;
        }

        /*
        @brief 取任一符合比對器
        */
        public static IAnyMatcher<T> GetAnyMatcher(params IMatcher<T>[] matchers)
        {
            var matcher = new Matcher<T>();
            matcher.AnyOf(matchers);

            return matcher;
        }

        /*
        @brief 取無符合比對器
        */
        public static INoneMatcher<T> GetNoneMatcher(params int[] complexIds)
        {
            var matcher = new Matcher<T>();
            matcher.NoneOf(complexIds);

            return matcher;
        }

        /*
        @brief 取無符合比對器
        */
        public static INoneMatcher<T> GetNoneMatcher(params IMatcher<T>[] matchers)
        {
            var matcher = new Matcher<T>();
            matcher.NoneOf(matchers);

            return matcher;
        }
    }
}
