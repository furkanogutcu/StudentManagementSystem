using System;
using System.Windows.Forms;
using StudentManagementSystem.Business.Concrete;
using StudentManagementSystem.Business.Constants;
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

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var manager = new AuthenticationManager();
                var result = manager.Login(txtGlobalUsername.Text, txtGlobalPassword.Text);
                if (result.Success)
                {
                    if (result.Data.GetType() == typeof(Student))
                    {
                        var student = (Student)result.Data;
                        MessageBox.Show($@"Hoşgeldiniz {student.FirstName} {student.LastName}.", Messages.StudentLogin);
                        StudentForm studentForm = new StudentForm(
                            student,
                            this,
                            new StudentManager(new SqlStudentDal()),
                            new DepartmentManager(new SqlDepartmentDal()),
                            new InstructorManager(new SqlInstructorDal()),
                            new EnrolledCourseManager(new SqlEnrolledCourseDal()),
                            new CatalogCourseManager(new SqlCatalogCourseDal()),
                            new AdviserApprovalManager(new SqlAdviserApprovalDal())
                            );
                        this.Hide();
                        studentForm.Show();
                    }
                    else if (result.Data.GetType() == typeof(Instructor))
                    {
                        var instructor = (Instructor)result.Data;
                        MessageBox.Show($@"Hoşgeldiniz {instructor.FirstName} {instructor.LastName}.", Messages.InstructorLogin);
                        InstructorForm instructorForm = new InstructorForm(
                            instructor,
                            this,
                            new DepartmentManager(new SqlDepartmentDal()),
                            new InstructorManager(new SqlInstructorDal()),
                            new CatalogCourseManager(new SqlCatalogCourseDal()),
                            new StudentManager(new SqlStudentDal()),
                            new EnrolledCourseManager(new SqlEnrolledCourseDal()),
                            new AdviserApprovalManager(new SqlAdviserApprovalDal())
                        );
                        this.Hide();
                        instructorForm.Show();
                    }
                    else if (result.Data.GetType() == typeof(Officer))
                    {
                        var officer = (Officer)result.Data;
                        MessageBox.Show($@"Hoşgeldiniz {officer.FirstName} {officer.LastName}.", Messages.OfficerLogin);
                        OfficerForm officerForm = new OfficerForm(
                            officer,
                            new OfficerManager(new SqlOfficerDal()),
                            new DepartmentManager(new SqlDepartmentDal()),
                            new InstructorManager(new SqlInstructorDal()),
                            new StudentManager(new SqlStudentDal()),
                            new CatalogCourseManager(new SqlCatalogCourseDal()),
                            this
                        );
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
