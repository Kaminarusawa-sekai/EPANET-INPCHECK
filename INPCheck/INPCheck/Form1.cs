using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Threading;

namespace INPCheck
{
    enum CompareMode
    {
        简单,
        一般,
        复杂
    }


    public partial class Form1 : Form
    {


        string[] array = { };
        BackgroundWorker worker = null;
        string filename1 = "";
        string filename2 = "";

        int selectIndex=0;
        int progressRate = 0;

        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(Enum.GetNames(typeof(CompareMode)));

            comboBox1.SelectedIndex = 1;
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = false;
            worker.WorkerReportsProgress = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged+=new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted+=new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);



        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filename = chooseINP();
            textBox1.Text = filename;   

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string filename = chooseINP();
            textBox2.Text = filename;
        }


        public string chooseINP()
        {
            string file = "";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = "inp文件(*.inp)|*.inp";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                file = dialog.FileName;
            }
            return file;
        }

        private void button3_Click(object sender, EventArgs e)
        {


            filename1 = textBox1.Text;
            filename2 = textBox2.Text;
            selectIndex=comboBox1.SelectedIndex;
            button4.BackColor = Color.Yellow;
            progressBar1.Value = 0;
            label6.Text = "对比中";
            //Form2 form2 = new Form2();
            if (worker.IsBusy)
            {
                return;
            }
            
            this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.button3.Enabled = false;

            worker.RunWorkerAsync();
            Thread thread = new Thread(new ThreadStart(ixQuery));
            thread.Start();
            //form2.ShowDialog();

        }

        void ixQuery()
        {
            while (worker.IsBusy)
            {
                progressRate = INPhelper.ix;
                try
                {
                    worker.ReportProgress(progressRate);
                }catch (Exception ex)
                {
                    break;
                }
                
                Thread.Sleep(10);
            }
            
        }

        Boolean easyCompare(string filename1,string filename2)
        {

            string filePathTXT1 = filename1.Replace("inp", "txt");
            string filePathTXT2 = filename2.Replace("inp", "txt");
            //string resultTXT = filename1.Replace(".inp", "1.txt");
            ArrayList arrayList = new ArrayList();
            Boolean flag=true; 
            
            if (filePathTXT1 == filePathTXT2)
            {
                return false;
            }
            try
            {
                File.Copy(filename1, filePathTXT1);
                File.Copy(filename2, filePathTXT2);
            }catch (Exception ex)
            {
                File.Delete(filePathTXT1);
                File.Delete(filePathTXT2);
                File.Copy(filename1, filePathTXT1);
                File.Copy(filename2, filePathTXT2);
            }
            
            try
            {
                // 创建一个 StreamReader 的实例来读取文件 
                // using 语句也能关闭 StreamReader
                StreamReader sr1 = new StreamReader(filePathTXT1);
                StreamReader sr2 = new StreamReader(filePathTXT2); 

                string line;
                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr1.ReadLine()) != null)
                {
                    if(line.Equals( sr2.ReadLine()))
                    {
                        continue;
                    }
                    else
                    {
                        arrayList.Add(line);

                        flag= false;    
                    }
                }
                sr1.Close();    
                sr2.Close();
            }
            catch (Exception e)
            {
                // 向用户显示出错消息
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            
            File.Delete(filePathTXT1);
            File.Delete(filePathTXT2);
            array = new string[arrayList.Count];
            for (int i = 0; i < arrayList.Count; i++)
            {
                array[i] = arrayList[i].ToString();
            }
            array = new string[1];
            array[0] = "有差异";
            return flag;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            switch (selectIndex)
            {
                case 0:
                    easyCompare(filename1, filename2);
                    break;
                case 1:

                    array = INPhelper.commonCompare(filename1, filename2);
                    break;
                case 2:
                    array = INPhelper.complexCompare(filename1, filename2);
                    break;
            }


        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.button1.Enabled = true;
            this.button2.Enabled = true;
            this.button3.Enabled = true;
            button4.BackColor = Color.Green;
            label6.Text = "已对比";
            progressBar1.Value = 100;
            label7.Text = array.Length.ToString();
            //textBox3.Lines=array;
            string result = "";
            int n = 2000;
            if (array.Length < n)
            {
                n = array.Length;
            }
            for (int i = 0; i < n; i++)
            {
                result = result + (array[i] + "\r\n");

            }
            textBox3.Text = result;
            if(array.Length < 0)
            {
                MessageBox.Show("无差异");
            }
            else
            {
                MessageBox.Show("有差异");
            }
            

        }
    }
}
