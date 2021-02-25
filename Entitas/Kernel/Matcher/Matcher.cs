using System;
using System.Collections.Generic;
using System.Text;

namespace Kent.Entitas
{
    /*
    @brief 配對器
    @note 主體
    */
    public partial class Matcher<T> : IAllMatcher<T> where T : IEntity
    {
        /*
        @brief 全符合旗標
        */
        private readonly FlagsUtil allFlags = new FlagsUtil();

        /*
        @brief 組件混和編號
        @note 用來判斷是否全部組件符合
        */        
        public int AllComplexId { get { return allFlags.Id; } }

        /*
        @brief 任一符合旗標
        */
        private readonly FlagsUtil anyFlags = new FlagsUtil();

        /*
        @brief 組件混和編號
        @note 用來判斷是否任一組件符合
        */
        public int AnyComplexId { get { return anyFlags.Id; } }

        /*
        @brief 無符合旗標
        */
        private readonly FlagsUtil noneFlags = new FlagsUtil();

        /*
        @brief 組件混和編號
        @note 用來判斷是否無部組件符合
        */
        public int NoneComplexId { get { return noneFlags.Id; } }

        /*
        @brief 混和旗標
        */
        private readonly FlagsUtil mixFlags = new FlagsUtil();

        /*
        @brief 組件混和編號
        @note 會將另外三種混和編號的列表合併, 並去除重複的
        */
        public int MixComplexId
        {
            get
            {
                mixFlags.Clear();
                mixFlags.Set(GetHashCode(), true);

                return mixFlags.Id;
            }
        }

        /*
        @brief 取全符合
        */
        public INoneMatcher<T> AllOf(params int[] complexIds)
        {
            allFlags.Clear();
            allFlags.Set(Merge(complexIds), true);
            isHashChanged = true;

            return this;
        }

        /*
        @brief 取全符合
        */
        public INoneMatcher<T> AllOf(params IMatcher<T>[] matchers)
        {
            allFlags.Clear();
            allFlags.Set(Merge(matchers), true);
            isHashChanged = true;

            return this;
        }

        /*
        @brief 取任一符合
        */
        public INoneMatcher<T> AnyOf(params int[] complexIds)
        {
            anyFlags.Clear();
            anyFlags.Set(Merge(complexIds), true);
            isHashChanged = true;

            return this;
        }

        /*
        @brief 取任一符合
        */
        public INoneMatcher<T> AnyOf(params IMatcher<T>[] matchers)
        {
            anyFlags.Clear();
            anyFlags.Set(Merge(matchers), true);
            isHashChanged = true;

            return this;
        }

        /*
        @brief 取無符合
        */
        public IComponentMatcher<T> NoneOf(params int[] complexIds)
        {
            noneFlags.Clear();
            noneFlags.Set(Merge(complexIds), true);
            isHashChanged = true;

            return this;
        }

        /*
        @brief 取無符合
        */
        public IComponentMatcher<T> NoneOf(params IMatcher<T>[] matchers)
        {
            noneFlags.Clear();
            noneFlags.Set(Merge(matchers), true);
            isHashChanged = true;

            return this;
        }

        /*
        @brief 實體是否符合此配對器
        */
        public bool IsEntityMatch(T entity)
        {
            return (AllComplexId == 0 || entity.HasAllComponents(AllComplexId)) &&
                (AnyComplexId == 0 || entity.HasAnyComponents(AnyComplexId)) &&
                (NoneComplexId == 0 || entity.HasAnyComponents(NoneComplexId) == false);
        }
    }
}
