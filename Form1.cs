using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        // Tạo BindingManager
        private BindingManagerBase bMgr;
        SqlConnection conn = new SqlConnection(Properties.Settings.Default.ConnectionString);
        SqlDataAdapter da = new SqlDataAdapter();
        DataSet ds = new DataSet();


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string sql = @"SELECT * FROM Employee";

            SqlDataAdapter da = new SqlDataAdapter(sql, conn);

            // Tạo SqlCommandBuilder để tự động sinh các lệnh InsertCommand, UpdateCommand, và DeleteCommand
            SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(da);

            // Fill dữ liệu vào DataSet
            da.Fill(ds, "Employee");

            // Gán lại SqlDataAdapter đã cấu hình lệnh
            this.da = da;

            // Ràng buộc dữ liệu cho các TextBox
            FirstnameTextBox.DataBindings.Clear();
            LastnameTextBox.DataBindings.Clear();
            comboBox1.DataBindings.Clear();

            FirstnameTextBox.DataBindings.Add("Text", ds.Tables["Employee"], "FirstName");
            LastnameTextBox.DataBindings.Add("Text", ds.Tables["Employee"], "LastName");
            dataGridView1.DataSource = ds;
            dataGridView1.DataMember = "Employee";

            // Đảm bảo BindingManagerBase được khởi tạo chính xác
            bMgr = this.BindingContext[ds, "Employee"];

            comboBox1.DataSource = LoadDepartmentData();
            comboBox1.ValueMember = "DepartmentID";
            comboBox1.DisplayMember = "DepartmentName";
            comboBox1.DataBindings.Add("SelectedValue", ds.Tables["Employee"], "DepartmentID");


        }

        private void SaveChanges()
        {
            // Lưu các thay đổi từ DataSet vào cơ sở dữ liệu
            da.Update(ds.Tables["Employee"]);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem DataSet có chứa ít nhất một bảng không
            if (bMgr.Position < bMgr.Count - 1)
            {
                bMgr.Position += 1;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (bMgr.Position > 0)
                bMgr.Position -= 1;
        }
        private List<Department> LoadDepartmentData()
        {
            List<Department> list = new List<Department>();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Department", conn);
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Department cl = new Department();
                cl.DepartmentID = reader.GetInt32(0);  // Sử dụng DepartmentID thay vì ID
                cl.DepartmentName = reader.GetString(1);
                cl.DepartmentManagerID = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2);
                list.Add(cl);
            }

            reader.Close();
            conn.Close();

            return list;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void UpdateTextBoxes()
        {
            DataRow currentRow = ds.Tables["Employee"].Rows[bMgr.Position];

            FirstnameTextBox.Text = currentRow["FirstName"].ToString();
            LastnameTextBox.Text = currentRow["LastName"].ToString();
            comboBox1.SelectedValue = currentRow["DepartmentID"];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                // Tạo một hàng mới
                DataRow newRow = ds.Tables["Employee"].NewRow();

                // Cung cấp giá trị cho EmployeeID (nếu cần thiết)
                newRow["EmployeeID"] = GetNextEmployeeID();

                newRow["FirstName"] = FirstnameTextBox.Text;
                newRow["LastName"] = LastnameTextBox.Text;
                newRow["DepartmentID"] = comboBox1.SelectedValue;
                ds.Tables["Employee"].Rows.Add(newRow);
                // Lưu các thay đổi
                SaveChanges();
                bMgr = this.BindingContext[ds, "Employee"];
                // Cập nhật vị trí của BindingManagerBase
                bMgr.Position = ds.Tables["Employee"].Rows.Count - 1;
                MessageBox.Show("Thêm nhân viên mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi thêm nhân viên mới: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
      
    
        // Phương thức để lấy giá trị EmployeeID mới
        private int GetNextEmployeeID()
        {
            // Giả sử EmployeeID là số nguyên, bạn có thể thực hiện như sau:
            int maxID = 0;
            if (ds.Tables["Employee"].Rows.Count > 0)
            {
                maxID = (int)ds.Tables["Employee"].Compute("MAX(EmployeeID)", string.Empty);
            }
            return maxID + 1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Employee"].Rows.Count > 0)
            {
                // Xóa bản ghi hiện tại
                ds.Tables["Employee"].Rows[bMgr.Position].Delete();

                // Cập nhật lại BindingManagerBase để hiển thị bản ghi tiếp theo
                if (bMgr.Position > 0)
                {
                    bMgr.Position -= 1;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy hàng hiện tại từ DataSet dựa trên vị trí hiện tại của BindingManagerBase
                DataRow currentRow = ds.Tables["Employee"].Rows[bMgr.Position];

                // Cập nhật giá trị của các cột từ các điều khiển giao diện người dùng
                currentRow["FirstName"] = FirstnameTextBox.Text;
                currentRow["LastName"] = LastnameTextBox.Text;
                currentRow["DepartmentID"] = comboBox1.SelectedValue;

                // Lưu các thay đổi vào cơ sở dữ liệu
                SaveChanges();

                MessageBox.Show("Cập nhật thông tin nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi cập nhật thông tin nhân viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
