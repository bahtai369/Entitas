using System;
using Kent.Entitas;

namespace Host
{
    /*
    @brief 
    */
    internal static class Program
    {
        /*
        @brief
        */
        static void Main(string[] args)
        {
            var mgr = new DemoMgr();
            DemoSystem.Inst.SetMgr(mgr);
            SystemsSet.Inst.AddSystem(DemoSystem.Inst);

            SystemsSet.Inst.Init();

            //SystemsSet.Inst.Update();

            SystemsSet.Inst.Free();
        }
    }
}
