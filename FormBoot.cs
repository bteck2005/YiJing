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
    public partial class FormBoot : Form
    {
        public FormBoot()
        {
            InitializeComponent();
            //m_pFormBoot = this;
        }


        //FormBoot m_pFormBoot;

        FormMain m_FormMain;
        private void btnLogin_Click(object sender, EventArgs e)
        {
            m_FormMain = new FormMain();
            this.Hide();
            m_FormMain.Show();
        }
    }
}
