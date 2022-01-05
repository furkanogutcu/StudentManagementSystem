using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using StudentManagementSystem.Application.Helpers;
using StudentManagementSystem.Application.Utilities;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Business.Constants;
using StudentManagementSystem.Core.Entities;
using StudentManagementSystem.Core.Utilities.Others;
using StudentManagementSystem.Core.Utilities.Validation;
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

        private void GetCurrentStudent()
        {
            var studentResult = _studentService.GetByStudentNo(_student.StudentNo);
            if (studentResult.Success)
            {
                _student = studentResult.Data;
            }
        }

        // Other Methods

        private void StudentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var messageDialogResult = MessageBox.Show(Messages.AppClosingConfirmation,
                    Messages.AppClosing, MessageBoxButtons.OKCancel);
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
            var dialogResult = MessageBox.Show(Messages.LogOutConfirmation, Messages.SafetyLogOut, MessageBoxButtons.YesNo);

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
            GetCurrentStudent();
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
                txtProfileInfoDepartment.Text = Messages.CouldNotBeRetrivedFromServer;
                txtProfileInfoAdviser.Text = Messages.CouldNotBeRetrivedFromServer;
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
            GetCurrentStudent();
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

                gradeViewComboBoxItems.Reverse();   // Reverse the list to show the current semester first

                cmbGradeViewSelectSemester.DataSource = gradeViewComboBoxItems;
            }
            else
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingDepartmentDetails}\n\n{departmentResult.Message}", Messages.ServerError);
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
            GetCurrentStudent();
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
                        MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingCourseDetails}\n\n{courseResult.Message}", Messages.ServerError);
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

                var gradeViewComboBoxItems = new List<string>();
                var studentCurrentSemester = _student.Semester;
                var studentEnrollmentYear = Convert.ToInt32(_student.EnrollmentDate);

                for (int i = 0; i < studentCurrentSemester; i++)
                {
                    var addedYear = Convert.ToInt32(i / 2);
                    var first = "GÜZ";
                    var second = "BAHAR";
                    gradeViewComboBoxItems.Add($"{studentEnrollmentYear + addedYear} - {studentEnrollmentYear + addedYear + 1} {i + 1}. dönem ({(i % 2 == 0 ? first : second)})");
                }

                gradeViewComboBoxItems.Reverse();   // Reverse the list to show the current semester first

                cmbCourseRegisterApprovedCoursesSelectSemester.DataSource = gradeViewComboBoxItems;

                // Available Courses

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

                // Courses on Draft

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
                        MessageBox.Show($"{Messages.SomethingWentWrongWhileFetchingData}\n\n{ErrorMessageBuilder.CreateErrorMessageFromStringList(new List<string> { departmentResult.Message, instructorResult.Message })}", Messages.ServerError);
                        return;
                    }
                }

                SetDataToDataGridView<CatalogCourseView>(dataGridViewCourseRegisterSelectedCourses, columnNames, coursesOnDraftFinalList.ToList<dynamic>());



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

        private void cmbCourseRegisterApprovedCoursesSelectSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            var coursesAlreadyEnrolledResult = _enrolledCourseService.GetAllByStudentNo(_student.StudentNo);

            if (!coursesAlreadyEnrolledResult.Success)
            {
                MessageBox.Show(
                    $"{Messages.SomethingWentWrongWhileGettingStudentsEnrolledCourses}\n\n{coursesAlreadyEnrolledResult.Message}",
                    Messages.ServerError);
                return;
            }

            var coursesAlreadyEnrolled = coursesAlreadyEnrolledResult.Data;

            var columnNames = new List<string>() { "Ders No", "Ders Adı", "Ders Öğretmeni", "Kredi", "Ders Dönemi", "Ders Bölümü" };

            // Enrolled Courses

            var coursesAlreadyEnrolledList = new List<CatalogCourseView>();

            var numberOfCoursesApprovedInTheCurrentSemester = coursesAlreadyEnrolled.Count;

            foreach (var courseAlreadyEnrolled in coursesAlreadyEnrolled)
            {
                var courseResult = _catalogCourseService.GetByCourseNo(courseAlreadyEnrolled.CourseNo);

                if (!courseResult.Success)
                {
                    MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingCourseDetails}\n\n{courseResult.Message}", Messages.ServerError);
                    return;
                }

                if (courseResult.Data.CourseSemester != (_student.Semester - cmbCourseRegisterApprovedCoursesSelectSemester.SelectedIndex)) // Here checked the current semester as the list in the combobox is inverted.
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
                    MessageBox.Show($"{Messages.SomethingWentWrongWhileFetchingData}\n\n{ErrorMessageBuilder.CreateErrorMessageFromStringList(new List<string> { departmentResult.Message, instructorResult.Message })}", Messages.ServerError);
                    return;
                }
            }

            SetDataToDataGridView<CatalogCourseView>(dataGridViewCourseRegisterApprovedCourses, columnNames, coursesAlreadyEnrolledList.ToList<dynamic>());

            tabControlCourseRegister.TabPages["tabPageApprovedCourses"].Text =
                $"Onaylanan Dersler ({numberOfCoursesApprovedInTheCurrentSemester} ders)";
        }

        private void btnGlobalTranscript_Click(object sender, EventArgs e)
        {
            GetCurrentStudent();
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
                var selection = MessageBox.Show(message, Messages.UpdateConfirmation, MessageBoxButtons.YesNo);
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
                            var selection = MessageBox.Show(Messages.PasswordChangeConfirmation, Messages.PasswordChanging, MessageBoxButtons.YesNo);

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
            var selectedSemester = _student.Semester - cmbGradeViewSelectSemester.SelectedIndex; // Here checked the current semester as the list in the combobox is inverted.
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
                        MessageBox.Show($"{Messages.SomethingWentWrongWhileFetchingData}\n\n{ErrorMessageBuilder.CreateErrorMessageFromStringList(new List<string> { courseResult.Message, instructorResult.Message })}", Messages.ServerError);
                        return;
                    }
                }

                var columnNames = new List<string>() { "Ders No", "Ders Adı", "Ders Öğretmeni", "Vize Notu", "Final Notu", "Bütünleme Notu", "Başarı Notu", "Geçme Durumu" };

                SetDataToDataGridView<EnrolledCourseView>(dataGridViewGradeViewGrades, columnNames, finalList.ToList<dynamic>());
            }
            else
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingStudentsEnrolledCourses}\n\n{studentEnrolledCoursesResult.Message}", Messages.ServerError);
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
                MessageBox.Show(Messages.YouMustSelectAtLeastOneCourse, Messages.Warning);
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

            var dialogResult = MessageBox.Show($"Aşağıdaki dersler taslağa gönderilecek. {Messages.DoYouConfirm}\n\n{message}", Messages.SendToDraftConfirmation, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                foreach (var selectedCourse in selectedCourses)
                {
                    _coursesOnDraft.Add(selectedCourse);
                }

                ReBuildCourseRegisterPanel();

                MessageBox.Show(Messages.SuccesfullySavedToDraft, Messages.Successful);
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
                MessageBox.Show(Messages.YouMustSelectAtLeastOneCourse, Messages.Warning);
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

            var dialogResult = MessageBox.Show($"Aşağıdaki dersleri taslaktan silinecek. {Messages.DoYouConfirm} (İlgili dersler tekrar 'Açılan dersler' ekranında listelenecek)\n\n{message}", Messages.DeleteToDraftConfirmation, MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                foreach (var selectedCourse in selectedCourses)
                {
                    _coursesOnDraft.Remove(selectedCourse);
                }

                ReBuildCourseRegisterPanel();

                MessageBox.Show(Messages.SuccesfullyDeletedToDraft, Messages.Successful);
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

            var dialogResult = MessageBox.Show($"Aşağıdaki dersler danışman onayına gönderilecek. {Messages.DoYouConfirm}\n\n{message}\nDerslerin toplam kredisi: {totalCourseCredit} kredi", Messages.SendToAdviserApprovalConfirmation, MessageBoxButtons.YesNo);
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
                    MessageBox.Show(Messages.SuccessfulySendToAdviser, Messages.Successful);
                }
                ReBuildCourseRegisterPanel();
            }
        }

        private void btnTranscriptGetTranscriptPDF_Click(object sender, EventArgs e)
        {
            var departmentResult = _departmentService.GetByDepartmentNo(_student.DepartmentNo);
            var enrolledCourseResult = _enrolledCourseService.GetAllByStudentNo(_student.StudentNo);

            if (!departmentResult.Success || !enrolledCourseResult.Success)
            {
                MessageBox.Show(Messages.SomethingWentWrongWhileFetchingData, Messages.ServerError);
                return;
            }

            var enrolledCourseDict = new Dictionary<EnrolledCourse, CatalogCourse>();

            foreach (var enrolledCourse in enrolledCourseResult.Data)
            {
                var courseDetail = _catalogCourseService.GetByCourseNo(enrolledCourse.CourseNo);
                if (courseDetail.Success)
                {
                    enrolledCourseDict.Add(enrolledCourse, courseDetail.Data);
                }
                else
                {
                    MessageBox.Show(Messages.SomethingWentWrongWhileFetchingData, Messages.ServerError);
                    return;
                }
            }

            var pdfBody = PDFHelper.CreateA4TranskriptBody(_student, departmentResult.Data, enrolledCourseDict, false);

            Document pdfDocument = new Document(PageSize.A4, 10, 10, 10, 10);
            var fileName =
                $"{TurkishCharNormalizer.Normalization(_student.FirstName.ToLower())}_{TurkishCharNormalizer.Normalization(_student.LastName.ToLower())}_transkript";
            var targetPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\{fileName}.pdf";
            PdfWriter writer = PdfWriter.GetInstance(pdfDocument, new FileStream(targetPath, FileMode.Create));

            pdfDocument.Open();
            pdfDocument.Add(pdfBody);
            pdfDocument.Close();
            MessageBox.Show(
                $"PDF formatında transkript alma işleminiz başarıyla tamamlandı. Transkript dosya dizini:\n\n{targetPath}",
                Messages.Successful);
        }
    }
}
