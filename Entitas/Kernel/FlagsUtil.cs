using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace Kent.Entitas
{
    /*
    @brief 旗標工具
    @note 只可容納32個開關(0~31)
    */
    public class FlagsUtil
    {
        /*
        @brief 
        */
        private const int MaxSize = 32;

        /*
        @brief 遮罩
        @note 用來取單一個bit開關
        */
        private static readonly int[] masks = new int[MaxSize];

        /*
        @brief 
        */
        private BitVector32 depend;

        /*
        @brief 
        */
        public int Id { get { return depend.Data; } }

        /*
        @brief 
        @param idx [in] 開關索引0~31
        */
        public bool this[int idx]
        {
            get
            {
                return MaxSize > idx ? depend[masks[idx]] : false;
            }
            set
            {
                if (MaxSize > idx)
                    depend[masks[idx]] = value;
            }
        }

        /*
        @brief 
        */
        static FlagsUtil()
        {
            int temp = 0;

            for (int i = 0; i < MaxSize; i++)
            {
                masks[i] = BitVector32.CreateMask(temp);
                temp = masks[i];
            }
        }

        /*
        @brief 
        */
        public FlagsUtil()
        {
            depend = new BitVector32();
        }

        /*
        @brief 
        @note 會複製開關狀態
        */
        public FlagsUtil(FlagsUtil src)
        {
            depend = new BitVector32(src.depend);
        }

        /*
        @brief 
        @note 會複製開關狀態
        */
        public FlagsUtil(BitVector32 src)
        {
            depend = new BitVector32(src);
        }        

        /*
        @brief 
        @param indices [in] 開關索引 
        @note 可一次設定多個開關索引 
        */
        public FlagsUtil(params int[] indices)
        {
            depend = new BitVector32(MergeIndices(indices));
        }        

        /*
        @brief 清除所有開關
        @param isOpen [in] 會將所有開關都設定成此狀態
        */
        public void Clear(bool isOpen = false)
        {
            depend[int.MaxValue] = isOpen;
        }

        /*
        @brief 同時取多個開關
        @param indices [in] 開關索引 
        @return 要全部都開才會回傳true, 不然就是回傳false
        */
        public bool Get(params int[] indices)
        {
            return depend[MergeIndices(indices)];
        }

        /*
        @brief 同時設定多個開關
        @param value [in] 用二進位轉十進位方式傳入要設定的開關
        */
        public void Set(int value, bool isOpen)
        {
            depend[value] = isOpen;
        }

        /*
        @brief 同時設定多個開關
        @param indices [in] 開關索引 
        */
        public void Set(bool isOpen, params int[] indices)
        {
            depend[MergeIndices(indices)] = isOpen;
        }

        /*
        @brief 合併多個開關索引
        */
        private int MergeIndices(params int[] indices)
        {
            int res = 0;

            foreach (var idx in indices)
            {
                if (idx >= MaxSize)
                    continue;

                res |= masks[idx];
            }

            return res;
        }

        /*
        @brief 
        */
        public override int GetHashCode()
        {
            return Id;
        }

        /*
        @brief 
        */
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;

            return obj.GetHashCode() == GetHashCode();
        }

        /*
        @brief 
        */
        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
