using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi_Jing_App
{
    public static class CSysBase
    {
        public const int m_kShiCao_Total = 50, m_kYao_MaxCnt = 6;  // Yao = 爻 = levels
        public const int m_kShiCao_InitExtract_TaiJi = 1, 
                         m_kShiCao_RightExtract_Ren = 1,  // TaiJi = 太極 = Singularity， Ren = 人 = human
                         m_kShi_Divider_SiJi = 4, m_kHighNumMark = 8;       // ShiJi = 四季 = four season

    }
    public static class CImageIndx
    {
        public const int BASE_IMAGE = 0;
        public const int HILIGHT_IMAGE = 1;
    }

    public static class CSubChange_State
    {
        public const int YANG_LOW_NUM = 1;
        public const int YIN_HIGH_NUM = 3;
    }
    public static class CYaoXiang_State
    {
        //YANG_LOW_NUM = 1; YIN_HIGH_NUM = 3;
        // YANG_LOW_NUM , YANG_LOW_NUM , YANG_LOW_NUM  = 3 LaoYang
        // YANG_LOW_NUM , YANG_LOW_NUM, YIN_HIGH_NUM  =  5 ShaoYin
        // YANG_LOW_NUM , YIN_HIGH_NUM , YIN_HIGH_NUM = 7 ShaoYang            
        // YIN_HIGH_NUM , YIN_HIGH_NUM , YIN_HIGH_NUM = 9 LaoYin

        public const int LAO_YANG_CHANGE = 3;
        public const int SHAO_YIN_NOCHANGE = 5;

        public const int SHAO_YANG_NOCHANGE = 7;
        public const int LAO_YIN_CHANGE = 9;
    }
    public static class CYao_BarIndx
    {
        public const int LAO_YANG = 0;
        public const int SHAO_YANG = 1;
        public const int SHAO_YIN = 2;
        public const int LAO_YIN = 3;
    }

    class CYaoXiang_BarState
    {
        public CYaoXiang_BarState()
        {
            nSubChange_0 = nSubChange_1 = nSubChange_2 = 0;
        }


        int m_nShiCao_LeftCnt_Remain, m_nShiCao_RightCnt_Remain;
        public int nSubChange_0 { get; private set; }
        public int nSubChange_1 { get; private set; }
        public int nSubChange_2 { get; private set; }

        public int nYaoXiang_BarState { get; private set; }    


        public void GetBian_SubChange(int iSubStepCnt, int iShiCao_LeftDiv, int iShiCao_RightDiv, 
                                        out int oShiCao_WorkingRemain)
        {
            m_nShiCao_LeftCnt_Remain = iShiCao_LeftDiv;
            m_nShiCao_RightCnt_Remain = (iShiCao_RightDiv - CSysBase.m_kShiCao_RightExtract_Ren);
            int nLeftResult_A_Ren = CSysBase.m_kShiCao_RightExtract_Ren;    // Ren = 人 = human

            int nLeftResult_B_RunYue = (m_nShiCao_LeftCnt_Remain % CSysBase.m_kShi_Divider_SiJi);  // RunYue = 閏月
            if (nLeftResult_B_RunYue == 0)
                nLeftResult_B_RunYue = CSysBase.m_kShi_Divider_SiJi;

            int nRightResult_C_RunYue = (m_nShiCao_RightCnt_Remain % CSysBase.m_kShi_Divider_SiJi);
            if (nRightResult_C_RunYue == 0)
                nRightResult_C_RunYue = CSysBase.m_kShi_Divider_SiJi;

            int nSumResult = (nLeftResult_A_Ren + nLeftResult_B_RunYue + nRightResult_C_RunYue);
            oShiCao_WorkingRemain = (iShiCao_LeftDiv + iShiCao_RightDiv) - nSumResult;

            int nSubChange = CSubChange_State.YANG_LOW_NUM;         // 第一爻，一變
            if (nSumResult >= CSysBase.m_kHighNumMark)
                nSubChange = CSubChange_State.YIN_HIGH_NUM;

            if (iSubStepCnt == 0)
                nSubChange_0 = nSubChange;
            else if (iSubStepCnt == 1)
                nSubChange_1 = nSubChange;
            else
                nSubChange_2 = nSubChange;
        }
        public bool GetYaoBian_LevelChange()
        {
            bool bSuccess = false;
            nYaoXiang_BarState = (nSubChange_0 + nSubChange_1 + nSubChange_2);
            switch (nYaoXiang_BarState)
            {
                case CYaoXiang_State.LAO_YANG_CHANGE:
                case CYaoXiang_State.LAO_YIN_CHANGE:
                case CYaoXiang_State.SHAO_YANG_NOCHANGE:
                case CYaoXiang_State.SHAO_YIN_NOCHANGE:
                    bSuccess = true;
                    break;
                default:
                    nYaoXiang_BarState = -1;  // call exception
                    break;
            }
            return bSuccess;
        }

    }
}
