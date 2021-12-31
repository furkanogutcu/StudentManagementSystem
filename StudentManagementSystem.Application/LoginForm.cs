using System;
using System.Windows.Forms;
using StudentManagementSystem.Business.Concrete;
using StudentManagementSystem.DataAccess.Concrete.Sql;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Application
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var manager = new AuthenticationManager();
            var result = manager.Login(txtGlobalUsername.Text, txtGlobalPassword.Text);
            if (result.Success)
            {
                if (result.Data.GetType() == typeof(Student))
                {
                    StudentForm studentForm = new StudentForm();
                    var student = (Student)result.Data;
                    MessageBox.Show($@"Hoşgeldiniz {student.FirstName} {student.LastName}.", "Öğrenci Girişi");
                    this.Hide();
                    studentForm.Show();
                }
                else if (result.Data.GetType() == typeof(Instructor))
                {
                    InstructorForm instructorForm = new InstructorForm();
                    var instructor = (Instructor)result.Data;
                    MessageBox.Show($@"Hoşgeldiniz {instructor.FirstName} {instructor.LastName}.", "Öğretim Görevlisi Girişi");
                    this.Hide();
                    instructorForm.Show();
                }
                else if (result.Data.GetType() == typeof(Officer))
                {
                    var officer = (Officer)result.Data;
                    MessageBox.Show($@"Hoşgeldiniz {officer.FirstName} {officer.LastName}.", "Memur Girişi");
                    OfficerForm officerForm = new OfficerForm(officer, new OfficerManager(new SqlOfficerDal()));
                    this.Hide();
                    officerForm.Show();
                }
            }
            else
            {
                MessageBox.Show(result.Message);
            }

        }
    }
}
