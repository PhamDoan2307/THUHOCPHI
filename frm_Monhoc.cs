using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThuHocPhi.Controls.Tudien;
using ThuHocPhi.Models;
using ThuHocPhi.Models.Tudien;
using ThuHocPhi.Shares;

namespace ThuHocPhi.Views.TuDien
{
    public partial class frm_Monhoc : Form
    {
        DataDataContext db = new DataDataContext();
        monhoc_ctrl mh_ctrl = new monhoc_ctrl();

        public frm_Monhoc()
        {
            InitializeComponent();
            btn_chon.Click += new EventHandler(btn_chon_click);
            btn_import.Click += new EventHandler(btn_import_click);
        }

        private void btn_chon_click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            //of.Filter = ".xls||.xlsx";
            txt_chon.Text = of.ShowDialog() == DialogResult.OK ? of.FileName : "";
        }

        private void btn_import_click(object sender, EventArgs e)
        {
            if (!ValidInput())
                return;
            showdata();
            DataTable data = ReadDataFromExcelFile();
            ImportIntoDatabase(data);
            dataGridView1.DataSource = mh_ctrl.GetAllMH().Data;
        }
        private void showdata()
        {
            DataHocphiTableAdapters.MonHocTableAdapter adapter = new DataHocphiTableAdapters.MonHocTableAdapter();
            dataGridView1.DataSource = adapter.GetData();
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private bool ValidInput()
        {
            if (txt_chon.Text.Trim() == "")
            {
                MessageBox.Show("Xin vui lòng chọn tập tin excel cần import");
                
                btn_chon.Focus();
                return false;
            }
            return true;
        }
        private DataTable ReadDataFromExcelFile()
        {
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + txt_chon.Text.Trim() + ";Extended Properties=Excel 8.0";
            // Tạo đối tượng kết nối
            OleDbConnection oledbConn1 = new OleDbConnection(connectionString);
            DataTable data = null;
            try
            {
                // Mở kết nối
                oledbConn1.Open();

                // Tạo đối tượng OleDBCommand và query data từ sheet có tên "Sheet1"
                 OleDbCommand cmd1 = new OleDbCommand("SELECT * FROM [TDMH$]", oledbConn1);

                // Tạo đối tượng OleDbDataAdapter để thực thi việc query lấy dữ liệu từ tập tin excel
                OleDbDataAdapter oleda1 = new OleDbDataAdapter();

                oleda1.SelectCommand = cmd1;

                // Tạo đối tượng DataSet để hứng dữ liệu từ tập tin excel
                DataSet ds = new DataSet();

                // Đổ đữ liệu từ tập excel vào DataSet
                oleda1.Fill(ds);
                data = ds.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                // Đóng chuỗi kết nối
                oledbConn1.Close();
            }
            return data;
           
        }
        private void ImportIntoDatabase(DataTable data)
        {
            if (data == null || data.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để import");
                return;
            }
            DataHocphiTableAdapters.MonHocTableAdapter adapter = new DataHocphiTableAdapters.MonHocTableAdapter();
            string mamh = "", tenmh = "", sotchp = "";
           
            try
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    mamh = data.Rows[i]["MaMH"].ToString().Trim();
                    tenmh = data.Rows[i]["TenMH"].ToString().Trim();
                    sotchp = data.Rows[i]["SoTCHP"].ToString().Trim();
                   // mamh = data.Rows[i]["MaMH"].ToString().Trim();
                   
                    DataHocphi.MonHocDataTable existingEmployee = adapter.GetDataByMaMH(mamh);
                    // Nếu MaSV chưa tồn tại trong DB thì thêm mới
                    if (existingEmployee == null || existingEmployee.Rows.Count == 0)
                    {
                        adapter.InsertQueryMH(mamh, tenmh, sotchp);
                    }
                    // Ngược lại, sinh vien đã tồn tại trong DB thì update
                    //else
                    //{
                    //    adapter.UpdateQueryThuhocphi(hoten, lop, mamh, tenmh, nhhk, sotc, sotchp, dqt, dthi, dtk, dso, diemchu, ngayluu, masv);
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            MessageBox.Show("Kết thúc import");
        }

        private void frm_Monhoc_Load(object sender, EventArgs e)
        {
            Utils.ShowFormCenterOfPanel(this);
            var rs = mh_ctrl.GetAllMH();
            UpdateDtg(rs);

        }

        private void btn_capnhat_Click(object sender, EventArgs e)
        {
            
                frm_CapnhatMH frm = new frm_CapnhatMH();
                frm.txt_mamh.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                frm.txt_tenmh.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                frm.txt_sotchp.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    frm.Close();
                }
                var hk = mh_ctrl.GetAllMH();
                UpdateDtg(hk);
            
        }
        private void UpdateDtg(ActionResult<List<monhoc_ett>> lst)
        {
            switch (lst.ErrCode)
            {
                case CEnum.HaveNoData:
                    break;
                case CEnum.Success:
                    dataGridView1.DataSource = lst.Data;
                    break;
                case CEnum.Fail:
                    break;
                default:
                    break;
            }
        }
        private void btn_them_Click(object sender, EventArgs e)
        {
            frm_ThemMH frm = new frm_ThemMH();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                frm.Close();
            }
            var hk = mh_ctrl.GetAllMH();
            UpdateDtg(hk);
        }
        private void txt_timkiem_KeyUp(object sender, KeyEventArgs e)
        {
            DataDataContext db = new DataDataContext();
            var Lst = (from d in db.MonHocs where d.MaMH.Contains(txt_timkiem.Text) select d).ToList();
            dataGridView1.DataSource = Lst;
        }
    }
}
