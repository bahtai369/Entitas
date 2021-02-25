using System;
using System.Collections.Generic;
using System.Text;

namespace Kent.Entitas
{
    /*
    @brief 除錯資訊
    */
    public class DebugInfo
    {
        /*
        @brief 管理器名稱
        */
        public readonly string MgrName;

        /*
        @brief 組件名稱
        */
        public readonly string[] ComponentNames;

        /*
        @brief 
        */
        public DebugInfo(string mgrName, string[] componentNames)
        {
            MgrName = mgrName;
            ComponentNames = componentNames;
        }
    }
}
