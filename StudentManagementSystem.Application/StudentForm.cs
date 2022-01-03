using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using StudentManagementSystem.Application.Utilities;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Business.Constants;
using StudentManagementSystem.Core.Entities;
using StudentManagementSystem.Core.Utilities.Others;
using StudentManagementSystem.Entities.Concrete;
using StudentManagementSystem.Entities.Views;

namespace StudentManagementSystem.Application
{
    public partial class StudentForm : Form
    {
        private Student _student;
        private List<CatalogCourse> _openedCourses = new List<CatalogCourse>();
        private List<CatalogCourse> _coursesOnDraft = new List<CatalogCourse>();
        private readonly IStudentService _studentService;
        private readonly IDepartmentService _departmentService;
        private readonly IInstructorService _instructorService;
        private readonly IEnrolledCourseService _enrolledCourseService;
        private readonly ICatalogCourseService _catalogCourseService;
        private readonly IAdviserApprovalService _adviserApprovalService;
        private readonly List<Panel> _panels = new List<Panel>();
        private readonly LoginForm _application;

        public StudentForm(Student student, LoginForm application, IStudentService studentService, IDepartmentService departmentService, IInstructorService instructorService, IEnrolledCourseService enrolledCourseService, ICatalogCourseService catalogCourseService, IAdviserApprovalService adviserApprovalService)
        {
            _student = student;
            _application = application;
            _studentService = studentService;
            _departmentService = departmentService;
            _instructorService = instructorService;
            _enrolledCourseService = enrolledCourseService;
            _catalogCourseService = catalogCourseService;
            _adviserApprovalService = adviserApprovalService;
            InitializeComponent();
            _panels.Add(pnlGlobalProfile);
            _panels.Add(pnlGlobalGradeView);
            _panels.Add(pnlGlobalCourseRegister);
            _panels.Add(pnlGlobalTranscript);
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

        // Global Methods

        private static void SetDataToDataGridView<T>(DataGridView targetDataGridView, List<String> columnNames, List<dynamic> dataList)
            where T : class, IViews, new()
        {
            DataTable dt = new DataTable();
            foreach (var columnName in columnNames)
            {
                dt.Columns.Add(columnName);
            }

            var numberOfInfoInRow = 0;

            switch (typeof(T).Name)
            {
                case nameof(EnrolledCourseView):
                    foreach (var data in dataList)
                    {
                        if (data.GetType() != typeof(EnrolledCourseView))
                        {
                            throw new Exception(Messages.Error);
                        }

                        string status = "";

                        var enrolledCourse = (EnrolledCourseView)data;

                        if (enrolledCourse.GetStatus() != null)
                        {
                            if (enrolledCourse.GetStatus() == true)
                            {
                                status = "GEÇTİ";
                            }
                            else
                            {
                                status = "KALDI";
                            }
                        }
                        dt.Rows.Add(enrolledCourse.CourseNo, enrolledCourse.CourseName, enrolledCourse.InstructorFullName, enrolledCourse.VizeResult, enrolledCourse.FinalResult, enrolledCourse.ButunlemeResult, enrolledCourse.GetCompletionGrade(), status);
                    }
                    numberOfInfoInRow = 8;
                    break;

                case nameof(CatalogCourseView):
                    if (columnNames[0] == "SECIM")
                    {
                        dt.Columns[0].DataType = typeof(bool);
                        dt.Columns[0].ReadOnly = false;
                    }

                    foreach (var data in dataList)
                    {
                        if (data.GetType() != typeof(CatalogCourseView))
                        {
                            throw new Exception(Messages.Error);
                        }

                        var catalogCourse = (CatalogCourseView)data;

                        if (columnNames[0] == "SECIM")
                        {
                            dt.Rows.Add(false, catalogCourse.CourseNo, catalogCourse.CourseName, catalogCourse.InstructorFullName, catalogCourse.Credit, catalogCourse.CourseSemester, catalogCourse.DepartmentName);
                        }
                        else
                        {
                            dt.Rows.Add(catalogCourse.CourseNo, catalogCourse.CourseName, catalogCourse.InstructorFullName, catalogCourse.Credit, catalogCourse.CourseSemester, catalogCourse.DepartmentName);
                        }
                    }
                    numberOfInfoInRow = columnNames[0] == "SECIM" ? 7 : 6;
                    break;

                default:
                    throw new Exception(Messages.Error);
            }

            if (columnNames.Count != numberOfInfoInRow)
            {
                throw new Exception(Messages.Error);
            }

            targetDataGridView.DataSource = dt;
            targetDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            targetDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }

        // Global Methods End

        // Profile

        private void btnGlobalProfile_Click(object sender, System.EventArgs e)
        {
            PanelSwitcher.ShowPanel(pnlGlobalProfile, _panels);
            ReBuildProfilePanel();
        }

        private void ReBuildProfilePanel()
        {
            PanelCleaner.Clean(pnlGlobalProfile);
            txtProfileStudentNo.Text = _student.StudentNo.ToString();
            txtProfileInfoFirstName.Text = _student.FirstName;
            txtProfileInfoLastName.Text = _student.LastName;
            txtProfileInfoEmail.Text = _student.Email;
            txtProfileInfoPhone.Text = _student.Phone;
            txtProfileInfoEnrollmentDate.Text = _student.EnrollmentDate.ToString();
            txtProfileInfoSemester.Text = _student.Semester.ToString();

            var departmentResult = _departmentService.GetByDepartmentNo(_student.DepartmentNo);
            var instructorResult = _instructorService.GetByInstructorNo(_student.AdviserNo);

            if (departmentResult.Success && instructorResult.Success)
            {
                txtProfileInfoDepartment.Text = departmentResult.Data.DepartmentName;
                txtProfileInfoAdviser.Text = $"{instructorResult.Data.FirstName} {instructorResult.Data.LastName}";
            }
            else
            {
                txtProfileInfoDepartment.Text = "SUNUCUDAN ALINAMADI";
                txtProfileInfoAdviser.Text = $"SUNUCUDAN ALINAMADI";
            }

            if (_student.ModifiedAt != null)
            {
                lblProfileLastProfileUpdate.Text = $@"Son profil güncellemesi: {_student.ModifiedAt}";
            }
            else
            {
                lblProfileLastProfileUpdate.Text = string.Empty;
                lblProfileLastProfileUpdate.Visible = false;
            }
        }

        // Grade View

        private void btnGlobalGradeView_Click(object sender, EventArgs e)
        {
            PanelSwitcher.ShowPanel(pnlGlobalGradeView, _panels);
            ReBuildGradeViewPanel();
        }

        private void ReBuildGradeViewPanel()
        {
            PanelCleaner.Clean(pnlGlobalGradeView);
            var departmentResult = _departmentService.GetByDepartmentNo(_student.DepartmentNo);
            if (departmentResult.Success)
            {
                var totalDepartmentSemester = departmentResult.Data.NumberOfSemester;
                var studentEnrollmentYear = Convert.ToInt32(_student.EnrollmentDate);
                var studentCurrentSemester = _student.Semester;
                var currentYear = DateTime.Now.Year;

                var yearDiff = currentYear - studentEnrollmentYear;
                if (yearDiff > (totalDepartmentSemester / 2))
                {
                    MessageBox.Show($"Öğrenci bilgilerinde bir sorun var:\n\n-Öğrenci {studentEnrollmentYear} yılında sisteme kayıtlanmış. {currentYear} yılında öğrencinin dönemi {yearDiff * 2} veya {yearDiff * 2 + 1} olabilir ancak öğrencinin kayıtlı olduğu bölümün dönem sayısı {totalDepartmentSemester}.\n\nLütfen durumu danışmanınıza iletiniz.", Messages.Error);
                    return;
                }

                if (studentCurrentSemester > totalDepartmentSemester)
                {
                    MessageBox.Show($"Öğrenci bilgilerinde bir sorun var:\n\n-Öğrenci dönemi {studentCurrentSemester}, toplam bölüm döneminden {totalDepartmentSemester} büyük olamaz.\n\nLütfen durumu danışmanınıza iletiniz.", Messages.Error);
                    return;
                }

                if (studentCurrentSemester > ((yearDiff + 1) * 2))
                {
                    MessageBox.Show($"Öğrenci bilgilerinde bir sorun var:\n\n-Öğrenci {studentEnrollmentYear} yılında sisteme kayıtlanmış. {currentYear} yılında öğrencinin dönemi en fazla {(yearDiff + 1) * 2} olabilir ancak öğrencinin dönemi {studentCurrentSemester}.\n\nLütfen durumu danışmanınıza iletiniz.", Messages.Error);
                    return;
                }

                var gradeViewComboBoxItems = new List<string>();

                for (int i = 0; i < studentCurrentSemester; i++)
                {
                    var addedYear = Convert.ToInt32(i / 2);
                    var first = "GÜZ";
                    var second = "BAHAR";
                    gradeViewComboBoxItems.Add($"{studentEnrollmentYear + addedYear} - {studentEnrollmentYear + addedYear + 1} {i + 1}. dönem ({(i % 2 == 0 ? first : second)})");
                }

                cmbGradeViewSelectSemester.DataSource = gradeViewComboBoxItems;
            }
            else
            {
                MessageBox.Show(Messages.SomethingWentWrongWhileGettingDepartmentDetails, Messages.ServerError);
            }
        }

        // Course Register

        private void btnGlobalCourseRegister_Click(object sender, EventArgs e)
        {
            PanelSwitcher.ShowPanel(pnlGlobalCourseRegister, _panels);
            ReBuildCourseRegisterPanel();
        }

        private void ReBuildCourseRegisterPanel()
        {
            PanelCleaner.Clean(pnlGlobalCourseRegister);
            var catalogCourseResult = _catalogCourseService.GetAllByDepartmentNoAndSemesterNo(_student.DepartmentNo, _student.Semester);
            var coursesAlreadyEnrolledResult = _enrolledCourseService.GetAllByStudentNo(_student.StudentNo);
            var adviserApprovalResult = _adviserApprovalService.GetAllByStudentNo(_student.StudentNo);
            if (catalogCourseResult.Success && coursesAlreadyEnrolledResult.Success && adviserApprovalResult.Success)
            {
                var openedCourses = catalogCourseResult.Data;
                var coursesAlreadyEnrolled = coursesAlreadyEnrolledResult.Data;

                var deletedCourses = new List<CatalogCourse>();

                // Add already registered courses to the list of courses to be deleted
                foreach (var openedCourse in openedCourses)
                {
                    foreach (var enrolledCourse in coursesAlreadyEnrolled)
                    {
                        if (openedCourse.CourseNo == enrolledCourse.CourseNo)
                        {
                            deletedCourses.Add(openedCourse);
                        }
                    }
                }

                // Add courses pending advisor approval to the list of courses to be deleted
                foreach (var adviserApproval in adviserApprovalResult.Data)
                {
                    var courseResult = _catalogCourseService.GetByCourseNo(adviserApproval.CatalogCourseCode);
                    if (courseResult.Success)
                    {
                        deletedCourses.Add(courseResult.Data);
                    }
                    else
                    {
                        MessageBox.Show(Messages.SomethingWentWrongWhileGettingCourseDetails, Messages.ServerError);
                        return;
                    }
                }

                // Delete courses to be deleted from opened courses
                foreach (var deletedCourse in deletedCourses)
                {
                    var deletedCourseInOpenedCourses = openedCourses.Find(o => o.CourseNo == deletedCourse.CourseNo);
                    openedCourses.Remove(deletedCourseInOpenedCourses);
                }

                // Finally, Assign actually available courses to the _openedCourses list
                _openedCourses.Clear();
                foreach (var catalogCourse in openedCourses)
                {
                    _openedCourses.Add(catalogCourse);
                }

                // If there is a course added to the draft in the _openedCourses, remove it from the _openedCourses.
                if (_coursesOnDraft.Count > 0)
                {
                    foreach (var courseOnDraft in _coursesOnDraft)
                    {
                        if (_openedCourses.Any(o => o.CourseNo == courseOnDraft.CourseNo))
                        {
                            var deletedCourse = _openedCourses.Find(c => c.CourseNo == courseOnDraft.CourseNo);
                            _openedCourses.Remove(deletedCourse);
                        }
                    }
                }

                //Currently available course list (_openedCourses) and drafted course list (_coursesOnDraft) are available.
                //Now it's time to show them on the screen

                tabControlCourseRegister.TabPages["tabPageAvailableCourses"].Text =
                    $"Açılan Dersler ({_openedCourses.Count} ders)";
                tabControlCourseRegister.TabPages["tabPageCoursesOnDraft"].Text =
                    $"Taslaktaki Dersler ({_coursesOnDraft.Count} ders)";

                var openedCourseFinalList = new List<CatalogCourseView>();

                foreach (var openedCourse in _openedCourses)
                {

                    var departmentResult = _departmentService.GetByDepartmentNo(openedCourse.DepartmentNo);
                    var instructorResult = _instructorService.GetByInstructorNo(openedCourse.InstructorNo);

                    if (departmentResult.Success && instructorResult.Success)
                    {
                        var catalogCourseView = new CatalogCourseView
                        {
                            CourseName = openedCourse.CourseName,
                            CourseNo = openedCourse.CourseNo,
                            InstructorFullName = $"{instructorResult.Data.FirstName} {instructorResult.Data.LastName}",
                            DepartmentName = departmentResult.Data.DepartmentName,
                            CourseSemester = openedCourse.CourseSemester,
                            Credit = openedCourse.Credit
                        };

                        openedCourseFinalList.Add(catalogCourseView);
                    }
                    else
                    {
                        MessageBox.Show(Messages.SomethingWentWrongWhileFetchingData, Messages.ServerError);
                        return;
                    }
                }

                var columnNames = new List<string>() { "SECIM", "Ders No", "Ders Adı", "Ders Öğretmeni", "Kredi", "Ders Dönemi", "Ders Bölümü" };

                SetDataToDataGridView<CatalogCourseView>(dataGridViewCourseRegisterAvailableCourses, columnNames, openedCourseFinalList.ToList<dynamic>());


                var coursesOnDraftFinalList = new List<CatalogCourseView>();

                foreach (var courseOnDraft in _coursesOnDraft)
                {

                    var departmentResult = _departmentService.GetByDepartmentNo(courseOnDraft.DepartmentNo);
                    var instructorResult = _instructorService.GetByInstructorNo(courseOnDraft.InstructorNo);

                    if (departmentResult.Success && instructorResult.Success)
                    {
                        var catalogCourseView = new CatalogCourseView
                        {
                            CourseName = courseOnDraft.CourseName,
                            CourseNo = courseOnDraft.CourseNo,
                            InstructorFullName = $"{instructorResult.Data.FirstName} {instructorResult.Data.LastName}",
                            DepartmentName = departmentResult.Data.DepartmentName,
                            CourseSemester = courseOnDraft.CourseSemester,
                            Credit = courseOnDraft.Credit
                        };

                        coursesOnDraftFinalList.Add(catalogCourseView);
                    }
                    else
                    {
                        MessageBox.Show(Messages.SomethingWentWrongWhileFetchingData, Messages.ServerError);
                        return;
                    }
                }

                SetDataToDataGridView<CatalogCourseView>(dataGridViewCourseRegisterSelectedCourses, columnNames, coursesOnDraftFinalList.ToList<dynamic>());

                var coursesAlreadyEnrolledList = new List<CatalogCourseView>();

                var numberOfCoursesApprovedInTheCurrentSemester = coursesAlreadyEnrolled.Count;
                foreach (var courseAlreadyEnrolled in coursesAlreadyEnrolled)
                {
                    var courseResult = _catalogCourseService.GetByCourseNo(courseAlreadyEnrolled.CourseNo);

                    if (!courseResult.Success)
                    {
                        MessageBox.Show(Messages.SomethingWentWrongWhileFetchingData, Messages.ServerError);
                        return;
                    }

                    if (courseResult.Data.CourseSemester != _student.Semester)
                    {
                        numberOfCoursesApprovedInTheCurrentSemester -= 1;
                        continue;
                    }

                    var departmentResult = _departmentService.GetByDepartmentNo(courseResult.Data.DepartmentNo);
                    var instructorResult = _instructorService.GetByInstructorNo(courseResult.Data.InstructorNo);

                    if (departmentResult.Success && instructorResult.Success)
                    {
                        var catalogCourseView = new CatalogCourseView
                        {
                            CourseName = courseResult.Data.CourseName,
                            CourseNo = courseResult.Data.CourseNo,
                            InstructorFullName = $"{instructorResult.Data.FirstName} {instructorResult.Data.LastName}",
                            DepartmentName = departmentResult.Data.DepartmentName,
                            CourseSemester = courseResult.Data.CourseSemester,
                            Credit = courseResult.Data.Credit,
                        };

                        coursesAlreadyEnrolledList.Add(catalogCourseView);
                    }
                    else
                    {
                        MessageBox.Show(Messages.SomethingWentWrongWhileFetchingData, Messages.ServerError);
                        return;
                    }
                }

                var newColumnNames = new List<string>();
                foreach (var columnName in columnNames)
                {
                    newColumnNames.Add(columnName);
                }
                newColumnNames.RemoveAt(0);
                SetDataToDataGridView<CatalogCourseView>(dataGridViewCourseRegisterApprovedCourses, newColumnNames, coursesAlreadyEnrolledList.ToList<dynamic>());

                tabControlCourseRegister.TabPages["tabPageApprovedCourses"].Text =
                    $"Onaylanan Dersler ({numberOfCoursesApprovedInTheCurrentSemester} ders)";

                if (adviserApprovalResult.Data.Count > 0)
                {
                    lblAdviserApprovalCoursesPendingApproval.Text =
                        $"Danışman onaya gönderdiğiniz {adviserApprovalResult.Data.Count} ders danışmanınız tarafından onaylanmayı bekliyor";
                    lblAdviserApprovalCoursesPendingApproval.Visible = true;
                }
                else
                {
                    lblAdviserApprovalCoursesPendingApproval.Visible = false;
                    lblAdviserApprovalCoursesPendingApproval.Text = "";
                }

            }
            else
            {
                MessageBox.Show(Messages.SomethingWentWrongWhileFetchingData, Messages.ServerError);
            }
        }

        private void btnGlobalTranscript_Click(object sender, EventArgs e)
        {
            PanelSwitcher.ShowPanel(pnlGlobalTranscript, _panels);
            lblTranscriptPdfOutputFileName.Text = $"{TurkishCharNormalizer.Normalization(_student.FirstName.ToLower())}_{TurkishCharNormalizer.Normalization(_student.LastName.ToLower())}_transkript.pdf";
        }

        // Profile Methods

        private void btnProfileUpdate_Click(object sender, System.EventArgs e)
        {
            if (txtProfileUpdateEmail.Text == string.Empty && txtProfileUpdateFirstName.Text == string.Empty &&
                txtProfileUpdateLastName.Text == string.Empty && txtProfileUpdatePhone.Text == string.Empty)
            {
                MessageBox.Show(Messages.MustFillInTheFieldsWantToUpdate, Messages.Warning);
            }
            else
            {
                var message = "Aşağıdaki alanlar güncellenecek onaylıyor musunuz?\n";
                if (txtProfileUpdateEmail.Text != string.Empty) message += $"\nEski email: {_student.Email} --> Yeni email: {txtProfileUpdateEmail.Text}\n";
                if (txtProfileUpdateFirstName.Text != string.Empty) message += $"\nEski ad: {_student.FirstName} --> Yeni ad: {txtProfileUpdateFirstName.Text}\n";
                if (txtProfileUpdateLastName.Text != string.Empty) message += $"\nEski soyad: {_student.LastName} --> Yeni soyad: {txtProfileUpdateLastName.Text}\n";
                if (txtProfileUpdatePhone.Text != string.Empty) message += $"\nEski telefon: {_student.Phone} --> Yeni telefon: {txtProfileUpdatePhone.Text}\n";
                var selection = MessageBox.Show(message, "Güncellemeyi onaylıyor musunuz?", MessageBoxButtons.YesNo);
                if (selection == DialogResult.Yes)
                {
                    var newStudent = new Student()
                    {
                        Email = txtProfileUpdateEmail.Text == string.Empty ? _student.Email : txtProfileUpdateEmail.Text,
                        FirstName = txtProfileUpdateFirstName.Text == string.Empty ? _student.FirstName : txtProfileUpdateFirstName.Text,
                        LastName = txtProfileUpdateLastName.Text == string.Empty ? _student.LastName : txtProfileUpdateLastName.Text,
                        Phone = txtProfileUpdatePhone.Text == string.Empty ? _student.Phone : txtProfileUpdatePhone.Text,
                        ModifiedAt = DateTime.Now,
                        StudentNo = _student.StudentNo,
                        Password = _student.Password,
                        AdviserNo = _student.AdviserNo,
                        DepartmentNo = _student.DepartmentNo,
                        EnrollmentDate = _student.EnrollmentDate,
                        Semester = _student.Semester,
                        CreatedAt = _student.CreatedAt,
                        DeletedAt = _student.DeletedAt
                    };
                    var updateResult = _studentService.Update(newStudent);
                    if (updateResult.Success)
                    {
                        MessageBox.Show(Messages.ProfileHasBeenUpdated, Messages.Successful);
                        var studentGetResult = _studentService.GetByStudentNo(_student.StudentNo);
                        if (studentGetResult.Success)
                        {
                            _student = studentGetResult.Data;
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
                    var studentResult = _studentService.GetByStudentNo(_student.StudentNo);
                    if (studentResult.Success)
                    {
                        if (txtProfileOldPassword.Text == studentResult.Data.Password)
                        {
                            var selection = MessageBox.Show("Şifreniz güncellenecek. Onaylıyor musunuz?", "Güncellemeyi onaylıyor musunuz?", MessageBoxButtons.YesNo);

                            if (selection == DialogResult.Yes)
                            {
                                var updateResult = _studentService.Update(new Student
                                {
                                    Email = _student.Email,
                                    FirstName = _student.FirstName,
                                    LastName = _student.LastName,
                                    StudentNo = _student.StudentNo,
                                    Phone = _student.Phone,
                                    Password = txtProfileNewPassword.Text,
                                    EnrollmentDate = _student.EnrollmentDate,
                                    Semester = _student.Semester,
                                    AdviserNo = _student.AdviserNo,
                                    DepartmentNo = _student.DepartmentNo,
                                    ModifiedAt = DateTime.Now,
                                    CreatedAt = _student.CreatedAt,
                                    DeletedAt = _student.DeletedAt
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
                        MessageBox.Show($"{Messages.SomethingWentWrongWhileCheckPassword}:\n\n{studentResult.Message}", Messages.ServerError);
                    }
                }
            }
            else
            {
                MessageBox.Show(Messages.MakeSureFillInAllFields, Messages.Warning);
            }
        }

        // Grade View Methods

        private void cmbGradeViewSelectSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedSemester = cmbGradeViewSelectSemester.SelectedIndex + 1;
            var studentEnrolledCoursesResult = _enrolledCourseService.GetAllByStudentNo(_student.StudentNo);
            if (studentEnrolledCoursesResult.Success)
            {
                var enrolledCourses = studentEnrolledCoursesResult.Data;

                var finalList = new List<EnrolledCourseView>();

                foreach (var enrolledCourse in enrolledCourses)
                {
                    var courseResult = _catalogCourseService.GetByCourseNo(enrolledCourse.CourseNo);
                    var instructorResult = _instructorService.GetByCourseNo(enrolledCourse.CourseNo);

                    if (courseResult.Success && instructorResult.Success)
                    {
                        if (courseResult.Data.CourseSemester != selectedSemester)
                        {
                            continue;
                        }

                        var courseView = new EnrolledCourseView
                        {
                            CourseName = courseResult.Data.CourseName,
                            CourseNo = enrolledCourse.CourseNo,
                            InstructorFullName = $"{instructorResult.Data.FirstName} {instructorResult.Data.LastName}",
                            VizeResult = enrolledCourse.VizeResult,
                            FinalResult = enrolledCourse.FinalResult,
                            ButunlemeResult = enrolledCourse.ButunlemeResult
                        };

                        finalList.Add(courseView);
                    }
                    else
                    {
                        MessageBox.Show(Messages.SomethingWentWrongWhileGettingStudentsEnrolledCourses, Messages.ServerError);
                        return;
                    }
                }

                var columnNames = new List<string>() { "Ders No", "Ders Adı", "Ders Öğretmeni", "Vize Notu", "Final Notu", "Bütünleme Notu", "Başarı Notu", "Geçme Durumu" };

                SetDataToDataGridView<EnrolledCourseView>(dataGridViewGradeViewGrades, columnNames, finalList.ToList<dynamic>());
            }
            else
            {
                MessageBox.Show(Messages.SomethingWentWrongWhileGettingStudentsEnrolledCourses, Messages.ServerError);
            }
        }

        // Course Register Methods

        private void dataGridViewCourseRegisterAvailableCourses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                dataGridViewCourseRegisterAvailableCourses.Rows[e.RowIndex].Cells["SECIM"].Value = !(bool)dataGridViewCourseRegisterAvailableCourses.Rows[e.RowIndex].Cells["SECIM"].Value;
        }

        private void btnCourseRegisterSendToDraft_Click(object sender, EventArgs e)
        {
            var selectedCoursesNumbers = new List<int>();
            foreach (DataGridViewRow dataGridViewRow in dataGridViewCourseRegisterAvailableCourses.Rows)
            {
                if ((bool)dataGridViewRow.Cells["SECIM"].Value == true)
                {
                    selectedCoursesNumbers.Add(Convert.ToInt32(dataGridViewRow.Cells["Ders No"].Value));
                }
            }

            if (selectedCoursesNumbers.Count == 0)
            {
                MessageBox.Show("Herhangi bir ders seçmediniz", Messages.Warning);
                return;
            }

            var message = "";
            var selectedCourses = new List<CatalogCourse>();

            foreach (var selectedCoursesNumber in selectedCoursesNumbers)
            {
                var selectedCourse = _openedCourses.Find(c => c.CourseNo == selectedCoursesNumber);

                if (selectedCourse != null)
                {
                    selectedCourses.Add(selectedCourse);
                    message += $"- {selectedCourse.CourseName} ({selectedCourse.Credit} kredi)\n";
                }
                else
                {
                    MessageBox.Show(Messages.SomethingWentWrongWhileGettingCourseDetails, Messages.Error);
                    return;
                }
            }

            var dialogResult = MessageBox.Show($"Aşağıdaki dersleri taslağa göndermeyi onaylıyor musunuz?\n\n{message}", "Taslağa Gönderme Onayı", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                foreach (var selectedCourse in selectedCourses)
                {
                    _coursesOnDraft.Add(selectedCourse);
                }

                ReBuildCourseRegisterPanel();

                MessageBox.Show(
                    "Seçilen dersler başarıyla taslağa kaydedildi. Taslaktaki derslerinizi danışman onayına göndermeyi unutmayınız",
                    Messages.Successful);
            }
        }

        private void dataGridViewCourseRegisterSelectedCourses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                dataGridViewCourseRegisterSelectedCourses.Rows[e.RowIndex].Cells["SECIM"].Value = !(bool)dataGridViewCourseRegisterSelectedCourses.Rows[e.RowIndex].Cells["SECIM"].Value;
        }

        private void btnCourseRegisterDeleteToDraft_Click(object sender, EventArgs e)
        {
            var selectedCoursesNumbers = new List<int>();
            foreach (DataGridViewRow dataGridViewRow in dataGridViewCourseRegisterSelectedCourses.Rows)
            {
                if ((bool)dataGridViewRow.Cells["SECIM"].Value == true)
                {
                    selectedCoursesNumbers.Add(Convert.ToInt32(dataGridViewRow.Cells["Ders No"].Value));
                }
            }

            if (selectedCoursesNumbers.Count == 0)
            {
                MessageBox.Show("Herhangi bir ders seçmediniz", Messages.Warning);
                return;
            }

            var message = "";
            var selectedCourses = new List<CatalogCourse>();

            foreach (var selectedCoursesNumber in selectedCoursesNumbers)
            {
                var selectedCourse = _coursesOnDraft.Find(c => c.CourseNo == selectedCoursesNumber);

                if (selectedCourse != null)
                {
                    selectedCourses.Add(selectedCourse);
                    message += $"- {selectedCourse.CourseName} ({selectedCourse.Credit} kredi)\n";
                }
                else
                {
                    MessageBox.Show(Messages.SomethingWentWrongWhileGettingCourseDetails, Messages.Error);
                    return;
                }
            }

            var dialogResult = MessageBox.Show($"Aşağıdaki dersleri taslaktan silmeyi onaylıyor musunuz? (İlgili dersler tekrar 'Açılan dersler' ekranında listelenecek)\n\n{message}", "Taslaktan Silme Onayı", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                foreach (var selectedCourse in selectedCourses)
                {
                    _coursesOnDraft.Remove(selectedCourse);
                }

                ReBuildCourseRegisterPanel();

                MessageBox.Show(
                    "Seçilen dersler başarıyla taslaktan silindi",
                    Messages.Successful);
            }
        }

        private void btnCourseRegisterSubmitForAdviserApproval_Click(object sender, EventArgs e)
        {
            var message = "";
            var totalCourseCredit = 0;

            foreach (var catalogCourse in _coursesOnDraft)
            {
                message += $"- {catalogCourse.CourseName} ({catalogCourse.Credit} kredi)\n";
                totalCourseCredit += catalogCourse.Credit;
            }

            var dialogResult = MessageBox.Show($"Aşağıdaki dersleri danışman onayına göndermeyi onaylıyor musunuz?\n\n{message}\nDerslerin toplam kredisi: {totalCourseCredit} kredi", "Danışman Onayına Gönderme Onayı", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var failedCourses = new List<CatalogCourse>();

                var coursesToBeSent = new List<CatalogCourse>();

                foreach (var catalogCourse in _coursesOnDraft)
                {
                    coursesToBeSent.Add(catalogCourse);
                }

                foreach (var catalogCourse in coursesToBeSent)
                {
                    var adviserApproval = new AdviserApproval
                    {
                        Id = 1,
                        StudentNo = _student.StudentNo,
                        CatalogCourseCode = catalogCourse.CourseNo,
                    };
                    var result = _adviserApprovalService.Add(adviserApproval);
                    if (result.Success)
                    {
                        _coursesOnDraft.Remove(catalogCourse);
                    }
                    else
                    {
                        failedCourses.Add(catalogCourse);
                    }
                }

                if (failedCourses.Count > 0)
                {
                    var errorMessage = "";
                    foreach (var catalogCourse in failedCourses)
                    {
                        errorMessage +=
                            $"-{catalogCourse.CourseName} ({catalogCourse.Credit} kredi) --> (DANIŞMAN ONAYINA GÖNDERİLEMEDİ)\n";
                    }

                    MessageBox.Show(
                        $"Danışman onayına gönderim tamamlandı. Aşağıdaki dersler danışman onayına gönderilirken bir şeyler ters gitti:\n\n{errorMessage}",
                        Messages.Error);
                }
                else
                {
                    MessageBox.Show("Taslaktaki tüm dersler başarıyla danışman onayına gönderildi",
                        Messages.Successful);
                }
                ReBuildCourseRegisterPanel();
            }
        }

        private void btnTranscriptGetTranscriptPDF_Click(object sender, EventArgs e)
        {
            var departmentResult = _departmentService.GetByDepartmentNo(_student.DepartmentNo);

            if (!departmentResult.Success)
            {
                MessageBox.Show(Messages.SomethingWentWrongWhileGettingDepartmentDetails, Messages.ServerError);
            }

            // https://stackoverflow.com/questions/4023118/html-to-pdf-turkish-character-problem
            // https://www.c-sharpcorner.com/blogs/create-table-in-pdf-using-c-sharp-and-itextsharp

            BaseFont STF_Helvetica_Turkish = BaseFont.CreateFont("Helvetica", "CP1254", BaseFont.NOT_EMBEDDED);
            var fontNormal = new iTextSharp.text.Font(STF_Helvetica_Turkish, 12, iTextSharp.text.Font.NORMAL);
            var fontBold = new iTextSharp.text.Font(STF_Helvetica_Turkish, 12, iTextSharp.text.Font.BOLD);
            var headerFont = new iTextSharp.text.Font(STF_Helvetica_Turkish, 18, iTextSharp.text.Font.BOLD);
            BaseColor baseColorBlue = new BaseColor(153, 255, 255);
            BaseColor baseColorGray = new BaseColor(224, 224, 224);
            BaseColor baseColorYellow = new BaseColor(255, 255, 102);
            BaseColor baseColorYellowLight = new BaseColor(255, 255, 153);

            Document pdfDocument = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            var fileName =
                $"{TurkishCharNormalizer.Normalization(_student.FirstName.ToLower())}_{TurkishCharNormalizer.Normalization(_student.LastName.ToLower())}_transkript";
            var targetPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\{fileName}.pdf";
            PdfWriter writer = PdfWriter.GetInstance(pdfDocument, new FileStream(targetPath, FileMode.Create));

            PdfPTable transcriptTable = new PdfPTable(5);
            transcriptTable.HorizontalAlignment = 0;
            transcriptTable.TotalWidth = 820f;
            transcriptTable.LockedWidth = true;
            float[] widths = new float[] { 20f, 60f, 40f, 30f, 40f };
            //float[] widths = new float[] { 20f, 60f, 60f, 30f, 50f};
            transcriptTable.SetWidths(widths);
            transcriptTable.AddCell(new PdfPCell(new Phrase($"{_student.FirstName.ToUpper()} {_student.LastName.ToUpper()} ÖĞRENCİSİNE AİT TRANSKRİPT", headerFont))
            {
                Colspan = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = baseColorBlue,
                MinimumHeight = 60f,
                FixedHeight = 60f,
            });

            var emptyFullCell = new PdfPCell(new Phrase(" ", fontNormal))
            {
                Colspan = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            };

            var dateTime = DateTime.Now;
            transcriptTable.AddCell(new PdfPCell(new Phrase($"Tarih: {(dateTime.Day.ToString().Length == 1 ? "0" : "")}{dateTime.Day}/{(dateTime.Day.ToString().Length == 1 ? "0" : "")}{dateTime.Month}/{dateTime.Year}", fontBold))
            {
                Colspan = 5,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                MinimumHeight = 40f,
            });

            transcriptTable.AddCell(new PdfPCell(new Phrase("Öğrenci no:", fontBold)) { BackgroundColor = baseColorGray, VerticalAlignment = Element.ALIGN_MIDDLE });
            transcriptTable.AddCell(new PdfPCell(new Phrase($"{_student.StudentNo}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE });
            transcriptTable.AddCell(new PdfPCell(new Phrase(" ", fontBold)) { Rowspan = 3, Colspan = 1 });
            transcriptTable.AddCell(new PdfPCell(new Phrase("Bölümü:", fontBold)) { BackgroundColor = baseColorGray, VerticalAlignment = Element.ALIGN_MIDDLE });
            transcriptTable.AddCell(new PdfPCell(new Phrase($"{departmentResult.Data.DepartmentName}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE });

            transcriptTable.AddCell(new PdfPCell(new Phrase("Adı:", fontBold)) { BackgroundColor = baseColorGray, VerticalAlignment = Element.ALIGN_MIDDLE });
            transcriptTable.AddCell(new PdfPCell(new Phrase($"{_student.FirstName.ToUpper()}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE });
            transcriptTable.AddCell(new PdfPCell(new Phrase("Soyadı:", fontBold)) { BackgroundColor = baseColorGray, VerticalAlignment = Element.ALIGN_MIDDLE });
            transcriptTable.AddCell(new PdfPCell(new Phrase($"{_student.LastName.ToUpper()}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE });

            transcriptTable.AddCell(new PdfPCell(new Phrase("Kayıt yılı:", fontBold)) { BackgroundColor = baseColorGray, VerticalAlignment = Element.ALIGN_MIDDLE });
            transcriptTable.AddCell(new PdfPCell(new Phrase($"{_student.EnrollmentDate}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE });
            transcriptTable.AddCell(new PdfPCell(new Phrase("Mezuniyet yılı:", fontBold)) { BackgroundColor = baseColorGray, VerticalAlignment = Element.ALIGN_MIDDLE });
            transcriptTable.AddCell(new PdfPCell(new Phrase($"{(departmentResult.Data.NumberOfSemester / 2) + _student.EnrollmentDate}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE });

            transcriptTable.AddCell(emptyFullCell);

            var enrolledCourseDetails = _enrolledCourseService.GetAllByStudentNo(_student.StudentNo);
            if (!enrolledCourseDetails.Success)
            {
                return;
            }

            for (int i = 0; i < _student.Semester; i++)
            {
                var addedYear = Convert.ToInt32(i / 2);
                var first = "Güz";
                var second = "Bahar";

                transcriptTable.AddCell(new PdfPCell(new Phrase($"{i + 1}. Yarıyıl : {_student.EnrollmentDate + addedYear} - {_student.EnrollmentDate + addedYear + 1} {(i % 2 == 0 ? first : second)}", fontBold))
                {
                    Colspan = 5,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    BackgroundColor = baseColorBlue
                });

                transcriptTable.AddCell(new PdfPCell(new Phrase("Ders Kodu", fontBold)) { BackgroundColor = baseColorGray, MinimumHeight = 25, FixedHeight = 25, VerticalAlignment = Element.ALIGN_MIDDLE });
                transcriptTable.AddCell(new PdfPCell(new Phrase("Ders Adı", fontBold)) { BackgroundColor = baseColorGray, MinimumHeight = 25, FixedHeight = 25, VerticalAlignment = Element.ALIGN_MIDDLE });
                transcriptTable.AddCell(new PdfPCell(new Phrase("Kredisi", fontBold)) { BackgroundColor = baseColorGray, MinimumHeight = 25, FixedHeight = 25, VerticalAlignment = Element.ALIGN_MIDDLE });
                transcriptTable.AddCell(new PdfPCell(new Phrase("Başarı Notu", fontBold)) { BackgroundColor = baseColorGray, MinimumHeight = 25, FixedHeight = 25, VerticalAlignment = Element.ALIGN_MIDDLE });
                transcriptTable.AddCell(new PdfPCell(new Phrase("Durumu", fontBold)) { BackgroundColor = baseColorGray, MinimumHeight = 25, FixedHeight = 25, VerticalAlignment = Element.ALIGN_MIDDLE });

                var totalCredit = 0;
                var totalCompleteCredit = 0;
                var semesterGrade = 0;

                foreach (var enrolledCourse in enrolledCourseDetails.Data)
                {
                    var courseDetails = _catalogCourseService.GetByCourseNo(enrolledCourse.CourseNo);
                    if (!courseDetails.Success)
                    {
                        return;
                    }

                    if (courseDetails.Data.CourseSemester != i + 1)
                    {
                        continue;
                    }

                    var enrolledCourseView = new EnrolledCourseView
                    {
                        CourseName = courseDetails.Data.CourseName,
                        CourseNo = courseDetails.Data.CourseNo,
                        InstructorFullName = "",
                        VizeResult = enrolledCourse.VizeResult,
                        FinalResult = enrolledCourse.FinalResult,
                        ButunlemeResult = enrolledCourse.ButunlemeResult
                    };

                    transcriptTable.AddCell(new PdfPCell(new Phrase($"{courseDetails.Data.CourseNo}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE, MinimumHeight = 25, FixedHeight = 25 });
                    transcriptTable.AddCell(new PdfPCell(new Phrase($"{courseDetails.Data.CourseName}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE, MinimumHeight = 25, FixedHeight = 25 });
                    transcriptTable.AddCell(new PdfPCell(new Phrase($"{courseDetails.Data.Credit}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE, MinimumHeight = 25, FixedHeight = 25 });
                    transcriptTable.AddCell(new PdfPCell(new Phrase($"{enrolledCourseView.GetCompletionGrade()}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE, MinimumHeight = 25, FixedHeight = 25 });
                    transcriptTable.AddCell(new PdfPCell(new Phrase($"{(enrolledCourseView.GetStatus() == null ? "" : enrolledCourseView.GetStatus() == true ? "GEÇTİ" : "KALDI")}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE, MinimumHeight = 25, FixedHeight = 25 });

                    totalCredit += courseDetails.Data.Credit;
                    totalCompleteCredit += enrolledCourseView.GetStatus() == null ? 0 :
                        enrolledCourseView.GetStatus() == true ? courseDetails.Data.Credit : 0;
                    semesterGrade += enrolledCourseView.GetCompletionGrade() == null ? 0 : Convert.ToInt32(enrolledCourseView.GetCompletionGrade());
                }

                transcriptTable.AddCell(new PdfPCell(new Phrase(" ", fontBold)) { Rowspan = 2, Colspan = 2 });
                transcriptTable.AddCell(new PdfPCell(new Phrase("Alınan Ders Sayısı", fontBold)) { BackgroundColor = baseColorYellow });
                transcriptTable.AddCell(new PdfPCell(new Phrase("A/T Kredi", fontBold)) { BackgroundColor = baseColorYellow });
                transcriptTable.AddCell(new PdfPCell(new Phrase("Puan", fontBold)) { BackgroundColor = baseColorYellow });

                transcriptTable.AddCell(new PdfPCell(new Phrase($"{i}", fontNormal)) { BackgroundColor = baseColorYellowLight });
                transcriptTable.AddCell(new PdfPCell(new Phrase($"{totalCredit} | {totalCompleteCredit}", fontNormal)) { BackgroundColor = baseColorYellowLight });
                transcriptTable.AddCell(new PdfPCell(new Phrase($"{semesterGrade}", fontNormal)) { BackgroundColor = baseColorYellowLight });

                transcriptTable.AddCell(new PdfPCell(new Phrase(" "))
                {
                    Colspan = 5,
                    MinimumHeight = 10,
                    FixedHeight = 10,
                });
            }

            pdfDocument.Open();
            pdfDocument.Add(transcriptTable);
            pdfDocument.Close();
            MessageBox.Show(
                $"PDF formatında transkript alma işleminiz başarıyla tamamlandı. Transkript dosya dizini:\n\n{targetPath}",
                Messages.Successful);
        }
    }
}
