using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Yi_Jing_App
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

            m_nStepCnt = m_nSubStepCnt = 0;
            m_bEndPredict = false;
            aYaoXiang_BarState = new CYaoXiang_BarState[CSysBase.m_kYao_MaxCnt];
            for (int i = 0; i < CSysBase.m_kYao_MaxCnt; i++)
                aYaoXiang_BarState[i] = new CYaoXiang_BarState();

            m_MouseDwnClientCoord = new Point(0, 0);
            pbxBase1.Image = pbxShi_Step1.Image = pbxShiCao_taiji.Image = null;
            m_nShi_Width = pbxBase1.Size.Width;
#if DEBUG
            lblMouseHoverClientCoord.Visible = tbxMouseHoverClientCoord.Visible = true;
#else
            lblMouseHoverClientCoord.Visible = tbxMouseHoverClientCoord.Visible = false;
#endif
        }


        int m_nShiCao_WorkingTotal, m_nShiCao_WorkingRemain, m_nShi_Width,  //  ShiCao = 蓍草 = stick, 
            m_nShiCao_LeftDiv_TianCe, m_nShiCao_RightDiv_DiCe,      // TianCe = 天策 = haven's prediction, DiCe = 地策 = earth's prediction
            //m_nShiCao_LeftCnt_Remain, m_nShiCao_RightCnt_Remain,
            m_nStepCnt, m_nSubStepCnt;
        bool m_bMouseInRange, m_bEndPredict;
        CYaoXiang_BarState[] aYaoXiang_BarState;
        Point m_MouseHoverClientCoord, m_MouseDwnClientCoord, m_Shi_DivClientCoord;


        private void btnShi_Start_Click(object sender, EventArgs e)     // Shi = 筮 = sort
        {
            m_nShiCao_WorkingRemain = m_nShiCao_WorkingTotal = (CSysBase.m_kShiCao_Total - CSysBase.m_kShiCao_InitExtract_TaiJi );
            pbxBase0.Image = pbxShiCao0.Image = null;

            pbxBase1.Image = Yi_Jing_App.Properties.Resources.shicao_hollowbase;
            pbxShi_Step1.Image = Yi_Jing_App.Properties.Resources.shicao0;
            pbxShiCao_taiji.Image = Yi_Jing_App.Properties.Resources.shicao1_orange;

            m_bMouseInRange = false;
        }

        private string SetCoordTxt(Point iCoord)
        {
            string sCoordTxt = iCoord.X.ToString("00") + ", " + iCoord.Y.ToString("00");
            return sCoordTxt;
        }
        private void pbxShi_Step1_MouseHover(object sender, EventArgs e)    // Shi = 筮 = sort
        {
            m_bMouseInRange = true;
            pbxShi_Step1.Image = imglst_shicao.Images[CImageIndx.HILIGHT_IMAGE];

            PictureBox SticksSelRange = (PictureBox)sender;
            Point TmpMouseCoord = SticksSelRange.PointToClient(Control.MousePosition);
            m_MouseHoverClientCoord = OffsetMouseCoord(TmpMouseCoord);
#if DEBUG
            tbxMouseHoverClientCoord.Text = SetCoordTxt(m_MouseHoverClientCoord);
#endif
        }
        private Point OffsetMouseCoord(Point iMouseRelCoord)
        {
            Point TmpCoord = new Point(iMouseRelCoord.X, iMouseRelCoord.Y);
            int nMouseCoordOffset = (pbxBase1.Width - pbxShi_Step1.Width) / 2;
            TmpCoord.X += nMouseCoordOffset;
            return TmpCoord;
        }
        private void pbxShi_Step1_MouseLeave(object sender, EventArgs e)    // Shi = 筮 = sort
        {
            m_bMouseInRange = false;
            pbxShi_Step1.Image = imglst_shicao.Images[CImageIndx.BASE_IMAGE];
#if DEBUG
            tbxMouseHoverClientCoord.Text = "";
#endif
        }

        private void pbxShi_Step1_MouseDown(object sender, MouseEventArgs e)    // Shi = 筮 = sort
        {
            Point TmpMouseCoord = new Point(e.X, e.Y);
            m_MouseDwnClientCoord = OffsetMouseCoord(TmpMouseCoord);
        }
        private void pbxShi_Step_Click(object sender, EventArgs e)     // Shi = 筮 = sort
        {
            if (m_bEndPredict)
            {
                MessageBox.Show("End of Prediction. Please click [Reset] to start again.");
                return;
            }
            if (!m_bMouseInRange)
            {
                MessageBox.Show("Please click within the highlighted sticks");
                return;
            }

            //MessageBox.Show("click");
            m_Shi_DivClientCoord = m_MouseDwnClientCoord;
            CalcShi_Separation(m_Shi_DivClientCoord, m_nShiCao_WorkingRemain, out m_nShiCao_LeftDiv_TianCe, 
                                out m_nShiCao_RightDiv_DiCe);

            aYaoXiang_BarState[m_nStepCnt].GetBian_SubChange(m_nSubStepCnt, m_nShiCao_LeftDiv_TianCe, 
                                                           m_nShiCao_RightDiv_DiCe, out m_nShiCao_WorkingRemain);
            
            if (++m_nSubStepCnt > 2)
            {
                if (!aYaoXiang_BarState[m_nStepCnt].GetYaoBian_LevelChange())
                {
                    m_bEndPredict = true;
                    MessageBox.Show("Error in calculating Prediction.. Please click [Reset] to start again.");
                    return;
                }

                SetBarState(aYaoXiang_BarState[m_nStepCnt].nYaoXiang_BarState);

                m_nSubStepCnt = 0;
                if (++m_nStepCnt > 5)
                {
                    m_bEndPredict = true;
                    MessageBox.Show("End of Prediction. Please click [Reset] to start again.");
                    return;
                }
            }
        }
        private void SetBarState(int iYaoXiang_BarState)
        {
            switch (iYaoXiang_BarState)
            {
                case CYaoXiang_State.LAO_YANG_CHANGE:
                    SetYao_BarImage(m_nStepCnt, CYao_BarIndx.LAO_YANG);
                    break;
                case CYaoXiang_State.SHAO_YANG_NOCHANGE:
                    SetYao_BarImage(m_nStepCnt, CYao_BarIndx.SHAO_YANG);
                    break;
                case CYaoXiang_State.SHAO_YIN_NOCHANGE:
                    SetYao_BarImage(m_nStepCnt, CYao_BarIndx.SHAO_YIN);
                    break;
                case CYaoXiang_State.LAO_YIN_CHANGE:
                    SetYao_BarImage(m_nStepCnt, CYao_BarIndx.LAO_YIN);
                    break;
            }
        }
        private void SetYao_BarImage(int iStepCnt, int iBarIndx)
        {
            if (iStepCnt == 0)
                pbxYao1.Image = imageList_Yao.Images[iBarIndx];
            else if (iStepCnt == 1)
                pbxYao2.Image = imageList_Yao.Images[iBarIndx];
            else if (iStepCnt == 2)
                pbxYao3.Image = imageList_Yao.Images[iBarIndx];
            else if (iStepCnt == 3)
                pbxYao4.Image = imageList_Yao.Images[iBarIndx];
            else if (iStepCnt == 4)
                pbxYao5.Image = imageList_Yao.Images[iBarIndx];
            else if (iStepCnt == 5)
                pbxYao6.Image = imageList_Yao.Images[iBarIndx];
        }
        private void CalcShi_Separation(Point iDivCoord, int iShiCao_WorkingRemain, out int oShiCao_LeftCnt, out int oShiCao_RightCnt)
        {
            double dDivFraction = ((double)iDivCoord.X / (double)m_nShi_Width);
            
            oShiCao_LeftCnt = (int)(iShiCao_WorkingRemain * dDivFraction);
            oShiCao_RightCnt = (iShiCao_WorkingRemain - oShiCao_LeftCnt);
        }
    }

}
