using System;
using System.Collections.Generic;
using System.Text;

namespace Kent.Entitas
{
    /*
    @brief 實體群組配對器
    */
    public interface IMatcher<T> where T : IEntity
    {
        /*
        @brief 組件混和編號
        @note 會將另外三種混和編號的列表合併, 並去除重複的
        */
        int MixComplexId { get; }

        /*
        @brief 實體是否符合此配對器
        */
        bool IsEntityMatch(T entity);
    }

    /*
    @brief 組件配對器
    */
    public interface IComponentMatcher<T> : IMatcher<T> where T : IEntity
    {
        /*
        @brief 組件混和編號
        @note 用來判斷是否全部組件符合
        */
        int AllComplexId { get; }

        /*
        @brief 組件混和編號
        @note 用來判斷是否任一組件符合
        */
        int AnyComplexId { get; }

        /*
        @brief 組件混和編號
        @note 用來判斷是否無部組件符合
        */
        int NoneComplexId { get; }
    }

    /*
    @brief 無組件符合比對器
    */
    public interface INoneMatcher<T> : IComponentMatcher<T> where T : IEntity
    {
        // TODO
    }

    /*
    @brief 任一組件符合比對器
    */
    public interface IAnyMatcher<T> : INoneMatcher<T> where T : IEntity
    {
        // TODO
    }

    /*
    @brief 全部組件符合比對器
    */
    public interface IAllMatcher<T> : IAnyMatcher<T> where T : IEntity
    {
        // TODO
    }
}
