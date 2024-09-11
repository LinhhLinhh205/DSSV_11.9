using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace BT01
{
    public partial class Form1 : Form
    {
        string strcon = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=..\..\..\data\QLSV.mdb";
        OleDbConnection cnn;
        DataSet ds = new DataSet();
        DataTable tblKhoa = new DataTable("KHOA");
        DataTable tblSinhVien = new DataTable("SINHVIEN");
        DataTable tblKetQua = new DataTable("KETQUA");
        OleDbCommand cmdKhoa, cmdSinhVien, cmdKetQua;
        int stt = -1;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cnn = new OleDbConnection(strcon);
            taocautrucbang();
            mocnoiquanhe();
            nhaplieu();
            khoitaocombobox();
            stt = 0;
            gandulieu(stt);
        }

        private void gandulieu(int stt)
        {
            DataRow rsv = tblSinhVien.Rows[stt];
            txtMaSV.Text = rsv["MaSV"].ToString();
            txtHo.Text = rsv["HoSV"].ToString();
            txtTen.Text = rsv["TenSV"].ToString();
            chckGioiTinh.Checked = (Boolean)rsv["Phai"];
            dateNgaySinh.Text = rsv["NgaySinh"].ToString();
            txtNoiSinh.Text = rsv["NoiSinh"].ToString();
            cboMaKhoa.SelectedValue = rsv["MaKH"].ToString();
            txtHocBong.Text = rsv["HocBong"].ToString();

            lbStt.Text = (stt + 1) + "/" + tblSinhVien.Rows.Count;

            txtDiem.Text = TongDiem(txtMaSV.Text).ToString();

        }
        private Double TongDiem(string MSV)
        {
            double kq = 0;
            Object td = tblKetQua.Compute("sum(Diem)", "MaSV='" + MSV + "'");
            if (td == DBNull.Value)
                kq = 0;
            else
                kq = Convert.ToDouble(td);
            return kq;
        }

        

        private void khoitaocombobox()
        {
            cboMaKhoa.DisplayMember = "TenKH";
            cboMaKhoa.ValueMember = "MaKH";
            cboMaKhoa.DataSource = tblKhoa;
        }

        private void nhaplieu()
        {
            nhaplieukhoa();
            nhaplieusinhvien();
            nhaplieuketqua();
        }

        private void nhaplieuketqua()
        {
            cnn.Open();
            cmdKetQua = new OleDbCommand("select* from ketqua", cnn);
            OleDbDataReader rkq = cmdKetQua.ExecuteReader();
            while (rkq.Read())
            {
                DataRow r = tblKetQua.NewRow();
                for (int i = 0; i < rkq.FieldCount; i++)
                    r[i] = rkq[i];
                tblKetQua.Rows.Add(r);
            }
            rkq.Close();
            cnn.Close();
        }

        private void nhaplieusinhvien()
        {
            cnn.Open();
            cmdSinhVien = new OleDbCommand("select* from sinhvien", cnn);
            OleDbDataReader rsv = cmdSinhVien.ExecuteReader();
            while (rsv.Read())
            {
                DataRow r = tblSinhVien.NewRow();
                for (int i = 0; i < rsv.FieldCount; i++)
                    r[i] = rsv[i];
                tblSinhVien.Rows.Add(r);
            }
            rsv.Close();
            cnn.Close();
        }

        private void nhaplieukhoa()
        {
            cnn.Open();
            cmdKhoa = new OleDbCommand("select* from khoa", cnn);
            OleDbDataReader rkh = cmdKhoa.ExecuteReader();
            while (rkh.Read())
            {
                DataRow r = tblKhoa.NewRow();
                for (int i = 0; i < rkh.FieldCount; i++)
                    r[i] = rkh[i];
                tblKhoa.Rows.Add(r);
            }
            rkh.Close();
            cnn.Close();
        }

        private void btnTruoc_Click(object sender, EventArgs e)
        {
            if (stt == 0) return;
            stt--;
            gandulieu(stt);
        }

        private void btnSau_Click(object sender, EventArgs e)
        {
            if (stt == tblSinhVien.Rows.Count - 1) return;
            stt++;
            gandulieu(stt);
        }

        private void btnThem_Click(object sender, EventArgs e)
        {

        }

        private void btnGhi_Click(object sender, EventArgs e)
        {
            cnn.Open();
            if (txtMaSV.ReadOnly == true)
            {
                DataRow rsv = tblSinhVien.Rows.Find(txtMaSV.Text);
                rsv["HoSV"] = txtHo.Text;
                rsv["TenSV"] = txtTen.Text;
                rsv["Phai"] = chckGioiTinh.Checked;
                rsv["NgaySinh"] = dateNgaySinh.Text;
                rsv["NoiSinh"] = txtNoiSinh.Text;
                rsv["MaKH"] = cboMaKhoa.SelectedValue.ToString();
                rsv["HocBong"] = txtHocBong.Text;

                string chuoi_sua = "Update SINHVIEN set ";
                chuoi_sua += "HoSV = '" + txtHo.Text + "',";
                chuoi_sua += "TenSV = '" + txtTen.Text + "',";
                chuoi_sua += "Phai = " + chckGioiTinh.Checked + ",";
                chuoi_sua += "NgaySinh = #" + dateNgaySinh.Value + "#,";
                chuoi_sua += "NoiSinh = '" + txtNoiSinh.Text + "',";
                chuoi_sua += "MaKH='" + cboMaKhoa.SelectedValue.ToString() + "',";
                chuoi_sua += "HocBong=" + txtHocBong.Text;
                chuoi_sua += "Where MaSV='" + txtMaSV.Text + "'";
                cmdSinhVien = new OleDbCommand(chuoi_sua, cnn);
                int n = cmdSinhVien.ExecuteNonQuery();
                if (n > 0)
                    MessageBox.Show("Cập nhật thành công");
            }
        }

        private void mocnoiquanhe()
        {
            ds.Relations.Add("KH_SV", ds.Tables["KHOA"].Columns["MaKH"], ds.Tables["SINHVIEN"].Columns["MaKH"], true);
            ds.Relations.Add("SV_KQ", ds.Tables["SINHVIEN"].Columns["MaSV"], ds.Tables["KETQUA"].Columns["MaSV"], true);
            ds.Relations["KH_SV"].ChildKeyConstraint.DeleteRule = Rule.None;
            ds.Relations["SV_KQ"].ChildKeyConstraint.DeleteRule = Rule.None;
        }

        private void taocautrucbang()
        {
            tblKhoa.Columns.Add("MaKH", typeof(string));
            tblKhoa.Columns.Add("TenKH", typeof(string));
            tblKhoa.PrimaryKey = new DataColumn[] { tblKhoa.Columns["MaKH"] };

            tblSinhVien.Columns.Add("MaSV", typeof(string));
            tblSinhVien.Columns.Add("HoSV", typeof(string));
            tblSinhVien.Columns.Add("TenSV", typeof(string));
            tblSinhVien.Columns.Add("Phai", typeof(Boolean));
            tblSinhVien.Columns.Add("NgaySinh", typeof(DateTime));
            tblSinhVien.Columns.Add("NoiSinh", typeof(string));
            tblSinhVien.Columns.Add("MaKH", typeof(string));
            tblSinhVien.Columns.Add("HocBong", typeof(double));
            tblSinhVien.PrimaryKey = new DataColumn[] { tblSinhVien.Columns["MaSV"] };

            tblKetQua.Columns.Add("MaSV", typeof(string));
            tblKetQua.Columns.Add("MaMH", typeof(string));
            tblKetQua.Columns.Add("Diem", typeof(double));
            tblKetQua.PrimaryKey = new DataColumn[] { tblKetQua.Columns["MaSV"], tblKetQua.Columns["MaMH"] };
            ds.Tables.AddRange(new DataTable[] { tblKhoa, tblSinhVien, tblKetQua });
        }
    }
}
