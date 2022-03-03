using System;
using System.Windows.Forms;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Business.Constants;
using StudentManagementSystem.Business.DependencyResolvers.Autofac;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Application
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var authManager = InstanceFactory.GetInstance<IAuthenticationService>();
                var result = authManager.Login(txtGlobalUsername.Text, txtGlobalPassword.Text);
                if (result.Success)
                {
                    if (result.Data.GetType() == typeof(Student))
                    {
                        var student = (Student)result.Data;
                        MessageBox.Show($@"Hoşgeldiniz {student.FirstName} {student.LastName}.", Messages.StudentLogin);
                        StudentForm studentForm = new StudentForm(student, this);
                        this.Hide();
                        studentForm.Show();
                    }
                    else if (result.Data.GetType() == typeof(Instructor))
                    {
                        var instructor = (Instructor)result.Data;
                        MessageBox.Show($@"Hoşgeldiniz {instructor.FirstName} {instructor.LastName}.", Messages.InstructorLogin);
                        InstructorForm instructorForm = new InstructorForm(instructor, this);
                        this.Hide();
                        instructorForm.Show();
                    }
                    else if (result.Data.GetType() == typeof(Officer))
                    {
                        var officer = (Officer)result.Data;
                        MessageBox.Show($@"Hoşgeldiniz {officer.FirstName} {officer.LastName}.", Messages.OfficerLogin);
                        OfficerForm officerForm = new OfficerForm(officer, this);
                        this.Hide();
                        officerForm.Show();
                    }
                    txtGlobalUsername.Clear();
                    txtGlobalPassword.Clear();
                }
                else
                {
                    MessageBox.Show(result.Message, Messages.Error);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Bir şeyler ters gitti.\n\n{exception.Message}", Messages.Error);
            }
        }
    }
}
