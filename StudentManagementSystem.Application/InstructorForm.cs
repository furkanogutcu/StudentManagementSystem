using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using StudentManagementSystem.Application.Utilities;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Business.Constants;
using StudentManagementSystem.Business.DependencyResolvers.Autofac;
using StudentManagementSystem.Core.Utilities.Validation;
using StudentManagementSystem.Entities.Concrete;
using StudentManagementSystem.Entities.Views;

namespace StudentManagementSystem.Application
{
    public partial class InstructorForm : Form
    {
        private Instructor _instructor;
        private List<CatalogCourse> _availableCourses = new List<CatalogCourse>();
        private List<AdviserApproval> _coursesPendingApproval = new List<AdviserApproval>();
        private readonly IDepartmentService _departmentService;
        private readonly IInstructorService _instructorService;
        private readonly ICatalogCourseService _catalogCourseService;
        private readonly IStudentService _studentService;
        private readonly IEnrolledCourseService _enrolledCourseService;
        private readonly IAdviserApprovalService _adviserApprovalService;
        private readonly List<Panel> _panels = new List<Panel>();
        private readonly LoginForm _application;

        public InstructorForm(Instructor instructor, LoginForm application)
        {
            _instructor = instructor;
            _application = application;
            _departmentService = InstanceFactory.GetInstance<IDepartmentService>();
            _instructorService = InstanceFactory.GetInstance<IInstructorService>();
            _catalogCourseService = InstanceFactory.GetInstance<ICatalogCourseService>();
            _studentService = InstanceFactory.GetInstance<IStudentService>();
            _enrolledCourseService = InstanceFactory.GetInstance<IEnrolledCourseService>();
            _adviserApprovalService = InstanceFactory.GetInstance<IAdviserApprovalService>();
            InitializeComponent();
            _panels.Add(pnlGlobalProfile);
            _panels.Add(pnlGlobalGradeOperations);
            _panels.Add(pnlGlobalAdviserOperations);
        }

        private void GetCurrentInstructor()
        {
            var instructorResult = _instructorService.GetByInstructorNo(_instructor.InstructorNo);
            if (instructorResult.Success)
            {
                _instructor = instructorResult.Data;
            }
        }

        // Other Methods

        private void InstructorForm_FormClosing(object sender, FormClosingEventArgs e)
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
                this.FormClosing -= InstructorForm_FormClosing;
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
            GetCurrentInstructor();
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
                txtProfileInfoDepartment.Text = Messages.CouldNotBeRetrivedFromServer;
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
            GetCurrentInstructor();
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
                MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingDepartmentDetails}\n\n{departmentResult.Message}", Messages.ServerError);
            }
        }

        // Adviser Operations

        private void btnGlobalAdviserOperations_Click(object sender, EventArgs e)
        {
            PanelSwitcher.ShowPanel(pnlGlobalAdviserOperations, _panels);
            ReBuildAdviserOperationsPanel();
        }

        private void ReBuildAdviserOperationsPanel()
        {
            GetCurrentInstructor();
            PanelCleaner.Clean(pnlGlobalAdviserOperations);
            var studentListResult = _studentService.GetAllByAdvisorNo(_instructor.InstructorNo);

            if (studentListResult.Success)
            {
                if (studentListResult.Data.Count > 0)
                {
                    grbxAdviserOperationsShowOperations.Visible = true;
                    lblAdviserThereIsNoStudentYouAreAdviser.Visible = false;
                    DataSetterToBoxes.SetDataToListBox<Student>(listBoxAdviserOperationsStudentList, studentListResult.Data);
                }
                else
                {
                    lblAdviserThereIsNoStudentYouAreAdviser.Visible = true;
                    grbxAdviserOperationsShowOperations.Visible = false;
                }
            }
            else
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingCurrentStudents}\n\n{studentListResult.Message}", Messages.ServerError);
            }
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

                var selection = MessageBox.Show(message, Messages.UpdateConfirmation, MessageBoxButtons.YesNo);

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
                            MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingNewProfileInfos}:\n\n{instructorGetResult.Message}\n\n{Messages.ApplicationIsRestarting}..", Messages.ServerError);
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
                            var selection = MessageBox.Show(Messages.PasswordChangeConfirmation, Messages.PasswordChanging, MessageBoxButtons.YesNo);

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
                    if (catalogCourse.CourseSemester == Convert.ToInt32(cmbGradeOperationsSelectSemester.SelectedItem))
                    {
                        selectedCourses.Add(catalogCourse);
                    }
                }

                DataSetterToBoxes.SetDataToListBox<CatalogCourse>(listBoxGradeOperationsCurrentCourses, selectedCourses);

                lblGradeOperationsTotalCourse.Text = $"Verilen Dersler ({selectedCourses.Count} ders)";
            }
            else
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingCurrentCourses}\n\n{catalogCourses.Message}", Messages.ServerError);
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

            var selectedCourse = (CatalogCourse)listBoxGradeOperationsCurrentCourses.SelectedItem;

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
                        MessageBox.Show($"{Messages.SomethingWentWrongWhileFetchingData}\n\n{enrolledCourseResult.Message}", Messages.ServerError);
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
                MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingStudentDetails}\n\n{studentsResult.Message}", Messages.ServerError);
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
                    MessageBox.Show($"{Messages.SomethingWentWrongWhileFetchingData}\n\n{enrolledCourseResult.Message}", Messages.ServerError);
                }
            }
            else
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingInstructorDetails}\n\n{instructorResult.Message}", Messages.ServerError);
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
                MessageBox.Show(Messages.YouMustFirstEnterGrade, Messages.Warning);
                return;
            }

            if (txtGradeOperationsInputFinal.Text == string.Empty &&
                txtGradeOperationsInputButunleme.Text != string.Empty)
            {
                MessageBox.Show(Messages.FinalGradeBeforeTheButunlemeGrade, Messages.Warning);
                return;
            }

            if (listBoxGradeOperationsCurrentCourses.SelectedItem == null ||
                listBoxGradeOperationsCourseStudents.SelectedItem == null)
            {
                MessageBox.Show(Messages.YouMustFirstSelectACourseAndStudent, Messages.Warning);
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
                    $"Seçili öğrencinin puanları aşağıdaki gibi güncellenecek. {Messages.DoYouConfirm}\n\n- Vize notu: {txtGradeOperationsInputVize.Text}\n\n- Final notu: {txtGradeOperationsInputFinal.Text}\n\n- Bütünleme notu: {txtGradeOperationsInputButunleme.Text}";
                if (txtGradeOperationsInputFinal.Text != string.Empty ||
                    txtGradeOperationsInputButunleme.Text != string.Empty)
                {
                    message += $"\n\n- Öğrenci ortalaması: {enrolledCourseView.GetCompletionGrade()}\n- Öğrenci geçme/kalma durumu : {(enrolledCourseView.GetStatus() == null ? "" : enrolledCourseView.GetStatus() == true ? "GEÇTİ" : "KALDI")}";
                }
                var dialogResult = MessageBox.Show(message, Messages.GradeEntryConfirmation, MessageBoxButtons.YesNo);
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
                        VizeResult = enrolledCourseView.VizeResult,
                        FinalResult = enrolledCourseView.FinalResult,
                        ButunlemeResult = enrolledCourseView.ButunlemeResult
                    };

                    var updateResult = _enrolledCourseService.Update(updatedEnrolledCourse);
                    if (updateResult.Success)
                    {
                        MessageBox.Show(Messages.GradeEntryComplete, Messages.Successful);
                        listBoxGradeOperationsCourseStudents_SelectedIndexChanged(this, EventArgs.Empty);    //Rebuild screen
                    }
                    else
                    {
                        MessageBox.Show($"{Messages.SomethingWentWrongWhileUpdate}\n\n{updateResult.Message}", Messages.ServerError);
                    }
                }
            }
            else
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileFetchingData}\n\n{enrolledCourseResult.Message}", Messages.ServerError);
            }
        }

        // Adviser Operations

        private void listBoxAdviserOperationsStudentList_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = $@"{((Student)e.ListItem).FirstName} {((Student)e.ListItem).LastName}";
        }

        private void listBoxAdviserOperationsStudentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxAdviserOperationsStudentList.SelectedItem == null)
            {
                chckListBoxAdviserOperationsAvailableCourseList.DataSource = null;
                chckListBoxAdviserOperationsAvailableCourseList.Items.Clear();
                chckListBoxAdviserOperationsFinalCourseList.DataSource = null;
                chckListBoxAdviserOperationsFinalCourseList.Items.Clear();
                txtAdviserOperationsInfoStudentNo.Clear();
                txtAdviserOperationsInfoStudentFullName.Clear();
                txtAdviserOperationsInfoDepartment.Clear();
                txtAdviserOperationsInfoSemester.Clear();
                return;
            }

            var selectedStudent = (Student)listBoxAdviserOperationsStudentList.SelectedItem;

            var departmentResult = _departmentService.GetByDepartmentNo(selectedStudent.DepartmentNo);

            if (departmentResult.Success)
            {
                txtAdviserOperationsInfoStudentNo.Text = selectedStudent.StudentNo.ToString();
                txtAdviserOperationsInfoStudentFullName.Text = $"{selectedStudent.FirstName} {selectedStudent.LastName}";
                txtAdviserOperationsInfoDepartment.Text = departmentResult.Data.DepartmentName;
                txtAdviserOperationsInfoSemester.Text = selectedStudent.Semester.ToString();

                var approvalCourseListResult = _adviserApprovalService.GetAllByStudentNo(selectedStudent.StudentNo);
                var availableCourseListResult =
                    _catalogCourseService.GetAllByDepartmentNoAndSemesterNo(selectedStudent.DepartmentNo,
                        selectedStudent.Semester);

                var currentEnrolledCoursesResult = _enrolledCourseService.GetAllByStudentNo(selectedStudent.StudentNo);

                if (approvalCourseListResult.Success && availableCourseListResult.Success && currentEnrolledCoursesResult.Success)
                {
                    _availableCourses.Clear();
                    _coursesPendingApproval.Clear();

                    // Export availableCourseList to a tempList list to be able to check it
                    var tempList = new List<CatalogCourse>();
                    foreach (var catalogCourse in availableCourseListResult.Data)
                    {
                        tempList.Add(catalogCourse);
                    }

                    // If there is a course pending approval among the accessible courses, remove it from the list.
                    foreach (var availableCatalogCourse in tempList)
                    {
                        if (approvalCourseListResult.Data.Any(a => a.CatalogCourseCode == availableCatalogCourse.CourseNo))
                        {
                            availableCourseListResult.Data.Remove(availableCatalogCourse);
                        }
                    }

                    foreach (var enrolledCourse in currentEnrolledCoursesResult.Data)
                    {
                        if (availableCourseListResult.Data.Any(a => a.CourseNo == enrolledCourse.CourseNo))
                        {
                            var deletedCourse = availableCourseListResult.Data.Find(a => a.CourseNo == enrolledCourse.CourseNo);
                            availableCourseListResult.Data.Remove(deletedCourse);
                        }
                    }

                    // Lists are now available. Export them to global lists

                    foreach (var catalogCourse in availableCourseListResult.Data)
                    {
                        _availableCourses.Add(catalogCourse);
                    }

                    foreach (var adviserApproval in approvalCourseListResult.Data)
                    {
                        _coursesPendingApproval.Add(adviserApproval);
                    }

                    // Show global listings on screen

                    DataSetterToBoxes.SetDataToCheckedListBox<CatalogCourse>(chckListBoxAdviserOperationsAvailableCourseList, _availableCourses);

                    DataSetterToBoxes.SetDataToCheckedListBox<AdviserApproval>(chckListBoxAdviserOperationsFinalCourseList, _coursesPendingApproval);
                }
                else
                {
                    MessageBox.Show($"{Messages.SomethingWentWrongWhileFetchingData}\n\n{ErrorMessageBuilder.CreateErrorMessageFromStringList(new List<string> { approvalCourseListResult.Message, availableCourseListResult.Message, currentEnrolledCoursesResult.Message })}", Messages.ServerError);
                }
            }
            else
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingDepartmentDetails}\n\n{departmentResult.Message}", Messages.ServerError);
            }
        }

        private void chckListBoxAdviserOperationsFinalCourseList_Format(object sender, ListControlConvertEventArgs e)
        {
            var adviserApproval = ((AdviserApproval)e.ListItem);
            var course = _catalogCourseService.GetByCourseNo(adviserApproval.CatalogCourseCode);
            e.Value = $"{(course.Success ? course.Data.CourseName : Messages.CouldNotBeRetrivedFromServer)}";
        }

        private void btnAdviserOperationsAddToFinalList_Click(object sender, EventArgs e)
        {
            if (chckListBoxAdviserOperationsAvailableCourseList.CheckedItems.Count == 0)
            {
                MessageBox.Show(Messages.YouMustSelectAtLeastOneCourse, Messages.Warning);
                return;
            }

            var dialogResult =
                MessageBox.Show(
                    $"Havuzdaki {chckListBoxAdviserOperationsAvailableCourseList.CheckedItems.Count} ders Onay Bekleyen Dersler listesine eklenecek. {Messages.DoYouConfirm}",
                    "Aktarım Onayı", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                var selectedStudent = (Student)listBoxAdviserOperationsStudentList.SelectedItem;

                foreach (var checkedItem in chckListBoxAdviserOperationsAvailableCourseList.CheckedItems)
                {
                    var newAdviserApproval = new AdviserApproval
                    {
                        StudentNo = selectedStudent.StudentNo,
                        Id = 1,
                        CatalogCourseCode = ((CatalogCourse)checkedItem).CourseNo
                    };
                    var addedResult = _adviserApprovalService.Add(newAdviserApproval);
                    if (addedResult.Success)
                    {
                        _availableCourses.Remove((CatalogCourse)checkedItem);
                        _coursesPendingApproval.Add(newAdviserApproval);
                    }
                }
                ReBuildAdviserOperationsPanel();
            }
        }

        private void btnAdviserOperationsDeleteFromFinalList_Click(object sender, EventArgs e)
        {
            if (chckListBoxAdviserOperationsFinalCourseList.CheckedItems.Count == 0)
            {
                MessageBox.Show(Messages.YouMustSelectAtLeastOneCourse, Messages.Warning);
                return;
            }

            var dialogResult =
                MessageBox.Show(
                    $"Onay Bekleyen Dersler listesindeki {chckListBoxAdviserOperationsFinalCourseList.CheckedItems.Count} ders tekrar Eklenebilir Havuz Listesi'ne eklenecek. {Messages.DoYouConfirm}",
                    Messages.TransferConfirmation, MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                foreach (var checkedItem in chckListBoxAdviserOperationsFinalCourseList.CheckedItems)
                {
                    _adviserApprovalService.Delete((AdviserApproval)checkedItem);
                }
                ReBuildAdviserOperationsPanel();
            }
        }

        private void btnAdviserOperationsSubmitFinalList_Click(object sender, EventArgs e)
        {
            var dialogResult =
                MessageBox.Show(
                    $"Onay Bekleyen Dersler listesindeki {_coursesPendingApproval.Count} ders onaylanacak. {Messages.DoYouConfirm}",
                    Messages.ApprovalConfirmation, MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                foreach (var adviserApproval in _coursesPendingApproval)
                {
                    var enrolledCourse = new EnrolledCourse
                    {
                        Id = 1,
                        StudentNo = adviserApproval.StudentNo,
                        CourseNo = adviserApproval.CatalogCourseCode,
                        VizeResult = null,
                        FinalResult = null,
                        ButunlemeResult = null
                    };

                    var result = _enrolledCourseService.Add(enrolledCourse);

                    if (result.Success)
                    {
                        _adviserApprovalService.Delete(adviserApproval);
                    }
                }
                ReBuildAdviserOperationsPanel();
            }
        }
    }
}
