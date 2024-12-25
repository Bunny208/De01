using De01.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace De01
{

    public partial class frmSinhVien : Form
    {
        Model1 db = new Model1();
        private bool Add = false;

        public frmSinhVien()
        {
            InitializeComponent();
        }

        private void LoadSinhVien()
        {
            dtgSinhVien.DataSource = (from Sinhvien in db.Sinhviens
                                      join Lop in db.Lops on Sinhvien.MaLop equals Lop.MaLop
                                      select new
                                      {
                                          Sinhvien.MaSV,
                                          Sinhvien.HotenSV,
                                          Sinhvien.NgaySinh,
                                          TenLop = Lop.TenLop

                                      }).ToList();
        }

        private void LoadLop()
        {
            var LopList = db.Lops.ToList();
            cboLop.DataSource = LopList;
            cboLop.DisplayMember = "TenLop";
            cboLop.ValueMember = "MaLop";
            cboLop.SelectedIndex = -1;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSinhVien();
            LoadLop();
            btnKLuu.Enabled = false;
            btnLuu.Enabled = false;
            btnHuyTim.Enabled = false;

        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            btnSua.Enabled = true;
            btnLuu.Enabled = true;
            btnKLuu.Enabled = true;
            Add = true;
        }
        
        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtgSinhVien.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sinh viên để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var row = dtgSinhVien.SelectedRows[0];
                string maSV = row.Cells[0].Value.ToString();

                var studentToDelete = db.Sinhviens.FirstOrDefault(s => s.MaSV == maSV);
                if (studentToDelete != null)
                {
                    db.Sinhviens.Remove(studentToDelete);
                    db.SaveChanges();
                    LoadSinhVien();
                    MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa sinh viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearDuLieu()
        {
            txtHoTenSV.Text = "";
            txtMaSV.Text = "";
            cboLop.SelectedIndex = -1;
        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaSV.Text))
                {
                    MessageBox.Show("Vui lòng nhập mã sinh viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtHoTenSV.Text))
                {
                    MessageBox.Show("Vui lòng nhập họ tên sinh viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboLop.SelectedIndex == -1)
                {
                    MessageBox.Show("Vui lòng chọn lớp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Add)
                {
                    string maSV = txtMaSV.Text.Trim();
                    string hoTenSV = txtHoTenSV.Text.Trim();
                    DateTime ngaySinh = dtNgaysinh.Value;
                    string maLop = cboLop.SelectedValue.ToString();

                    var existingStudent = db.Sinhviens.FirstOrDefault(s => s.MaSV == maSV);
                    if (existingStudent != null)
                    {
                        MessageBox.Show("Mã sinh viên đã tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    Sinhvien newStudent = new Sinhvien
                    {
                        MaSV = maSV,
                        HotenSV = hoTenSV,
                        NgaySinh = ngaySinh,
                        MaLop = maLop
                    };

                    db.Sinhviens.Add(newStudent);
                    
                }
                else
                {
                    string maSV = txtMaSV.Text.Trim();
                    var student = db.Sinhviens.FirstOrDefault(s => s.MaSV == maSV);
                    if (student == null)
                    {
                        MessageBox.Show("Không tìm thấy sinh viên cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    student.HotenSV = txtHoTenSV.Text.Trim();
                    student.NgaySinh = dtNgaysinh.Value;
                    student.MaLop = cboLop.SelectedValue.ToString();
                    
                }

                db.SaveChanges();
                LoadSinhVien();
                ClearDuLieu();
                //EnableControls(false);
                MessageBox.Show(Add ? "Thêm sinh viên thành công!" : "Sửa thông tin sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnKLuu.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu thông tin sinh viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            try
            {
                string searchKeyword = txtTim.Text.Trim().ToLower(); 

                if (string.IsNullOrWhiteSpace(searchKeyword)) 
                {
                    MessageBox.Show("Vui lòng nhập từ khóa tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var filteredList = from Sinhvien in db.Sinhviens
                                   join Lop in db.Lops on Sinhvien.MaLop equals Lop.MaLop
                                   where Sinhvien.HotenSV.ToLower().Contains(searchKeyword) 
                                         
                                         

                                   select new
                                   {
                                       Sinhvien.MaSV,
                                       Sinhvien.HotenSV,
                                       Sinhvien.NgaySinh,
                                       TenLop = Lop.TenLop,
                                   };

                // Cập nhật lại DataGridView với danh sách đã lọc
                dtgSinhVien.DataSource = filteredList.ToList();
                btnKLuu.Visible = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        btnHuyTim.Enabled = true;
        }

       

        private void dtgSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dtgSinhVien.SelectedRows.Count > 0)
            {
                var row = dtgSinhVien.SelectedRows[0];
                txtMaSV.Text = row.Cells[0].Value.ToString();
                txtHoTenSV.Text = row.Cells[1].Value.ToString();
                dtNgaysinh.Text = row.Cells[2].Value.ToString();
                string tenlop = row.Cells[3].Value.ToString();
                var faculty = db.Lops.FirstOrDefault(f => f.TenLop == tenlop);
                if (faculty != null)
                {
                    cboLop.SelectedValue = faculty.MaLop;
                }
                else
                {
                    cboLop.SelectedIndex = -1;
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            btnLuu.Enabled = true;
            btnKLuu.Enabled = true;
        }

        private void btnKLuu_Click(object sender, EventArgs e)
        {
            ClearDuLieu();
            btnKLuu.Enabled = false;
            btnLuu.Enabled = true;
        }

        private void btnHuyTim_Click(object sender, EventArgs e)
        {
            try
            {
                txtTim.Text = "";
                LoadSinhVien();

                btnTim.Enabled = true;
                btnHuyTim.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hủy tìm kiếm người dùng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close(); 
            }
        }
    }
}
