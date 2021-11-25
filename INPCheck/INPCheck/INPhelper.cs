using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Collections;
using System.IO;


namespace INPCheck
{
    enum JUNCTION
    {
        节点ID,
        节点高程,
        节点需水量,
        节点水量模式
    }

    enum RESERVOIR
    {
        ID,
        水库水头,
        水库水量模式
    }

    enum TANK
    {
        ID,
        水池高程,
        初始液位,
        最低液位,
        最高液位,
        直径,
        最小容积,
        容积曲线
    }

    enum PIPE
    {
        ID,
        起始节点,
        终止节点,
        长度,
        口径,
        粗糙度,
        水损,
        状态

    }

    enum PUMP
    {
        ID,

        起始节点,
        终止节点,
        曲线
    }

    enum VALVE
    {
        ID,
        起始节点, 终止节点, 口径, 形式, 开度, 水损
    }

    enum TAGS
    {

    }

    enum DEMANDS
    {

    }

    enum LEAKAGES
    {

    }

    enum STATUS
    {

    }

    enum CURVES
    {


    }

    enum CONTROLS
    {

    }

    enum ENERGY
    {

    }

    enum EMITTERS
    {

    }

    enum QUALITY
    {

    }

    enum SOURCES
    {

    }

    enum REACTIONS
    {

    }

    enum MIXING
    {

    }

    enum TIMES
    {

    }

    enum REPORT
    {

    }

    enum OPTIONS
    {

    }

    enum COORDINATES
    {

    }

    enum VERTICES
    {

    }


    internal class INPhelper
    {
        public static string tempFIlePath = "//temp//";

        public static ArrayList OpenINP(string filePath)
        {

            string filePathCSV = filePath.Replace("inp", "csv");
            try
            {
                File.Delete(filePathCSV);
            }
            catch (Exception ex)
            {

            }
            File.Copy(filePath, filePathCSV);
            ArrayList dataTableList = new ArrayList();
            System.Text.Encoding encoding = GetType(filePathCSV);


            System.IO.FileStream fs = new System.IO.FileStream(filePathCSV, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.StreamReader sr = new System.IO.StreamReader(fs, encoding);

            string strLine = "";
            string[] aryLine = null;
            string[] tableHead = null;

            int columncount = 0;
            while ((strLine = sr.ReadLine()) != null)
            {
                if (strLine == "" || strLine == null)
                {
                    continue;
                }
                if (strLine.Substring(0, 1) == "[")
                {
                    DataTable dataTable = new DataTable();
                    dataTable.TableName = strLine;
                    dataTableList.Add(dataTable);
                    strLine = sr.ReadLine();
                    if (strLine == null)
                    {
                        break;
                    }
                    if (strLine == "")
                    {
                        while ((strLine = sr.ReadLine()) == "")
                        {

                        }
                    }

                    tableHead = strLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    columncount = tableHead.Length;
                    if (dataTable.TableName == "[PATTERNS]" || dataTable.TableName == "[OPTIONS]" || dataTable.TableName == "[TIMES]")
                    {
                        columncount = 1;
                        DataColumn dc = new DataColumn(strLine);
                        dataTable.Columns.Add(dc);
                        continue;
                    }
                    for (int i = 0; i < columncount; i++)
                    {

                        DataColumn dc = new DataColumn(tableHead[i]);
                        dataTable.Columns.Add(dc);

                    }
                    try
                    {
                        int idIndex = dataTable.Columns.IndexOf(";ID");
                        if (idIndex != -1)
                        {
                            dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns[0] };
                        }

                    }
                    catch (Exception)
                    {

                        continue;
                    }


                }
                else
                {
                    aryLine = strLine.Split(',');
                    DataTable dataTable = (DataTable)dataTableList[dataTableList.Count - 1];
                    DataRow dataRow = dataTable.NewRow();
                    if (dataTable.TableName == "[PATTERNS]" || dataTable.TableName == "[OPTIONS]" || dataTable.TableName == "[TIMES]")
                    {
                        dataRow[0] = aryLine[0];
                        dataTable.Rows.Add(dataRow);
                        continue;
                    }
                    string[] aryLineS = aryLine[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < columncount; j++)
                    {
                        try
                        {
                            dataRow[j] = aryLineS[j];
                        }
                        catch (System.IndexOutOfRangeException e)
                        {

                            dataRow[j] = null;
                        }


                    }
                    dataTable.Rows.Add(dataRow);
                }


            }

            /*
            if(aryLine!=null&& aryLine.Length > 0)
            {
                dt.DefaultView.Sort = tableHead[0] + " " + "asc";
            }
            */
            sr.Close();
            fs.Close();
            File.Delete(filePathCSV);
            return dataTableList;
        }

        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
        /// <param name="FILE_NAME">文件路径</param>
        /// <returns>文件的编码类型</returns>

        public static System.Text.Encoding GetType(string FILE_NAME)
        {
            System.IO.FileStream fs = new System.IO.FileStream(FILE_NAME, System.IO.FileMode.Open,
            System.IO.FileAccess.Read);
            System.Text.Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// 通过给定的文件流，判断文件的编码类型
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        public static System.Text.Encoding GetType(System.IO.FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            System.Text.Encoding reVal = System.Text.Encoding.Default;

            System.IO.BinaryReader r = new System.IO.BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = System.Text.Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = System.Text.Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = System.Text.Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }

        /// 判断是否是不带 BOM 的 UTF8 格式
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;  //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }


        public static DataRow searchRow(ArrayList dataTableArrayList, string name)
        {
            DataRow resultDataRow = null;
            foreach (var item in dataTableArrayList)
            {
                DataTable dt = (DataTable)item;

                try
                {
                    DataRow dr = dt.Rows.Find(name);
                    if (dr != null)
                    {
                        resultDataRow = dr;

                        break;
                    }
                }
                catch (System.Data.MissingPrimaryKeyException e)
                {

                    continue;
                }

            }
            return resultDataRow;
        }

        public static bool setValue(ArrayList dataTableArrayList, string name, int colIndex, float value)
        {
            bool flag = false;
            DataRow resultDataRow = null;
            foreach (var item in dataTableArrayList)
            {
                DataTable dt = (DataTable)item;

                try
                {
                    DataRow dr = dt.Rows.Find(name);
                    if (dr != null)
                    {
                        resultDataRow = dr;
                        resultDataRow[colIndex] = value;
                        flag = true;
                        break;
                    }
                }
                catch (System.Data.MissingPrimaryKeyException e)
                {

                    continue;
                }

            }
            return flag;
        }

        public static bool setValue(DataRow dataRow, int colIndex, float value)
        {
            bool flag = false;
            try
            {
                dataRow[colIndex] = value;
                flag = true;
            }
            catch (Exception e)
            {

            }
            return flag;
        }



        public static bool createINP(string filePath, ArrayList dataTableArrayList)
        {
            bool flag = false;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            string filePathCSV = filePath.Replace("inp", "csv");
            for (int i = 0; i < dataTableArrayList.Count; i++)
            {
                DataTable dataTable = (DataTable)dataTableArrayList[i];
                SaveCSV(dataTable, filePathCSV);
            }
            File.Move(filePathCSV, filePath);
            return flag;
        }

        public static void SaveCSV(DataTable dt, string fullPath)//table数据写入csv
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(fullPath);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            System.IO.FileStream fs = new System.IO.FileStream(fullPath, System.IO.FileMode.Append, System.IO.FileAccess.Write);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding("GB2312"));
            string data = "";

            sw.WriteLine(dt.TableName);

            for (int i = 0; i < dt.Columns.Count; i++)//写入列名
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += " ";
                }
            }
            sw.WriteLine(data);

            for (int i = 0; i < dt.Rows.Count; i++) //写入各行数据
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string str = dt.Rows[i][j].ToString();
                    str = str.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                    if (str.Contains(',') || str.Contains('"')
                        || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
                    {
                        str = string.Format("\"{0}\"", str);
                    }

                    data += str;
                    if (j < dt.Columns.Count - 1)
                    {
                        data += " ";
                    }

                }
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
        }

        public static int ix = 0;
        public static string[] commonCompare(string filename1, string filename2)
        {
            ArrayList dataTableList1 = OpenINP(filename1);
            ix = 30;
            ArrayList dataTableList2 = OpenINP(filename2);
            ix = 60;
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < dataTableList1.Count; i++)
            {
                ix = (i + 1) * 40 / dataTableList1.Count + 60;
                DataTable dt1 = (DataTable)dataTableList1[i];
                if (dt1.TableName == "[PATTERNS]" || dt1.TableName == "[OPTIONS]" || dt1.TableName == "[TIMES]")
                {
                    continue;
                }
                DataTable dt2 = (DataTable)dataTableList2[i];
                DataRowCollection dataRowCollection1 = dt1.Rows;
                DataRowCollection dataRowCollection2 = dt2.Rows;
                for (int j = 0; j < dataRowCollection1.Count; j++)
                {
                    try
                    {
                        //a = (dataRowCollection1[j] != dataRowCollection2[j]);


                        object[] temp1 = dataRowCollection1[j].ItemArray;
                        object[] temp2 = dataRowCollection2[j].ItemArray;

                        for (int k = 0; k < temp1.Length; k++)
                        {
                            if (temp1[k].GetType() == typeof(DBNull))
                            {
                                temp1[k] = "NULL";
                            }
                            if (temp2[k].GetType() == typeof(DBNull))
                            {
                                temp2[k] = "NULL";
                            }

                            if (((string)temp1[k]).Equals((string)temp2[k]))
                            {
                                continue;
                            }
                            else
                            {
                                arrayList.Add("ID" + ":" + (string)temp1[0] + "->" + (string)dt1.Columns[k].ToString() + ":" + (string)temp1[k] + " DIFFRENT TO " + "ID" + ":" + (string)temp2[0] + "->" + (string)dt1.Columns[k].ToString() + ":" + (string)temp2[k] + " in " + dataRowCollection1[j].Table.TableName);

                            }
                        }

                        continue;

                    }
                    catch (InvalidCastException e)
                    {
                        continue;
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            string temp = "";
                            object[] t = dataRowCollection1[j].ItemArray;
                            foreach (object o in t)
                            {
                                temp = temp + o.ToString() + " ";
                            }
                            arrayList.Add(temp + " in " + dataRowCollection1[j].Table.TableName);
                            continue;

                        }
                        catch (Exception ex)
                        {
                            string temp = "";
                            object[] t = dataRowCollection2[j].ItemArray;
                            foreach (object o in t)
                            {
                                temp = temp + o.ToString() + " ";
                            }
                            arrayList.Add(temp + " in " + dataRowCollection2[j].Table.TableName);
                            continue;
                        }

                    }

                }
            }
            string[] array = new string[arrayList.Count];
            for (int i = 0; i < arrayList.Count; i++)
            {
                array[i] = arrayList[i].ToString();
            }
            return array;
        }

        public static string[] complexCompare(string filename1, string filename2)
        {
            ArrayList dataTableList1 = OpenINP(filename1);
            ix = 30;
            ArrayList dataTableList2 = OpenINP(filename2);
            ix = 60;
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < dataTableList1.Count; i++)
            {
                ix = (i + 1) * 40 / dataTableList1.Count + 60;
                DataTable dt1 = (DataTable)dataTableList1[i];
                if (dt1.TableName == "[PATTERNS]" || dt1.TableName == "[OPTIONS]" || dt1.TableName == "[TIMES]")
                {
                    continue;
                }
                DataTable dt2 = (DataTable)dataTableList2[i];
                DataRowCollection dataRowCollection1 = dt1.Rows;
                DataRowCollection dataRowCollection2 = dt2.Rows;
                for (int j = 0; j < dataRowCollection1.Count; j++)
                {
                    try
                    {
                        //a = (dataRowCollection1[j] != dataRowCollection2[j]);


                        object[] temp1 = dataRowCollection1[j].ItemArray;
                        DataRow dataRow = searchRow(dataTableList2, (string)temp1[1]);

                        if (dataRow == null)
                        {
                            string temp = "";
                            object[] t = dataRowCollection1[j].ItemArray;
                            foreach (object o in t)
                            {
                                temp = temp + o.ToString() + " ";
                            }
                            arrayList.Add(temp + " in " + dataRowCollection1[j].Table.TableName);
                            continue;
                        }
                        else
                        {
                            object[] temp2 = dataRow.ItemArray;
                            for (int k = 0; k < temp1.Length; k++)
                            {
                                if (temp1[k].GetType() == typeof(DBNull))
                                {
                                    temp1[k] = "NULL";
                                }
                                if (temp2[k].GetType() == typeof(DBNull))
                                {
                                    temp2[k] = "NULL";
                                }

                                if (((string)temp1[k]).Equals((string)temp2[k]))
                                {
                                    continue;
                                }
                                else
                                {
                                    arrayList.Add("ID" + ":" + (string)temp1[0] + "->" + (string)dt1.Columns[k].ToString() + ":" + (string)temp1[k] + " DIFFRENT TO " + "ID" + ":" + (string)temp2[0] + "->" + (string)dt1.Columns[k].ToString() + ":" + (string)temp2[k] + " in " + dataRowCollection1[j].Table.TableName);

                                }
                            }

                            continue;

                        }
                    }
                    catch (InvalidCastException e)
                    {
                        continue;
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            string temp = "";
                            object[] t = dataRowCollection1[j].ItemArray;
                            foreach (object o in t)
                            {
                                temp = temp + o.ToString() + " ";
                            }
                            arrayList.Add(temp + " in " + dataRowCollection1[j].Table.TableName);
                            continue;

                        }
                        catch (Exception ex)
                        {
                            string temp = "";
                            object[] t = dataRowCollection2[j].ItemArray;
                            foreach (object o in t)
                            {
                                temp = temp + o.ToString() + " ";
                            }
                            arrayList.Add(temp + " in " + dataRowCollection2[j].Table.TableName);
                            continue;
                        }

                    }
                        


                        

                    }

                }
            
            string[] array = new string[arrayList.Count];
            for (int i = 0; i < arrayList.Count; i++)
            {
                array[i] = arrayList[i].ToString();
            }
            return array;

        }
    }
}
