using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Picture__INOUT.Model;

namespace Picture__INOUT
{
    
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        LinkToSQL link = new LinkToSQL();

        private void button_Input_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "*.png;*.jpg;*.gif";
            if(openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
            }

            byte[] savingImage = ConvertToByte(pictureBox1.Image);

            if (savingImage == null) { return; }
            using (SqlConnection con=new SqlConnection())
            {

                link.SqlCode= "INSERT INTO [dbo].[Picture]([P_Image]) " +
                              "VALUES (@K_image) ";
                SqlParameter para = new SqlParameter("@K_image", savingImage);
                link.fn_ExecuteSQL(con, para);
                MessageBox.Show("圖片存入");
            }
            
        }

        private void button_Output_Click(object sender, EventArgs e)
        {
            using (SqlConnection con=new SqlConnection())
            {
                link.SqlCode = $"SELECT *FROM [dbo].[Picture]" +
                               $" WHERE [P_ID]=@K_ID";
                SqlParameter para = new SqlParameter("@K_ID",Convert.ToInt32(txtID.Text));
                SqlDataReader reader=link.fn_ReadSQLData(con, para);
                if(reader.Read())
                {
                    byte[] picBinary = (byte[])reader[1];
                    pictureBox2.Image = ConvertToImage(picBinary);
                }
            }
           
        }

        private Image ConvertToImage(byte[]  picBinary)
        {
            
            Image image = null;

            using (MemoryStream ms = new MemoryStream(picBinary))
            {
                image = Image.FromStream(ms);
            }
            return image;
        }

        private byte[] ConvertToByte(Image image) 
        {
            if (image == null) { return null; }
            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }
    }
    
}
