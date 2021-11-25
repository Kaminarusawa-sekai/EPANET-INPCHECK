using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace INPCheck
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

        }

        public void stateChange(string s)
        {
            if (s=="1")
            {
                label1.Text = "正确";
            }
            else
            {
                label1.Text = s;    
            }
        }
        
    }
}
