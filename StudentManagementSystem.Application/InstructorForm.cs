using System;
using System.Collections.Generic;
using System.Windows.Forms;
using StudentManagementSystem.Application.Utilities;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Business.Constants;
using StudentManagementSystem.Entities.Concrete;
using StudentManagementSystem.Entities.Views;

namespace StudentManagementSystem.Application
{
    public partial class InstructorForm : Form
    {
        private Instructor _instructor;
        private readonly IDepartmentService _departmentService;
        private readonly IInstructorService _instructorService;
        private readonly ICatalogCourseService _catalogCourseService;
        private readonly IStudentService _studentService;
        private readonly IEnrolledCourseService _enrolledCourseService;
        private readonly List<Panel> _panels = new List<Panel>();
        private readonly LoginForm _application;

        public InstructorForm(Instructor instructor, LoginForm application, IDepartmentService departmentService, IInstructorService instructorService, ICatalogCourseService catalogCourseService, IStudentService studentService, IEnrolledCourseService enrolledCourseService)
        {
            _instructor = instructor;
            _application = application;
            _departmentService = departmentService;
            _instructorService = instructorService;
            _catalogCourseService = catalogCourseService;
            _studentService = studentService;
            _enrolledCourseService = enrolledCourseService;
            InitializeComponent();
            _panels.Add(pnlGlobalProfile);
            _panels.Add(pnlGlobalGradeOperations);
            _panels.Add(pnlGlobalAdviserOperations);
        }

        // Other Methods

        private void StudentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var messageDialogResult = MessageBox.Show("Uygulamadan çıkış yapmak istediğinize emin misiniz?",
                    "Uygulama Kapatılıyor", MessageBoxButtons.OKCancel);
                if (messageDialogResult == DialogResult.OK)
                {
                    _application.Close();
                }
                else
                {
                    e.Cancel = true;    // Stop close
                }
            }

            // If a shutdown call comes from the system (e.CloseReason == CloseReason.ApplicationExitCall), it will be shut down directly. For example, restarting the application when the password is changed.
        }

        private void btnGlobalLogOut_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Hesabınızdan çıkış yapmak istediğinize emin misiniz?", "Güvenli Çıkış", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                // https://stackoverflow.com/questions/10626396/how-to-bypass-formclosing-event
                this.FormClosing -= StudentForm_FormClosing;
                this.Close();
                _application.Show();
            }
        }

        // Profile

        private void btnGlobalProfile_Click(object sender, EventArgs e)
        {
            PanelSwitcher.ShowPanel(pnlGlobalProfile, _panels);
            ReBuildProfilePanel();
        }

        private void ReBuildProfilePanel()
        {
            PanelCleaner.Clean(pnlGlobalProfile);
            txtProfileInstructorNo.Text = _instructor.InstructorNo.ToString();
            txtProfileInfoFirstName.Text = _instructor.FirstName;
            txtProfileInfoLastName.Text = _instructor.LastName;
            txtProfileInfoEmail.Text = _instructor.Email;
            txtProfileInfoPhone.Text = _instructor.Phone;

            var departmentResult = _departmentService.GetByDepartmentNo(_instructor.DepartmentNo);

            if (departmentResult.Success)
            {
                txtProfileInfoDepartment.Text = departmentResult.Data.DepartmentName;
            }
            else
            {
                txtProfileInfoDepartment.Text = "SUNUCUDAN ALINAMADI";
            }

            if (_instructor.ModifiedAt != null)
            {
                lblProfileLastProfileUpdate.Text = $@"Son profil güncellemesi: {_instructor.ModifiedAt}";
            }
            else
            {
                lblProfileLastProfileUpdate.Text = string.Empty;
                lblProfileLastProfileUpdate.Visible = false;
            }
        }

        // Grade Operations

        private void btnGlobalGradeOperations_Click(object sender, EventArgs e)
        {
            PanelSwitcher.ShowPanel(pnlGlobalGradeOperations, _panels);
            ReBuildGradeOperationsPanel();
        }

        private void ReBuildGradeOperationsPanel()
        {
            PanelCleaner.Clean(pnlGlobalGradeOperations);
            txtGradeOperationsInputVize.Enabled = false;
            txtGradeOperationsInputFinal.Enabled = false;
            txtGradeOperationsInputButunleme.Enabled = false;
            btnGradeOperationsSaveGrades.Enabled = false;
            var departmentResult = _departmentService.GetByDepartmentNo(_instructor.DepartmentNo);
            if (departmentResult.Success)
            {
                var semesterList = new List<int>();
                for (int i = 0; i < departmentResult.Data.NumberOfSemester; i++)
                {
                    semesterList.Add(i + 1);
                }

                cmbGradeOperationsSelectSemester.DataSource = semesterList;
            }
            else
            {
                MessageBox.Show(Messages.SomethingWentWrongWhileGettingDepartmentDetails, Messages.ServerError);
            }
        }

        // Adviser Operations

        private void btnGlobalAdviserOperations_Click(object sender, EventArgs e)
        {

        }

        // Profile Methods

        private void btnProfileUpdate_Click(object sender, EventArgs e)
        {
            if (txtProfileUpdateEmail.Text == string.Empty && txtProfileUpdateFirstName.Text == string.Empty &&
                txtProfileUpdateLastName.Text == string.Empty && txtProfileUpdatePhone.Text == string.Empty)
            {
                MessageBox.Show(Messages.MustFillInTheFieldsWantToUpdate, Messages.Warning);
            }
            else
            {
                var message = "Aşağıdaki alanlar güncellenecek onaylıyor musunuz?\n";
                if (txtProfileUpdateEmail.Text != string.Empty) message += $"\nEski email: {_instructor.Email} --> Yeni email: {txtProfileUpdateEmail.Text}\n";
                if (txtProfileUpdateFirstName.Text != string.Empty) message += $"\nEski ad: {_instructor.FirstName} --> Yeni ad: {txtProfileUpdateFirstName.Text}\n";
                if (txtProfileUpdateLastName.Text != string.Empty) message += $"\nEski soyad: {_instructor.LastName} --> Yeni soyad: {txtProfileUpdateLastName.Text}\n";
                if (txtProfileUpdatePhone.Text != string.Empty) message += $"\nEski telefon: {_instructor.Phone} --> Yeni telefon: {txtProfileUpdatePhone.Text}\n";
                var selection = MessageBox.Show(message, "Güncellemeyi onaylıyor musunuz?", MessageBoxButtons.YesNo);
                if (selection == DialogResult.Yes)
                {
                    var newInstructor = new Instructor
                    {
                        Email = txtProfileUpdateEmail.Text == string.Empty ? _instructor.Email : txtProfileUpdateEmail.Text,
                        FirstName = txtProfileUpdateFirstName.Text == string.Empty ? _instructor.FirstName : txtProfileUpdateFirstName.Text,
                        LastName = txtProfileUpdateLastName.Text == string.Empty ? _instructor.LastName : txtProfileUpdateLastName.Text,
                        Phone = txtProfileUpdatePhone.Text == string.Empty ? _instructor.Phone : txtProfileUpdatePhone.Text,
                        ModifiedAt = DateTime.Now,
                        Password = _instructor.Password,
                        DepartmentNo = _instructor.DepartmentNo,
                        CreatedAt = _instructor.CreatedAt,
                        DeletedAt = _instructor.DeletedAt,
                        InstructorNo = _instructor.InstructorNo
                    };
                    var updateResult = _instructorService.Update(newInstructor);
                    if (updateResult.Success)
                    {
                        MessageBox.Show(Messages.ProfileHasBeenUpdated, Messages.Successful);
                        var instructorGetResult = _instructorService.GetByInstructorNo(_instructor.InstructorNo);
                        if (instructorGetResult.Success)
                        {
                            _instructor = instructorGetResult.Data;
                            ReBuildProfilePanel();   // like setState()
                            txtProfileUpdateEmail.Clear();
                            txtProfileUpdateFirstName.Clear();
                            txtProfileUpdateLastName.Clear();
                            txtProfileUpdatePhone.Clear();
                        }
                        else
                        {
                            MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingNewProfileInfos}:\n\n{updateResult.Message}\n\n{Messages.ApplicationIsRestarting}..", Messages.ServerError);
                            System.Windows.Forms.Application.Restart();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"{Messages.SomethingWentWrongWhileUpdate}:\n\n{updateResult.Message}", Messages.ServerError);
                    }
                }
            }
        }

        private void btnProfileChangePassword_Click(object sender, EventArgs e)
        {
            if (txtProfileOldPassword.Text != string.Empty && txtProfileNewPassword.Text != string.Empty && txtProfileReNewPassword.Text != string.Empty)
            {
                if (txtProfileNewPassword.Text != txtProfileReNewPassword.Text)
                {
                    MessageBox.Show(Messages.NewPasswordsDoNotMatch, Messages.Warning);
                    return;
                }

                if (txtProfileNewPassword.Text == txtProfileOldPassword.Text)
                {
                    MessageBox.Show(Messages.NewPasswordCannotBeSameAsOldPassword, Messages.Warning);
                }
                else
                {
                    var instructorResult = _instructorService.GetByInstructorNo(_instructor.InstructorNo);
                    if (instructorResult.Success)
                    {
                        if (txtProfileOldPassword.Text == instructorResult.Data.Password)
                        {
                            var selection = MessageBox.Show("Şifreniz güncellenecek. Onaylıyor musunuz?", "Güncellemeyi onaylıyor musunuz?", MessageBoxButtons.YesNo);

                            if (selection == DialogResult.Yes)
                            {
                                var updateResult = _instructorService.Update(new Instructor
                                {
                                    Email = _instructor.Email,
                                    FirstName = _instructor.FirstName,
                                    LastName = _instructor.LastName,
                                    Phone = _instructor.Phone,
                                    Password = txtProfileNewPassword.Text,
                                    DepartmentNo = _instructor.DepartmentNo,
                                    ModifiedAt = DateTime.Now,
                                    CreatedAt = _instructor.CreatedAt,
                                    DeletedAt = _instructor.DeletedAt,
                                    InstructorNo = _instructor.InstructorNo
                                });
                                if (updateResult.Success)
                                {
                                    MessageBox.Show($"{Messages.PasswordHasBeenChanged}. {Messages.LoginAgainWithNewPassword}. {Messages.ApplicationIsRestarting}..", Messages.Successful);
                                    System.Windows.Forms.Application.Restart();
                                }
                                else
                                {
                                    MessageBox.Show($"{Messages.SomethingWentWrongWhilePasswordChange}:\n\n{updateResult.Message}", Messages.ServerError);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(Messages.OldPasswordEnteredDoesNotMatchCurrentPassword, Messages.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"{Messages.SomethingWentWrongWhileCheckPassword}:\n\n{instructorResult.Message}", Messages.ServerError);
                    }
                }
            }
            else
            {
                MessageBox.Show(Messages.MakeSureFillInAllFields, Messages.Warning);
            }
        }

        // Grade Operations

        private void cmbGradeOperationsSelectSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGradeOperationsSelectSemester.SelectedItem == null)
            {
                return;
            }

            listBoxGradeOperationsCourseStudents.DataSource = null;
            listBoxGradeOperationsCourseStudents.Items.Clear();
            GradeOperationsClearScreenComponents();

            var catalogCourses = _catalogCourseService.GetAllByInstructorNo(_instructor.InstructorNo);
            if (catalogCourses.Success)
            {
                var selectedCourses = new List<CatalogCourse>();
                foreach (var catalogCourse in catalogCourses.Data)
                {
                    if (catalogCourse.CourseSemester == cmbGradeOperationsSelectSemester.SelectedIndex)
                    {
                        selectedCourses.Add(catalogCourse);
                    }
                }

                DataSetterToBoxes.SetDataToListBox<CatalogCourse>(listBoxGradeOperationsCurrentCourses, selectedCourses);

                lblGradeOperationsTotalCourse.Text = $"Verilen Dersler ({selectedCourses.Count} ders)";
            }
            else
            {
                MessageBox.Show(Messages.SomethingWentWrongWhileGettingCurrentCourses, Messages.ServerError);
            }
        }

        private void listBoxGradeOperationsCurrentCourses_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxGradeOperationsCurrentCourses.SelectedItem == null)
            {
                listBoxGradeOperationsCourseStudents.DataSource = null;
                listBoxGradeOperationsCourseStudents.Items.Clear();
                lblGradeOperationsInfoCourseAvarage.Text = "";
                return;
            }

            var selectedCourse = (CatalogCourse)listBoxGradeOperationsCurrentCourses.SelectedItem;  // IMPORTANT

            var studentsResult = _studentService.GetAllByCourseNo(selectedCourse.CourseNo);

            if (studentsResult.Success)
            {
                var numberOfStudent = 0;
                var gradeTotal = 0;
                foreach (var student in studentsResult.Data)
                {
                    var enrolledCourseResult =
                        _enrolledCourseService.GetByCourseNoAndStudentNo(selectedCourse.CourseNo, student.StudentNo);

                    if (enrolledCourseResult.Success)
                    {
                        var enrolledCourseView = new EnrolledCourseView
                        {
                            CourseName = "",
                            CourseNo = enrolledCourseResult.Data.CourseNo,
                            InstructorFullName = "",
                            VizeResult = enrolledCourseResult.Data.VizeResult,
                            ButunlemeResult = enrolledCourseResult.Data.ButunlemeResult,
                            FinalResult = enrolledCourseResult.Data.FinalResult
                        };
                        if (enrolledCourseView.GetCompletionGrade() != null)
                        {
                            gradeTotal += Convert.ToInt32(enrolledCourseView.GetCompletionGrade());
                            numberOfStudent += 1;
                        };
                    }
                    else
                    {
                        MessageBox.Show(Messages.SomethingWentWrongWhileFetchingData, Messages.ServerError);
                    }
                }

                if (numberOfStudent > 0)
                {
                    lblGradeOperationsInfoCourseAvarage.Text = (gradeTotal / numberOfStudent).ToString();
                }
                else
                {
                    lblGradeOperationsInfoCourseAvarage.Text = "";
                }

                DataSetterToBoxes.SetDataToListBox<Student>(listBoxGradeOperationsCourseStudents, studentsResult.Data);
            }
            else
            {
                MessageBox.Show(Messages.SomethingWentWrongWhileFetchingData, Messages.ServerError);
            }
        }

        private void listBoxGradeOperationsCourseStudents_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = $@"{((Student)e.ListItem).FirstName} {((Student)e.ListItem).LastName}";
        }

        private void listBoxGradeOperationsCourseStudents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxGradeOperationsCourseStudents.SelectedItem == null || listBoxGradeOperationsCurrentCourses.SelectedItem == null)
            {
                listBoxGradeOperationsCourseStudents.Items.Clear();
                GradeOperationsClearScreenComponents();
                return;
            }

            var selectedStudent = (Student)listBoxGradeOperationsCourseStudents.SelectedItem;
            var selectedCourse = (CatalogCourse)listBoxGradeOperationsCurrentCourses.SelectedItem;
            var instructorResult = _instructorService.GetByInstructorNo(selectedStudent.AdviserNo);

            if (instructorResult.Success)
            {
                var enrolledCourseResult =
                    _enrolledCourseService.GetByCourseNoAndStudentNo(selectedCourse.CourseNo, selectedStudent.StudentNo);

                if (enrolledCourseResult.Success)
                {
                    var enrolledCourseView = new EnrolledCourseView
                    {
                        CourseName = "",
                        CourseNo = enrolledCourseResult.Data.CourseNo,
                        InstructorFullName = "",
                        VizeResult = enrolledCourseResult.Data.VizeResult,
                        ButunlemeResult = enrolledCourseResult.Data.ButunlemeResult,
                        FinalResult = enrolledCourseResult.Data.FinalResult
                    };

                    txtGradeOperationsInfoStudentFullName.Text = $"{selectedStudent.FirstName} {selectedStudent.LastName}";
                    txtGradeOperationsInfoStudentAdviser.Text = $"{instructorResult.Data.FirstName} {instructorResult.Data.LastName}";
                    txtGradeOperationsInfoEnrollmentDate.Text = selectedStudent.EnrollmentDate.ToString();
                    txtGradeOperationsInfoSemester.Text = selectedStudent.Semester.ToString();
                    txtGradeOperationsInputVize.Text = enrolledCourseView.VizeResult.ToString();
                    txtGradeOperationsInputFinal.Text = enrolledCourseView.FinalResult.ToString();
                    txtGradeOperationsInputButunleme.Text = enrolledCourseView.ButunlemeResult.ToString();
                    lblGradeOperationsInfoStudentGradeAvarage.Text = enrolledCourseView.GetCompletionGrade() == null ? "" : enrolledCourseView.GetCompletionGrade().ToString();
                    lblGradeOperationsInfoStudentStatus.Text = enrolledCourseView.GetCompletionGrade() == null ? "" : Convert.ToInt32(enrolledCourseView.GetCompletionGrade()) > 50 ? "GEÇTİ" : "KALDI";
                    txtGradeOperationsInputVize.Enabled = true;
                    txtGradeOperationsInputFinal.Enabled = true;
                    txtGradeOperationsInputButunleme.Enabled = true;
                    btnGradeOperationsSaveGrades.Enabled = true;
                }
                else
                {
                    MessageBox.Show(Messages.SomethingWentWrongWhileFetchingData, Messages.ServerError);
                }
            }
            else
            {
                MessageBox.Show(Messages.SomethingWentWrongWhileGettingInstructorDetails, Messages.ServerError);
            }
        }

        private void GradeOperationsClearScreenComponents()
        {
            txtGradeOperationsInfoStudentFullName.Clear();
            txtGradeOperationsInfoStudentAdviser.Clear();
            txtGradeOperationsInfoEnrollmentDate.Clear();
            txtGradeOperationsInputVize.Clear();
            txtGradeOperationsInputFinal.Clear();
            txtGradeOperationsInputButunleme.Clear();
            txtGradeOperationsInfoSemester.Clear();
            lblGradeOperationsInfoStudentGradeAvarage.Text = "";
            lblGradeOperationsInfoStudentStatus.Text = "";
            txtGradeOperationsInputVize.Enabled = false;
            txtGradeOperationsInputFinal.Enabled = false;
            txtGradeOperationsInputButunleme.Enabled = false;
            btnGradeOperationsSaveGrades.Enabled = false;
        }

        private void btnGradeOperationsSaveGrades_Click(object sender, EventArgs e)
        {
            if (txtGradeOperationsInputVize.Text == string.Empty && txtGradeOperationsInputFinal.Text == string.Empty &&
                txtGradeOperationsInputButunleme.Text == string.Empty)
            {
                MessageBox.Show("Öncelikle not girişi yapmalısınız", Messages.Warning);
                return;
            }

            if (txtGradeOperationsInputFinal.Text == string.Empty &&
                txtGradeOperationsInputButunleme.Text != string.Empty)
            {
                MessageBox.Show("Bütünleme notu girebilmek için öncelikle final notu girmelisiniz");
                return;
            }

            if (listBoxGradeOperationsCurrentCourses.SelectedItem == null ||
                listBoxGradeOperationsCourseStudents.SelectedItem == null)
            {
                MessageBox.Show("Bir ders ve öğrenci seçmelisiniz", Messages.Warning);
                return;
            }

            var selectedCourse = (CatalogCourse)listBoxGradeOperationsCurrentCourses.SelectedItem;
            var selectedStudent = (Student)listBoxGradeOperationsCourseStudents.SelectedItem;

            var enrolledCourseResult =
                _enrolledCourseService.GetByCourseNoAndStudentNo(selectedCourse.CourseNo, selectedStudent.StudentNo);

            if (enrolledCourseResult.Success)
            {
                var enrolledCourseView = new EnrolledCourseView
                {
                    CourseName = "",
                    CourseNo = enrolledCourseResult.Data.CourseNo,
                    InstructorFullName = "",
                    VizeResult = txtGradeOperationsInputVize.Text == string.Empty
                        ? null
                        : Convert.ToInt32(txtGradeOperationsInputVize.Text),
                    FinalResult = txtGradeOperationsInputFinal.Text == string.Empty
                        ? null
                        : Convert.ToInt32(txtGradeOperationsInputFinal.Text),
                    ButunlemeResult = txtGradeOperationsInputButunleme.Text == string.Empty
                        ? null
                        : Convert.ToInt32(txtGradeOperationsInputButunleme.Text),
                };

                var message =
                    $"Seçili öğrencinin puanları aşağıdaki gibi güncellenecek. Onaylıyor musunuz?\n\n- Vize notu: {txtGradeOperationsInputVize.Text}\n\n- Final notu: {txtGradeOperationsInputFinal.Text}\n\n- Bütünleme notu: {txtGradeOperationsInputButunleme.Text}";
                if (txtGradeOperationsInputFinal.Text != string.Empty ||
                    txtGradeOperationsInputButunleme.Text != string.Empty)
                {
                    message += $"\n\n- Öğrenci ortalaması: {enrolledCourseView.GetCompletionGrade()}\n- Öğrenci geçme/kalma durumu : {(enrolledCourseView.GetStatus() == null ? "" : enrolledCourseView.GetStatus() == true ? "GEÇTİ" : "KALDI")}";
                }
                var dialogResult = MessageBox.Show(message, "Not Girişi Onayı", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    var updatedEnrolledCourse = new EnrolledCourse
                    {
                        Id = enrolledCourseResult.Data.Id,
                        StudentNo = enrolledCourseResult.Data.StudentNo,
                        CourseNo = enrolledCourseResult.Data.CourseNo,
                        CreatedAt = enrolledCourseResult.Data.CreatedAt,
                        DeletedAt = enrolledCourseResult.Data.DeletedAt,
                        ModifiedAt = DateTime.Now,
                        EnrolledDate = enrolledCourseResult.Data.EnrolledDate,
                        VizeResult = enrolledCourseView.VizeResult,
                        FinalResult = enrolledCourseView.FinalResult,
                        ButunlemeResult = enrolledCourseView.ButunlemeResult
                    };

                    var updateResult = _enrolledCourseService.Update(updatedEnrolledCourse);
                    if (updateResult.Success)
                    {
                        MessageBox.Show("Not atama işlemi başarıyla tamamlandı", Messages.Successful);
                        listBoxGradeOperationsCourseStudents_SelectedIndexChanged(this, EventArgs.Empty);    //Rebuild screen
                    }
                    else
                    {
                        MessageBox.Show(Messages.SomethingWentWrongWhileUpdate, Messages.ServerError);
                    }
                }
            }
            else
            {
                MessageBox.Show(Messages.SomethingWentWrongWhileFetchingData, Messages.ServerError);
            }
        }
    }
}
