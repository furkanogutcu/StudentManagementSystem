using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using StudentManagementSystem.Application.Utilities;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Business.Constants;
using StudentManagementSystem.Core.Entities;
using StudentManagementSystem.Core.Utilities.Others;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.Core.Utilities.Validation;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Application
{
    public partial class OfficerForm : Form
    {
        private Officer _officer;
        private readonly List<Panel> _panels = new List<Panel>();
        private readonly IOfficerService _officerService;
        private readonly IDepartmentService _departmentService;
        private readonly IInstructorService _instructorService;
        private readonly IStudentService _studentService;
        private readonly ICatalogCourseService _catalogCourseService;

        public OfficerForm(Officer officer, IOfficerService officerService, IDepartmentService departmentService, IInstructorService instructorService, IStudentService studentService, ICatalogCourseService catalogCourseService)
        {
            _officer = officer;
            _officerService = officerService;
            _departmentService = departmentService;
            _instructorService = instructorService;
            _studentService = studentService;
            _catalogCourseService = catalogCourseService;
            InitializeComponent();
            _panels.Add(pnlGlobalProfile);
            _panels.Add(pnlGlobalDepartmentOperations);
            _panels.Add(pnlGlobalCourseOperations);
            _panels.Add(pnlGlobalInstructorOperations);
            _panels.Add(pnlGlobalStudentOperations);
            _panels.Add(pnlGlobalAssignAdviser);
        }

        // Private Methods

        //Profile
        private void btnGlobalProfileOperations_Click(object sender, System.EventArgs e)
        {
            PanelSwitcher.ShowPanel(pnlGlobalProfile, _panels);
            ReBuildProfilePanel();
        }

        private void ReBuildProfilePanel()
        {
            PanelCleaner.Clean(pnlGlobalProfile);
            txtProfileOfficerNo.Text = _officer.OfficerNo.ToString();
            txtProfileInfoFirstName.Text = _officer.FirstName;
            txtProfileInfoLastName.Text = _officer.LastName;
            txtProfileInfoEmail.Text = _officer.Email;
            txtProfileInfoPhone.Text = _officer.Phone;
            if (_officer.ModifiedAt != null)
            {
                lblProfileLastProfileUpdate.Text = $@"Son profil güncellemesi: {_officer.ModifiedAt}";
            }
            else
            {
                lblProfileLastProfileUpdate.Text = string.Empty;
                lblProfileLastProfileUpdate.Visible = false;
            }
        }

        //Department
        private void btnGlobalDepartmentOperations_Click(object sender, System.EventArgs e)
        {
            PanelSwitcher.ShowPanel(pnlGlobalDepartmentOperations, _panels);
            ReBuildDepartmentPanel();
        }

        private void ReBuildDepartmentPanel()
        {
            PanelCleaner.Clean(pnlGlobalDepartmentOperations);
            var departmentResult = _departmentService.GetAll();
            if (departmentResult.Success)
            {
                DataSetterToBoxes.SetDataToListBox<Department>(listBoxDepartmentOperationsCurrentDepartments, departmentResult.Data);
                grbxDepartmentOperationsCurrentDepartments.Text = $@"Mevcut bölümler (Toplam kayıtlı bölüm sayısı: {departmentResult.Data.Count})";
            }
            else
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingCurrentDepartments}:\n\n{departmentResult.Message}", Messages.ServerError);
            }
        }

        //Course
        private void btnGlobalCourseOperations_Click(object sender, System.EventArgs e)
        {
            PanelSwitcher.ShowPanel(pnlGlobalCourseOperations, _panels);
            ReBuildCoursePanel();
        }

        private void ReBuildCoursePanel()
        {
            PanelCleaner.Clean(pnlGlobalCourseOperations);
            var courseResult = _catalogCourseService.GetAll();
            var departmentListForUpdateResult = _departmentService.GetAll();
            var instructorListForUpdateResult = _instructorService.GetAll();
            var semesterListResult = _departmentService.GetAListUpToTheMaxNumberOfSemester();
            if (courseResult.Success && departmentListForUpdateResult.Success && instructorListForUpdateResult.Success && semesterListResult.Success)
            {
                grbxCourseOperationsCurrentCourses.Text = $@"Dersleri listele (Toplam kayıtlı ders sayısı: {courseResult.Data.Count})";
                DataSetterToBoxes.SetDataToComboBox<Department>(cmbCourseOperationsCourseInfoDepartment, departmentListForUpdateResult.Data);
                DataSetterToBoxes.SetDataToComboBox<Instructor>(cmbCourseOperationsCourseInfoInstructor, instructorListForUpdateResult.Data);
                DataSetterToBoxes.SetDataToComboBox<Department>(cmbCourseOperationsAddCourseDepartment, departmentListForUpdateResult.Data);
                DataSetterToBoxes.SetDataToComboBox<Instructor>(cmbCourseOperationsAddCourseInstructor, instructorListForUpdateResult.Data);
                DataSetterToBoxes.SetDataToComboBox<Department>(cmbCourseOperationsFilterCourseDepartment, departmentListForUpdateResult.Data);
                cmbCourseOperationsFilterCourseSemester.DataSource = semesterListResult.Data;
                DataSetterToBoxes.SetDataToListBox<CatalogCourse>(listBoxCourseOperationsListCourses, courseResult.Data);
            }
            else
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingCurrentCourses}:\n\n{ErrorMessageBuilder.CreateErrorMessageFromStringList(new List<string> { courseResult.Message, departmentListForUpdateResult.Message, instructorListForUpdateResult.Message, semesterListResult.Message })}", Messages.ServerError);
            }
        }

        //Instructor
        private void btnGlobalInstructorOperations_Click(object sender, System.EventArgs e)
        {
            PanelSwitcher.ShowPanel(pnlGlobalInstructorOperations, _panels);
            ReBuildInstructorPanel();
        }

        private void ReBuildInstructorPanel()
        {
            PanelCleaner.Clean(pnlGlobalInstructorOperations);
            var instructorResult = _instructorService.GetAll();
            var departmentResult = _departmentService.GetAll();
            if (instructorResult.Success && departmentResult.Success)
            {
                DataSetterToBoxes.SetDataToComboBox<Department>(cmbInstructorOperationsFilterByDepartment, departmentResult.Data);
                DataSetterToBoxes.SetDataToComboBox<Department>(cmbInstructorOperationsAddAvailableDepartmentNames, departmentResult.Data);
                DataSetterToBoxes.SetDataToComboBox<Department>(cmbInstructorOperationsAssignInstructorToDepartmentDepartmentsNames, departmentResult.Data);
                DataSetterToBoxes.SetDataToComboBox<Instructor>(cmbInstructorOperationsAssignInstructorToDepartmentInstructors, instructorResult.Data);
                DataSetterToBoxes.SetDataToListBox<Instructor>(listBoxInstructorOperationsInstructorList, instructorResult.Data);
                grbxInstructorOperationsCurrentInstructors.Text = $@"Öğretim Görevlisi Listesi (Toplam kayıtlı öğretim görevlisi sayısı: {instructorResult.Data.Count})";
            }
            else
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingCurrentInstructors}:\n\n{ErrorMessageBuilder.CreateErrorMessageFromStringList(new List<string> { instructorResult.Message, departmentResult.Message })}", Messages.ServerError);
            }
        }

        //Student
        private void btnGlobalStudentOperations_Click(object sender, System.EventArgs e)
        {
            PanelSwitcher.ShowPanel(pnlGlobalStudentOperations, _panels);
            ReBuildStudentPanel();
        }

        private void ReBuildStudentPanel()
        {
            PanelCleaner.Clean(pnlGlobalStudentOperations);
            var studentResult = _studentService.GetAll();
            var instructorResult = _instructorService.GetAll();
            var departmentResult = _departmentService.GetAll();
            if (studentResult.Success && instructorResult.Success && departmentResult.Success)
            {
                DataSetterToBoxes.SetDataToComboBox<Department>(cmbStudentOperationsAddDepartment, departmentResult.Data);
                DataSetterToBoxes.SetDataToComboBox<Department>(cmbStudentOperationsInfoDepartment, departmentResult.Data);
                DataSetterToBoxes.SetDataToComboBox<Instructor>(cmbStudentOperationsAddAdviser, instructorResult.Data);
                DataSetterToBoxes.SetDataToComboBox<Department>(cmbStudentOperationsFilterDepartmentList, departmentResult.Data);
                DataSetterToBoxes.SetDataToComboBox<Instructor>(cmbStudentOperationsFilterAdviserList, instructorResult.Data);
                DataSetterToBoxes.SetDataToListBox<Student>(listBoxStudentOperationsStudentList, studentResult.Data);
                grbxStudentOperationsCurrentStudents.Text = $@"Öğrenci Listesi (Toplam kayıtlı öğrenci sayısı: {studentResult.Data.Count})";
            }
            else
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingCurrentStudents}:\n\n{ErrorMessageBuilder.CreateErrorMessageFromStringList(new List<string> { studentResult.Message, instructorResult.Message, departmentResult.Message })}");
            }
        }

        //Assign advisor
        private void btnGlobalAssignAdvisor_Click(object sender, System.EventArgs e)
        {
            PanelSwitcher.ShowPanel(pnlGlobalAssignAdviser, _panels);
        }

        // GLOBAL PRIVATE METHODS

        private void AddOperationForAddButtons<T>(IEntityCrudService<T> service, T addedEntity, Delegate methodToRunAfterAdd)
            where T : class, IEntity, new()
        {
            var addResult = service.Add(addedEntity);
            if (addResult.Success)
            {
                MessageBox.Show(Messages.AdditionComplete, Messages.Successful);
                methodToRunAfterAdd.DynamicInvoke();
            }
            else
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileAddition}:\n\n{addResult.Message}", Messages.ServerError);
            }
        }

        private void DeleteOperationForDeleteButtons<T>(IEntityCrudService<T> service, T deletedEntity, Delegate methodToRunAfterDeletion)
            where T : class, IEntity, new()
        {
            var deleteResult = service.Delete(deletedEntity);
            if (deleteResult.Success)
            {
                MessageBox.Show(Messages.DeletionComplete, Messages.Successful);
                methodToRunAfterDeletion.DynamicInvoke();
            }
            else
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileDeletion}:\n\n{deleteResult.Message}", Messages.ServerError);
            }
        }

        private void UpdateOperationForUpdateButtons<T>(IEntityCrudService<T> service, T updatedEntity, Delegate methodToRunAfterUpdate)
            where T : class, IEntity, new()
        {
            var updateResult = service.Update(updatedEntity);
            if (updateResult.Success)
            {
                MessageBox.Show(Messages.UpdateComplete, Messages.Successful);
                methodToRunAfterUpdate.DynamicInvoke();
            }
            else
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileUpdate}:\n\n{updateResult.Message}", Messages.ServerError);
            }
        }

        private void SwitchUpdateTextBoxEnabled(CheckBox controlCheckBox, TextBox targetTextBox)
        {
            targetTextBox.Enabled = true;
            targetTextBox.ReadOnly = !controlCheckBox.Checked;
        }
        private void SwitchUpdateComboBoxEnabled(CheckBox controlCheckBox, ComboBox controlComboBox)
        {
            controlComboBox.Enabled = controlCheckBox.Checked;
        }

        private void SetComboBoxSelectedItem<T>(ComboBox comboBox, string primaryField)
            where T : class, IEntity, new()
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                var selectedItem = comboBox.Items[i];

                bool condition;

                switch (typeof(T).Name)
                {
                    case nameof(Department):
                        condition = ((Department)selectedItem).DepartmentNo == Convert.ToInt32(primaryField);
                        break;
                    case nameof(Instructor):
                        condition = ((Instructor)selectedItem).InstructorNo == Convert.ToInt32(primaryField);
                        break;
                    default:
                        condition = false;
                        break;
                }

                if (condition)
                {
                    comboBox.SelectedItem = comboBox.Items[i];
                    break;
                }
            }
        }

        private void ApplySearchToListbox<T>(ListBox targetListbox, IDataResult<List<T>> searchResult, List<TextBox> textBoxesToClearAfterProcess)
            where T : class, IEntity, new()
        {
            if (searchResult.Success)
            {
                foreach (var textBox in textBoxesToClearAfterProcess)
                {
                    textBox.Text = string.Empty;
                }
                DataSetterToBoxes.SetDataToListBox<T>(targetListbox, searchResult.Data);
                MessageBox.Show(Messages.CreateSearchResultMessage(searchResult.Data.Count), Messages.Information);
            }
            else
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileSearching}:\n\n{searchResult.Message}", Messages.ServerError);
            }
        }

        // GLOBAL PRIVATE METHODS END

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
                if (txtProfileUpdateEmail.Text != string.Empty) message += $"\nEski email: {_officer.Email} --> Yeni email: {txtProfileUpdateEmail.Text}\n";
                if (txtProfileUpdateFirstName.Text != string.Empty) message += $"\nEski ad: {_officer.FirstName} --> Yeni ad: {txtProfileUpdateFirstName.Text}\n";
                if (txtProfileUpdateLastName.Text != string.Empty) message += $"\nEski soyad: {_officer.LastName} --> Yeni soyad: {txtProfileUpdateLastName.Text}\n";
                if (txtProfileUpdatePhone.Text != string.Empty) message += $"\nEski telefon: {_officer.Phone} --> Yeni telefon: {txtProfileUpdatePhone.Text}\n";
                var selection = MessageBox.Show(message, "Güncellemeyi onaylıyor musunuz?", MessageBoxButtons.YesNo);
                if (selection == DialogResult.Yes)
                {
                    var newOfficer = new Officer()
                    {
                        Email = txtProfileUpdateEmail.Text == string.Empty ? _officer.Email : txtProfileUpdateEmail.Text,
                        FirstName = txtProfileUpdateFirstName.Text == string.Empty ? _officer.FirstName : txtProfileUpdateFirstName.Text,
                        LastName = txtProfileUpdateLastName.Text == string.Empty ? _officer.LastName : txtProfileUpdateLastName.Text,
                        Phone = txtProfileUpdatePhone.Text == string.Empty ? _officer.Phone : txtProfileUpdatePhone.Text,
                        ModifiedAt = DateTime.Now,
                        OfficerNo = _officer.OfficerNo,
                        Password = _officer.Password,
                    };
                    var updateResult = _officerService.Update(newOfficer);
                    if (updateResult.Success)
                    {
                        MessageBox.Show(Messages.ProfileHasBeenUpdated, Messages.Successful);
                        var officerGetResult = _officerService.GetByOfficerNo(_officer.OfficerNo);
                        if (officerGetResult.Success)
                        {
                            _officer = officerGetResult.Data;
                            ReBuildProfilePanel();   // like setState()
                            txtProfileUpdateEmail.Clear();
                            txtProfileUpdateFirstName.Clear();
                            txtProfileUpdateLastName.Clear();
                            txtProfileUpdatePhone.Clear();
                        }
                        else
                        {
                            MessageBox.Show($@"{Messages.SomethingWentWrongWhileGettingNewProfileInfos}:\n\n{updateResult.Message}\n\n{Messages.ApplicationIsRestarting}..", Messages.ServerError);
                            System.Windows.Forms.Application.Restart();
                        }
                    }
                    else
                    {
                        MessageBox.Show($@"{Messages.SomethingWentWrongWhileUpdate}:\n\n{updateResult.Message}", Messages.ServerError);
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
                    var officerResult = _officerService.GetByOfficerNo(_officer.OfficerNo);
                    if (officerResult.Success)
                    {
                        if (txtProfileOldPassword.Text == officerResult.Data.Password)
                        {
                            var updateResult = _officerService.Update(new Officer
                            {
                                Email = _officer.Email,
                                FirstName = _officer.FirstName,
                                LastName = _officer.LastName,
                                OfficerNo = _officer.OfficerNo,
                                Phone = _officer.Phone,
                                Password = txtProfileNewPassword.Text,
                                ModifiedAt = DateTime.Now,
                            });
                            if (updateResult.Success)
                            {
                                MessageBox.Show($@"{Messages.PasswordHasBeenChanged}. {Messages.LoginAgainWithNewPassword}. {Messages.ApplicationIsRestarting}..", Messages.Successful);
                                System.Windows.Forms.Application.Restart();
                            }
                            else
                            {
                                MessageBox.Show($@"{Messages.SomethingWentWrongWhilePasswordChange}:\n\n{updateResult.Message}", Messages.ServerError);
                            }
                        }
                        else
                        {
                            MessageBox.Show(Messages.OldPasswordEnteredDoesNotMatchCurrentPassword, Messages.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show($@"{Messages.SomethingWentWrongWhileCheckPassword}:\n\n{officerResult.Message}", Messages.ServerError);
                    }
                }
            }
            else
            {
                MessageBox.Show(Messages.MakeSureFillInAllFields, Messages.Warning);
            }
        }

        // Department Operations Methods

        private void listBoxDepartmentOperationsCurrentDepartments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxDepartmentOperationsCurrentDepartments.SelectedItem == null)
            {
                return;
            }
            int selectedDepartmentNo = UniqueValueTaker.GetUniqueValueOfSelectedItemInListbox<Department>(listBoxDepartmentOperationsCurrentDepartments);

            var selectedDepartmentResult = _departmentService.GetByDepartmentNo(selectedDepartmentNo);
            var instructorListInDepartmentResult = _instructorService.GetAllByDepartmentNo(selectedDepartmentNo);
            var studentListInDepartmentResult = _studentService.GetAllByDepartmentNo(selectedDepartmentNo);

            if (selectedDepartmentResult.Success && instructorListInDepartmentResult.Success && studentListInDepartmentResult.Success)
            {
                txtDepartmentOperationsDepartmentNo.Text = selectedDepartmentResult.Data.DepartmentNo.ToString();
                txtDepartmentOperationsDepartmentName.Text = selectedDepartmentResult.Data.DepartmentName;
                txtDepartmentOperationsTotalInstructor.Text = instructorListInDepartmentResult.Data.Count.ToString();
                txtDepartmentOperationsTotalStudents.Text = studentListInDepartmentResult.Data.Count.ToString();
                txtDepartmentOperationsNumberOfSemester.Text = selectedDepartmentResult.Data.NumberOfSemester.ToString();
            }
            else
            {
                MessageBox.Show($@"{Messages.SomethingWentWrongWhileGettingDepartmentDetails}:\n\n{ErrorMessageBuilder.CreateErrorMessageFromStringList(
                    new List<string> { selectedDepartmentResult.Message, instructorListInDepartmentResult.Message, studentListInDepartmentResult.Message })}", Messages.ServerError);
            }
        }

        private void btnDepartmentOperationsDeleteDepartment_Click(object sender, EventArgs e)
        {
            if (listBoxDepartmentOperationsCurrentDepartments.SelectedItem == null)
            {
                return;
            }
            int departmentNo = UniqueValueTaker.GetUniqueValueOfSelectedItemInListbox<Department>(listBoxDepartmentOperationsCurrentDepartments);
            var deletedDepartmentResult = _departmentService.GetByDepartmentNo(departmentNo);
            if (deletedDepartmentResult.Success)
            {
                var messageDialogResult = MessageBox.Show($@"{deletedDepartmentResult.Data.DepartmentName} isimli bölüm silinecek. Onaylıyor musunuz?", Messages.DeleteConfirmation, MessageBoxButtons.YesNo);
                if (messageDialogResult == DialogResult.Yes)
                {
                    DeleteOperationForDeleteButtons<Department>(_departmentService, deletedDepartmentResult.Data, ReBuildDepartmentPanel);
                }
            }
            else
            {
                MessageBox.Show(Messages.SomethingWentWrongWhileGettingDepartmentDetails, Messages.ServerError);
            }


        }
        private void btnDepartmentOperationsUpdate_Click(object sender, EventArgs e)
        {
            if (listBoxDepartmentOperationsCurrentDepartments.SelectedItem == null)
            {
                return;
            }
            if (txtDepartmentOperationsUpdateDepartmentName.Text == string.Empty && txtDepartmentOperationsUpdateNumberOfSemester.Text == string.Empty)
            {
                MessageBox.Show(Messages.MustFillInTheFieldsWantToUpdate, Messages.Warning);
                return;
            }

            if (!NumberStringValidator.ValidateString(txtDepartmentOperationsUpdateNumberOfSemester.Text))
            {
                MessageBox.Show(Messages.DepartmentNumberOfSemesterMustConsistOfNumbersOnly, Messages.Warning);
                return;
            }

            int selectedDepartmentNo = UniqueValueTaker.GetUniqueValueOfSelectedItemInListbox<Department>(listBoxDepartmentOperationsCurrentDepartments);
            var updatedDepartmentDetailsResult = _departmentService.GetByDepartmentNo(selectedDepartmentNo);

            if (!updatedDepartmentDetailsResult.Success)
            {
                MessageBox.Show($@"{Messages.SomethingWentWrongWhileGettingDepartmentDetails}:\n\n{updatedDepartmentDetailsResult.Message}", Messages.ServerError);
                return;
            }

            var updateDepartment = new Department
            {
                DepartmentNo = updatedDepartmentDetailsResult.Data.DepartmentNo,
                DepartmentName = txtDepartmentOperationsUpdateDepartmentName.Text == string.Empty ? updatedDepartmentDetailsResult.Data.DepartmentName : txtDepartmentOperationsUpdateDepartmentName.Text,
                NumberOfSemester = txtDepartmentOperationsUpdateNumberOfSemester.Text == string.Empty ? updatedDepartmentDetailsResult.Data.NumberOfSemester : Convert.ToInt32(txtDepartmentOperationsUpdateNumberOfSemester.Text),
                ModifiedAt = updatedDepartmentDetailsResult.Data.ModifiedAt,
                CreatedAt = updatedDepartmentDetailsResult.Data.CreatedAt,
                DeletedAt = updatedDepartmentDetailsResult.Data.DeletedAt,
            };

            if (FindDifferencesInEntities.Find(updateDepartment, updatedDepartmentDetailsResult.Data).Values.Contains(true))
            {
                var messageDialogResult = MessageBox.Show($@"{updatedDepartmentDetailsResult.Data.DepartmentName} isimli bölüm güncellenecek. Onaylıyor musunuz?", Messages.UpdateConfirmation, MessageBoxButtons.YesNo);
                if (messageDialogResult == DialogResult.Yes)
                {
                    updatedDepartmentDetailsResult.Data.DepartmentName = txtDepartmentOperationsUpdateDepartmentName.Text;
                    UpdateOperationForUpdateButtons<Department>(_departmentService, updateDepartment, ReBuildDepartmentPanel);
                }
            }
            else
            {
                MessageBox.Show(Messages.TheDataToBeUpdatedIsTheSameAsTheOldData, Messages.Warning);
            }
        }

        private void btnDepartmentOperationsAddDepartment_Click(object sender, EventArgs e)
        {
            if (txtDepartmentOperationsAddDepartmentName.Text == string.Empty || txtDepartmentOperationsAddNumberOfSemester.Text == string.Empty)
            {
                MessageBox.Show(Messages.MakeSureFillInAllFields, Messages.Warning);
                return;
            }

            if (!NumberStringValidator.ValidateString(txtDepartmentOperationsAddNumberOfSemester.Text))
            {
                MessageBox.Show(Messages.DepartmentNumberOfSemesterMustConsistOfNumbersOnly, Messages.Warning);
                return;
            }

            var addedDepartment = new Department
            {
                DepartmentName = txtDepartmentOperationsAddDepartmentName.Text,
                NumberOfSemester = Convert.ToInt32(txtDepartmentOperationsAddNumberOfSemester.Text),
                DepartmentNo = 1, //Insignificant
            };
            var messageDialogResult = MessageBox.Show($@"{addedDepartment.DepartmentName} isimli yeni bir bölüm eklenecek. Onaylıyor musunuz?", Messages.AddConfirmation, MessageBoxButtons.YesNo);
            if (messageDialogResult == DialogResult.Yes)
            {
                AddOperationForAddButtons<Department>(_departmentService, addedDepartment, ReBuildDepartmentPanel);
            }
        }

        private void btnDepartmentOperationsSearch_Click(object sender, EventArgs e)
        {
            var textBoxesToClearAfterProcess = new List<TextBox>
            {
                txtDepartmentOperationsDepartmentName,
                txtDepartmentOperationsDepartmentNo,
                txtDepartmentOperationsTotalInstructor,
                txtDepartmentOperationsTotalStudents
            };

            if (txtDepartmentOperationsSearchByDepartmentNo.Text != string.Empty &&
                txtDepartmentOperationsSearchByDepartmentName.Text != string.Empty)
            {
                MessageBox.Show(Messages.NotAllSearchCriteriaCanBeFilledAtOnce, Messages.Warning);
            }
            else if (txtDepartmentOperationsSearchByDepartmentNo.Text != string.Empty &&
                     txtDepartmentOperationsSearchByDepartmentName.Text == string.Empty)
            {
                if (!NumberStringValidator.ValidateString(txtDepartmentOperationsSearchByDepartmentNo.Text))
                {
                    MessageBox.Show(Messages.DepartmentNumberMustConsistOfNumbersOnly, Messages.Warning);
                    return;
                }
                var departmentResult = _departmentService.GetAllByDepartmentNo(Convert.ToInt32(txtDepartmentOperationsSearchByDepartmentNo.Text));

                ApplySearchToListbox(listBoxDepartmentOperationsCurrentDepartments, departmentResult, textBoxesToClearAfterProcess);
            }
            else if (txtDepartmentOperationsSearchByDepartmentNo.Text == string.Empty &&
                     txtDepartmentOperationsSearchByDepartmentName.Text != string.Empty)
            {
                var departmentResult = _departmentService.GetAllContainDepartmentName(txtDepartmentOperationsSearchByDepartmentName.Text);
                ApplySearchToListbox(listBoxDepartmentOperationsCurrentDepartments, departmentResult, textBoxesToClearAfterProcess);
            }
            else
            {
                MessageBox.Show(Messages.NotAllSearchCriteriaCanBeFilledAtOnce, Messages.Warning);
            }

        }

        private void btnDepartmentOperationsSearchReset_Click(object sender, EventArgs e)
        {
            ReBuildDepartmentPanel();
        }

        // Course Operations Methods

        private void listBoxCourseOperationsListCourses_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxCourseOperationsListCourses.SelectedItem == null)
            {
                return;
            }

            int selectedCourseNo = UniqueValueTaker.GetUniqueValueOfSelectedItemInListbox<CatalogCourse>(listBoxCourseOperationsListCourses);

            var selectedCourseResult = _catalogCourseService.GetByCourseNo(selectedCourseNo);

            if (selectedCourseResult.Success)
            {
                var departmentResult = _departmentService.GetByDepartmentNo(selectedCourseResult.Data.DepartmentNo);
                var instructorResult = _instructorService.GetByInstructorNo(selectedCourseResult.Data.InstructorNo);

                if (departmentResult.Success && instructorResult.Success)
                {
                    txtCourseOperationsCourseInfoCourseNo.Text = selectedCourseResult.Data.CourseNo.ToString();
                    txtCourseOperationsCourseInfoCourseName.Text = selectedCourseResult.Data.CourseName;

                    SetComboBoxSelectedItem<Department>(cmbCourseOperationsCourseInfoDepartment, departmentResult.Data.DepartmentNo.ToString());    //FIXME
                    SetComboBoxSelectedItem<Instructor>(cmbCourseOperationsCourseInfoInstructor, instructorResult.Data.InstructorNo.ToString());    //FIXME

                    txtCourseOperationsCourseInfoCredit.Text = selectedCourseResult.Data.Credit.ToString();
                    txtCourseOperationsCourseInfoCourseYear.Text = selectedCourseResult.Data.CourseYear.ToString();
                    txtCourseOperationsCourseInfoSemester.Text = selectedCourseResult.Data.CourseSemester.ToString();
                }
                else
                {
                    MessageBox.Show($@"{Messages.SomethingWentWrongWhileGettingCourseDetails}:\n\n{ErrorMessageBuilder.CreateErrorMessageFromStringList(
                        new List<string> { departmentResult.Message, instructorResult.Message })}", Messages.ServerError);
                }
            }
            else
            {
                MessageBox.Show($@"{Messages.SomethingWentWrongWhileGettingDepartmentDetails}:\n\n{selectedCourseResult.Message})", Messages.ServerError);
            }
        }

        private void cmbCourseOperationsCourseInfoInstructor_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = $@"{((Instructor)e.ListItem).FirstName} {((Instructor)e.ListItem).LastName}";
        }

        private void cmbCourseOperationsAddCourseInstructor_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = $@"{((Instructor)e.ListItem).FirstName} {((Instructor)e.ListItem).LastName}";
        }

        private void chbxCourseOperationsCourseInfoCourseNameEnabled_CheckedChanged(object sender, EventArgs e)
        {
            SwitchUpdateTextBoxEnabled(chbxCourseOperationsCourseInfoCourseNameEnabled, txtCourseOperationsCourseInfoCourseName);
        }

        private void chbxCourseOperationsCourseInfoDepartmentEnabled_CheckedChanged(object sender, EventArgs e)
        {
            SwitchUpdateComboBoxEnabled(chbxCourseOperationsCourseInfoDepartmentEnabled, cmbCourseOperationsCourseInfoDepartment);
        }

        private void chbxCourseOperationsCourseInfoInstructorEnabled_CheckedChanged(object sender, EventArgs e)
        {
            SwitchUpdateComboBoxEnabled(chbxCourseOperationsCourseInfoInstructorEnabled, cmbCourseOperationsCourseInfoInstructor);
        }

        private void chbxCourseOperationsCourseInfoCreditEnabled_CheckedChanged(object sender, EventArgs e)
        {
            SwitchUpdateTextBoxEnabled(chbxCourseOperationsCourseInfoCreditEnabled, txtCourseOperationsCourseInfoCredit);
        }

        private void chbxCourseOperationsCourseInfoCourseYearEnabled_CheckedChanged(object sender, EventArgs e)
        {
            SwitchUpdateTextBoxEnabled(chbxCourseOperationsCourseInfoCourseYearEnabled, txtCourseOperationsCourseInfoCourseYear);
        }

        private void chbxCourseOperationsCourseInfoSemesterEnabled_CheckedChanged(object sender, EventArgs e)
        {
            SwitchUpdateTextBoxEnabled(chbxCourseOperationsCourseInfoSemesterEnabled, txtCourseOperationsCourseInfoSemester);
        }

        private void btnCourseOperationsDelete_Click(object sender, EventArgs e)
        {
            if (listBoxCourseOperationsListCourses.SelectedItem == null)
            {
                return;
            }
            int courseNo = UniqueValueTaker.GetUniqueValueOfSelectedItemInListbox<CatalogCourse>(listBoxCourseOperationsListCourses);
            var deletedCourseResult = _catalogCourseService.GetByCourseNo(courseNo);
            if (deletedCourseResult.Success)
            {
                var messageDialogResult = MessageBox.Show($@"{deletedCourseResult.Data.CourseName} isimli ders silinecek. Onaylıyor musunuz?", Messages.DeleteConfirmation, MessageBoxButtons.YesNo);
                if (messageDialogResult == DialogResult.Yes)
                {
                    DeleteOperationForDeleteButtons<CatalogCourse>(_catalogCourseService, deletedCourseResult.Data, ReBuildCoursePanel);
                }
            }
            else
            {
                MessageBox.Show(Messages.SomethingWentWrongWhileGettingCourseDetails, Messages.ServerError);
            }
        }

        private void btnCourseOperationsUpdate_Click(object sender, EventArgs e)
        {
            if (listBoxCourseOperationsListCourses.SelectedItem == null)
            {
                return;
            }
            if (!NumberStringValidator.ValidateString(txtCourseOperationsCourseInfoCredit.Text))
            {
                MessageBox.Show($@"{Messages.CourseCreditMustConsistOfNumbersOnly}", Messages.Warning);
                return;
            }

            if (!NumberStringValidator.ValidateString(txtCourseOperationsCourseInfoCourseYear.Text))
            {
                MessageBox.Show($@"{Messages.CourseYearMustConsistOfNumbersOnly}", Messages.Warning);
                return;
            }

            if (!NumberStringValidator.ValidateString(txtCourseOperationsCourseInfoSemester.Text))
            {
                MessageBox.Show($@"{Messages.CourseSemesterMustConsistOfNumbersOnly}", Messages.Warning);
                return;
            }

            int selectedCourseNo = UniqueValueTaker.GetUniqueValueOfSelectedItemInListbox<CatalogCourse>(listBoxCourseOperationsListCourses);
            var updatedCourseDetailsResult = _catalogCourseService.GetByCourseNo(selectedCourseNo);

            if (!updatedCourseDetailsResult.Success)
            {
                MessageBox.Show($@"{Messages.SomethingWentWrongWhileGettingCourseDetails}:\n\n{updatedCourseDetailsResult.Message}", Messages.ServerError);
                return;
            }

            var updatedCourse = new CatalogCourse
            {
                CourseNo = updatedCourseDetailsResult.Data.CourseNo,
                CourseName = txtCourseOperationsCourseInfoCourseName.Text,
                DepartmentNo = ((Department)cmbCourseOperationsCourseInfoDepartment.SelectedItem).DepartmentNo,
                InstructorNo = ((Instructor)cmbCourseOperationsCourseInfoInstructor.SelectedItem).InstructorNo,
                Credit = Convert.ToInt32(txtCourseOperationsCourseInfoCredit.Text),
                CourseYear = short.Parse(txtCourseOperationsCourseInfoCourseYear.Text),
                CourseSemester = Convert.ToInt32(txtCourseOperationsCourseInfoSemester.Text),
                CreatedAt = updatedCourseDetailsResult.Data.CreatedAt,
                ModifiedAt = updatedCourseDetailsResult.Data.ModifiedAt,
                DeletedAt = updatedCourseDetailsResult.Data.DeletedAt
            };

            var updatedDepartmentName = _departmentService.GetByDepartmentNo(updatedCourse.DepartmentNo).Data.DepartmentName;
            var updatedInstructor = _instructorService.GetByInstructorNo(updatedCourse.InstructorNo).Data;
            var updatedInstructorFullName = $"{updatedInstructor.FirstName} {updatedInstructor.LastName}";

            var message = $"{updatedCourseDetailsResult.Data.CourseName} isimli ders aşağıdaki gibi güncellenecek. Onaylıyor musunuz?\n";
            message += $"\n- Ders adı: {updatedCourse.CourseName}";
            message += $"\n- Bölüm: {updatedDepartmentName}";
            message += $"\n- Öğretim görevlisi {updatedInstructorFullName}";
            message += $"\n- Kredi: {updatedCourse.Credit}";
            message += $"\n- Ders yılı: {updatedCourse.CourseYear}";
            message += $"\n- Ders dönemi: {updatedCourse.CourseSemester}";

            if (FindDifferencesInEntities.Find(updatedCourse, updatedCourseDetailsResult.Data).Values
                .Contains(true))
            {
                var messageDialogResult = MessageBox.Show(message, Messages.UpdateConfirmation, MessageBoxButtons.YesNo);
                if (messageDialogResult == DialogResult.Yes)
                {
                    UpdateOperationForUpdateButtons<CatalogCourse>(_catalogCourseService, updatedCourse, ReBuildCoursePanel);
                }
            }
            else
            {
                MessageBox.Show(Messages.TheDataToBeUpdatedIsTheSameAsTheOldData, Messages.Warning);
            }
        }

        private void btnCourseOperationsAdd_Click(object sender, EventArgs e)
        {
            if (cmbCourseOperationsAddCourseDepartment.Items.Count < 1)
            {
                MessageBox.Show(Messages.ThereMustBeAtLeastOneDepartmentToBeAbleToAddACourse, Messages.Warning);
                return;
            }

            if (cmbCourseOperationsAddCourseInstructor.Items.Count < 1)
            {
                MessageBox.Show(Messages.ThereMustBeAtLeastOneInstructorToBeAbleToAddACourse, Messages.Warning);
                return;
            }
            if (txtCourseOperationsAddCourseCourseName.Text == string.Empty
                || txtCourseOperationsAddCourseCredit.Text == string.Empty
                || txtCourseOperationsAddCourseCourseSemester.Text == string.Empty
                || txtCourseOperationsAddCourseCourseYear.Text == string.Empty)
            {
                MessageBox.Show(Messages.MakeSureFillInAllFields, Messages.Warning);
                return;
            }

            if (!NumberStringValidator.ValidateString(txtCourseOperationsAddCourseCredit.Text))
            {
                MessageBox.Show(Messages.CourseCreditMustConsistOfNumbersOnly, Messages.Warning);
                return;
            }

            if (!NumberStringValidator.ValidateString(txtCourseOperationsAddCourseCourseYear.Text))
            {
                MessageBox.Show(Messages.CourseYearMustConsistOfNumbersOnly, Messages.Warning);
                return;
            }

            if (!NumberStringValidator.ValidateString(txtCourseOperationsAddCourseCourseSemester.Text))
            {
                MessageBox.Show(Messages.CourseSemesterMustConsistOfNumbersOnly, Messages.Warning);
                return;
            }

            var addedCourse = new CatalogCourse
            {
                CourseNo = 1,
                CourseYear = short.Parse(txtCourseOperationsAddCourseCourseYear.Text),
                CourseName = txtCourseOperationsAddCourseCourseName.Text,
                CourseSemester = Convert.ToInt32(txtCourseOperationsAddCourseCourseSemester.Text),
                Credit = Convert.ToInt32(txtCourseOperationsAddCourseCredit.Text),
                DepartmentNo = ((Department)cmbCourseOperationsAddCourseDepartment.SelectedItem).DepartmentNo,
                InstructorNo = ((Instructor)cmbCourseOperationsAddCourseInstructor.SelectedItem).InstructorNo,
            };
            var messageDialogResult = MessageBox.Show($@"{addedCourse.CourseName} isimli yeni bir ders eklenecek. Onaylıyor musunuz?", Messages.AddConfirmation, MessageBoxButtons.YesNo);
            if (messageDialogResult == DialogResult.Yes)
            {
                AddOperationForAddButtons<CatalogCourse>(_catalogCourseService, addedCourse, ReBuildCoursePanel);
            }
        }

        private void btnCourseOperationsSearch_Click(object sender, EventArgs e)
        {
            var textBoxesToClearAfterProcess = new List<TextBox>
            {
                txtCourseOperationsCourseInfoCourseNo,
                txtCourseOperationsCourseInfoCourseName,
                txtCourseOperationsCourseInfoCredit,
                txtCourseOperationsCourseInfoCourseYear,
                txtCourseOperationsCourseInfoSemester
            };

            if (txtCourseOperationsCourseInfoSearchByCourseNo.Text != string.Empty && txtCourseOperationsCourseInfoSearchByCourseName.Text != string.Empty)
            {
                MessageBox.Show(Messages.NotAllSearchCriteriaCanBeFilledAtOnce, Messages.Warning);
            }
            else if (txtCourseOperationsCourseInfoSearchByCourseNo.Text != string.Empty &&
                     txtCourseOperationsCourseInfoSearchByCourseName.Text == string.Empty)
            {
                if (!NumberStringValidator.ValidateString(txtCourseOperationsCourseInfoSearchByCourseNo.Text))
                {
                    MessageBox.Show(Messages.CourseNoMustConsistOfNumbersOnly, Messages.Warning);
                    return;
                }
                var courseResult = _catalogCourseService.GetAllByCourseNo(Convert.ToInt32(txtCourseOperationsCourseInfoSearchByCourseNo.Text));
                ApplySearchToListbox(listBoxCourseOperationsListCourses, courseResult, textBoxesToClearAfterProcess);

            }
            else if (txtCourseOperationsCourseInfoSearchByCourseNo.Text == string.Empty &&
                     txtCourseOperationsCourseInfoSearchByCourseName.Text != string.Empty)
            {
                var courseResult = _catalogCourseService.GetAllContainCourseName(txtCourseOperationsCourseInfoSearchByCourseName.Text);
                ApplySearchToListbox(listBoxCourseOperationsListCourses, courseResult, textBoxesToClearAfterProcess);
            }
            else
            {
                MessageBox.Show(Messages.NotAllSearchCriteriaCanBeFilledAtOnce, Messages.Warning);
            }
        }

        private void btnCourseOperationsSearchReset_Click(object sender, EventArgs e)
        {
            ReBuildCoursePanel();
        }

        private void btnCourseOperationsFilter_Click(object sender, EventArgs e)
        {
            if (!chbxCourseOperationsFilterByDepartment.Checked && !chbxCourseOperationsFilterBySemester.Checked)
            {
                MessageBox.Show(Messages.AtLeastOneFilterMustBeOn, Messages.Warning);
                return;
            }

            var conditions = new List<Func<CatalogCourse, bool>>();

            if (chbxCourseOperationsFilterByDepartment.Checked)
            {
                conditions.Add(c => c.DepartmentNo == Convert.ToInt32(cmbCourseOperationsFilterCourseDepartment.SelectedValue));
            }

            if (chbxCourseOperationsFilterBySemester.Checked)
            {
                conditions.Add(c => c.CourseSemester == Convert.ToInt32(cmbCourseOperationsFilterCourseSemester.SelectedValue));
            }

            FilteringTool.FilterListBox<CatalogCourse>(listBoxCourseOperationsListCourses, _catalogCourseService, conditions, Messages.SomethingWentWrongWhileGettingCurrentCourses);

        }

        private void btnCourseOperationsFilterReset_Click(object sender, EventArgs e)
        {
            ReBuildCoursePanel();
        }

        private void chbxCourseOperationsFilterByDepartment_CheckedChanged(object sender, EventArgs e)
        {
            SwitchUpdateComboBoxEnabled(chbxCourseOperationsFilterByDepartment, cmbCourseOperationsFilterCourseDepartment);

        }

        private void chbxCourseOperationsFilterBySemester_CheckedChanged(object sender, EventArgs e)
        {
            SwitchUpdateComboBoxEnabled(chbxCourseOperationsFilterBySemester, cmbCourseOperationsFilterCourseSemester);
        }

        // Instructor Operations Methods

        private void listBoxInstructorOperationsInstructorList_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = $@"{((Instructor)e.ListItem).FirstName} {((Instructor)e.ListItem).LastName}";
        }

        private void listBoxInstructorOperationsInstructorList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxInstructorOperationsInstructorList.SelectedItem == null)
            {
                return;
            }

            int selectedInstructorNo = UniqueValueTaker.GetUniqueValueOfSelectedItemInListbox<Instructor>(listBoxInstructorOperationsInstructorList);

            var selectedInstructorResult = _instructorService.GetByInstructorNo(selectedInstructorNo);
            var departmentResult = _departmentService.GetByDepartmentNo(selectedInstructorResult.Data.DepartmentNo);

            if (selectedInstructorResult.Success && departmentResult.Success)
            {
                txtInstructorOperationsInfoInstructorNo.Text = selectedInstructorResult.Data.InstructorNo.ToString();
                txtInstructorOperationsInfoDepartmentName.Text = departmentResult.Data.DepartmentName;

                txtInstructorOperationsInfoEmail.Text = selectedInstructorResult.Data.Email;
                txtInstructorOperationsInfoPhone.Text = selectedInstructorResult.Data.Phone;
                txtInstructorOperationsInfoFirstName.Text = selectedInstructorResult.Data.FirstName;
                txtInstructorOperationsInfoLastName.Text = selectedInstructorResult.Data.LastName;
            }
            else
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingDepartmentDetails}:\n\n{ErrorMessageBuilder.CreateErrorMessageFromStringList(new List<string> { selectedInstructorResult.Message, departmentResult.Message })}", Messages.ServerError);
            }
        }

        private void btnInstructorOperationsDelete_Click(object sender, EventArgs e)
        {
            if (listBoxInstructorOperationsInstructorList.SelectedItem == null)
            {
                return;
            }
            int instructorNo = UniqueValueTaker.GetUniqueValueOfSelectedItemInListbox<Instructor>(listBoxInstructorOperationsInstructorList);
            var deletedInstructorResult = _instructorService.GetByInstructorNo(instructorNo);
            if (deletedInstructorResult.Success)
            {
                var messageDialogResult = MessageBox.Show($@"{deletedInstructorResult.Data.FirstName} {deletedInstructorResult.Data.LastName} isimli öğretim görevlisi silinecek. Onaylıyor musunuz?", Messages.DeleteConfirmation, MessageBoxButtons.YesNo);
                if (messageDialogResult == DialogResult.Yes)
                {
                    DeleteOperationForDeleteButtons<Instructor>(_instructorService, deletedInstructorResult.Data, ReBuildInstructorPanel);
                }
            }
            else
            {
                MessageBox.Show(Messages.SomethingWentWrongWhileGettingCourseDetails, Messages.ServerError);
            }
        }

        private void btnInstructorOperationsSave_Click(object sender, EventArgs e)
        {
            if (txtInstructorOperationsAddFirstName.Text == string.Empty
                || txtInstructorOperationsAddLastName.Text == string.Empty
                || txtInstructorOperationsAddEmail.Text == string.Empty
                || txtInstructorOperationsAddPhone.Text == string.Empty
                )
            {
                MessageBox.Show(Messages.MakeSureFillInAllFields, Messages.Warning);
                return;
            }

            if (!NumberStringValidator.ValidateString(txtInstructorOperationsAddPhone.Text))
            {
                MessageBox.Show(Messages.OnlyNumberForPhone, Messages.Warning);
                return;
            }

            var password = $"{TurkishCharNormalizer.Normalization(txtInstructorOperationsAddFirstName.Text.ToLowerInvariant())}.{TurkishCharNormalizer.Normalization(txtInstructorOperationsAddLastName.Text.ToLowerInvariant())}";

            var addedInstructor = new Instructor
            {
                FirstName = txtInstructorOperationsAddFirstName.Text,
                LastName = txtInstructorOperationsAddLastName.Text,
                Email = txtInstructorOperationsAddEmail.Text,
                Phone = txtInstructorOperationsAddPhone.Text,
                DepartmentNo = Convert.ToInt32(cmbInstructorOperationsAddAvailableDepartmentNames.SelectedValue),
                Password = password,
                InstructorNo = 1    //Insignificant
            };
            var messageDialogResult = MessageBox.Show($"{addedInstructor.FirstName} {addedInstructor.LastName} isimli yeni bir öğretim görevlisi eklenecek. Onaylıyor musunuz?\n\nGiriş şifresi: {password}", Messages.AddConfirmation, MessageBoxButtons.YesNo);
            if (messageDialogResult == DialogResult.Yes)
            {
                AddOperationForAddButtons<Instructor>(_instructorService, addedInstructor, ReBuildInstructorPanel);
            }
        }

        private void cmbInstructorOperationsAssignInstructorToDepartmentInstructors_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = $@"{((Instructor)e.ListItem).FirstName} {((Instructor)e.ListItem).LastName}";
        }

        private void btnInstructorOperationsCompleteAssignment_Click(object sender, EventArgs e)
        {
            if (cmbInstructorOperationsAssignInstructorToDepartmentInstructors.SelectedItem == null ||
                cmbInstructorOperationsAssignInstructorToDepartmentDepartmentsNames.SelectedItem == null)
            {
                MessageBox.Show(Messages.MakeSureSelectAllFields, Messages.Warning);
                return;
            }

            var instructorNo = Convert.ToInt32(cmbInstructorOperationsAssignInstructorToDepartmentInstructors.SelectedValue);
            var newDepartmentNo = Convert.ToInt32(cmbInstructorOperationsAssignInstructorToDepartmentDepartmentsNames.SelectedValue);
            var instructorResult = _instructorService.GetByInstructorNo(instructorNo);
            var departmentResult = _departmentService.GetByDepartmentNo(newDepartmentNo);

            if (instructorResult.Success && departmentResult.Success)
            {
                var updatedInstructor = instructorResult.Data;
                if (updatedInstructor.DepartmentNo == newDepartmentNo)
                {
                    MessageBox.Show(Messages.TheInstructorIsAlreadyWorkingInTheDepartmentYouWantToAssign, Messages.Warning);
                    return;
                }
                updatedInstructor.DepartmentNo = newDepartmentNo;
                var messageDialogResult = MessageBox.Show($"{updatedInstructor.FirstName} {updatedInstructor.LastName} isimli öğretim görevlisi {departmentResult.Data.DepartmentName} isimli bölüme atanacak. Onaylıyor musunuz?", Messages.AddConfirmation, MessageBoxButtons.YesNo);
                if (messageDialogResult == DialogResult.Yes)
                {
                    UpdateOperationForUpdateButtons<Instructor>(_instructorService, updatedInstructor, ReBuildInstructorPanel);
                }
            }
            else
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingInstructorDetails}\n\n{ErrorMessageBuilder.CreateErrorMessageFromStringList(new List<string> { instructorResult.Message, departmentResult.Message })}", Messages.ServerError);
            }
        }

        private void btnInstructorOperationsSearch_Click(object sender, EventArgs e)
        {
            var textBoxesToClearAfterProcess = new List<TextBox>
            {
                txtInstructorOperationsInfoInstructorNo,
                txtInstructorOperationsInfoDepartmentName,
                txtInstructorOperationsInfoEmail,
                txtInstructorOperationsInfoPhone,
                txtInstructorOperationsInfoFirstName,
                txtInstructorOperationsInfoLastName
            };

            if (txtInstructorOperationsSearchByInstructorNo.Text != string.Empty && txtInstructorOperationsSearchByInstructorName.Text != string.Empty)
            {
                MessageBox.Show(Messages.NotAllSearchCriteriaCanBeFilledAtOnce, Messages.Warning);
            }
            else if (txtInstructorOperationsSearchByInstructorNo.Text != string.Empty &&
                     txtInstructorOperationsSearchByInstructorName.Text == string.Empty)
            {
                if (!NumberStringValidator.ValidateString(txtInstructorOperationsSearchByInstructorNo.Text))
                {
                    MessageBox.Show(Messages.InstructorNoMustConsistOfNumbersOnly, Messages.Warning);
                    return;
                }
                var instructorResult = _instructorService.GetAllByInstructorNo(Convert.ToInt32(txtInstructorOperationsSearchByInstructorNo.Text));
                ApplySearchToListbox(listBoxInstructorOperationsInstructorList, instructorResult, textBoxesToClearAfterProcess);
            }
            else if (txtInstructorOperationsSearchByInstructorNo.Text == string.Empty &&
                     txtInstructorOperationsSearchByInstructorName.Text != string.Empty)
            {
                var instructorResult = _instructorService.GetAllContainInstructorName(txtInstructorOperationsSearchByInstructorName.Text);
                ApplySearchToListbox(listBoxInstructorOperationsInstructorList, instructorResult, textBoxesToClearAfterProcess);
            }
            else
            {
                MessageBox.Show(Messages.NotAllSearchCriteriaCanBeFilledAtOnce, Messages.Warning);
            }
        }

        private void btnInstructorOperationsSearchReset_Click(object sender, EventArgs e)
        {
            ReBuildInstructorPanel();
        }

        private void btnInstructorOperationsFilter_Click(object sender, EventArgs e)
        {
            if (cmbInstructorOperationsFilterByDepartment.SelectedItem == null)
            {
                MessageBox.Show(Messages.MustSelectADepartmentToBeAbleToFilter, Messages.Warning);
                return;
            }

            FilteringTool.FilterListBox<Instructor>(listBoxInstructorOperationsInstructorList, _instructorService, new List<Func<Instructor, bool>>
            {
                i => i.DepartmentNo == Convert.ToInt32(cmbInstructorOperationsFilterByDepartment.SelectedValue)
            }, Messages.SomethingWentWrongWhileGettingCurrentCourses);
        }

        private void btnInstructorOperationsFilterReset_Click(object sender, EventArgs e)
        {
            ReBuildInstructorPanel();
        }

        // Student Operations Methods

        private void cmbStudentOperationsAddAdviser_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = $@"{((Instructor)e.ListItem).FirstName} {((Instructor)e.ListItem).LastName}";
        }

        private void cmbStudentOperationsFilterAdviserList_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = $@"{((Instructor)e.ListItem).FirstName} {((Instructor)e.ListItem).LastName}";
        }

        private void listBoxStudentOperationsStudentList_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = $@"{((Student)e.ListItem).FirstName} {((Student)e.ListItem).LastName}";
        }

        private void chbxStudentOperationsInfoStudentDepartmentEnabled_CheckedChanged(object sender, EventArgs e)
        {
            SwitchUpdateComboBoxEnabled(chbxStudentOperationsInfoStudentDepartmentEnabled, cmbStudentOperationsInfoDepartment);
        }

        private void listBoxStudentOperationsStudentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxStudentOperationsStudentList.SelectedItem == null)
            {
                return;
            }

            int selectedStudentNo = UniqueValueTaker.GetUniqueValueOfSelectedItemInListbox<Student>(listBoxStudentOperationsStudentList);

            var selectedStudentResult = _studentService.GetByStudentNo(selectedStudentNo);
            var departmentResult = _departmentService.GetByDepartmentNo(selectedStudentResult.Data.DepartmentNo);
            var instructorResult = _instructorService.GetByInstructorNo(selectedStudentResult.Data.AdviserNo);

            if (selectedStudentResult.Success && departmentResult.Success && instructorResult.Success)
            {
                txtStudentOperationsInfoStudentNo.Text = selectedStudentResult.Data.StudentNo.ToString();
                SetComboBoxSelectedItem<Department>(cmbStudentOperationsInfoDepartment, departmentResult.Data.DepartmentNo.ToString());
                txtStudentOperationsInfoAdviserName.Text = $@"{instructorResult.Data.FirstName} {instructorResult.Data.LastName}";
                txtStudentOperationsInfoEmail.Text = selectedStudentResult.Data.Email;
                txtStudentOperationsInfoPhone.Text = selectedStudentResult.Data.Phone;
                txtStudentOperationsInfoFirstName.Text = selectedStudentResult.Data.FirstName;
                txtStudentOperationsInfoLastName.Text = selectedStudentResult.Data.LastName;
                txtStudentOperationsInfoSemester.Text = selectedStudentResult.Data.Semester.ToString();
                txtStudentOperationsInfoEnrollmentDate.Text = selectedStudentResult.Data.EnrollmentDate.ToString();
            }
            else
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingDepartmentDetails}:\n\n{ErrorMessageBuilder.CreateErrorMessageFromStringList(new List<string> { selectedStudentResult.Message, departmentResult.Message, instructorResult.Message })}", Messages.ServerError);
            }
        }

        private void btnStudentOperationsDelete_Click(object sender, EventArgs e)
        {
            if (listBoxStudentOperationsStudentList.SelectedItem == null)
            {
                return;
            }
            int studentNo = UniqueValueTaker.GetUniqueValueOfSelectedItemInListbox<Student>(listBoxStudentOperationsStudentList);
            var deletedStudentResult = _studentService.GetByStudentNo(studentNo);
            if (deletedStudentResult.Success)
            {
                var messageDialogResult = MessageBox.Show($@"{deletedStudentResult.Data.FirstName} {deletedStudentResult.Data.LastName} isimli öğrenci silinecek. Onaylıyor musunuz?", Messages.DeleteConfirmation, MessageBoxButtons.YesNo);
                if (messageDialogResult == DialogResult.Yes)
                {
                    DeleteOperationForDeleteButtons<Student>(_studentService, deletedStudentResult.Data, ReBuildStudentPanel);
                }
            }
            else
            {
                MessageBox.Show(Messages.SomethingWentWrongWhileGettingStudentDetails, Messages.ServerError);
            }
        }

        private void btnStudenOperationsChangeStudentDepartment_Click(object sender, EventArgs e)
        {
            if (listBoxStudentOperationsStudentList.SelectedItem == null)
            {
                return;
            }

            int selectedStudentNo = UniqueValueTaker.GetUniqueValueOfSelectedItemInListbox<Student>(listBoxStudentOperationsStudentList);
            var updatedStudentResult = _studentService.GetByStudentNo(selectedStudentNo);

            if (!updatedStudentResult.Success)
            {
                MessageBox.Show($"{Messages.SomethingWentWrongWhileGettingStudentDetails}:\n\n{updatedStudentResult.Message}", Messages.ServerError);
                return;
            }

            var updatedStudent = updatedStudentResult.Data;
            int updatedDepartmentNo = Convert.ToInt32(cmbStudentOperationsInfoDepartment.SelectedValue);

            if (updatedStudent.DepartmentNo == updatedDepartmentNo)
            {
                MessageBox.Show(Messages.TheStudentIsAlreadyStudyingInTheDepartmentYouWantToChange, Messages.Warning);
                return;
            }

            var oldDepartmentName = _departmentService.GetByDepartmentNo(updatedStudent.DepartmentNo).Data.DepartmentName;

            updatedStudent.DepartmentNo = updatedDepartmentNo;

            var newDepartmentName = _departmentService.GetByDepartmentNo(updatedStudent.DepartmentNo).Data.DepartmentName;

            var message = $"{updatedStudent.FirstName} {updatedStudent.LastName} isimli öğrencinin öğrenim göreceği bölüm, {oldDepartmentName} bölümüden {newDepartmentName} bölümüne güncellenecek. Onaylıyor musunuz?\n";


            var messageDialogResult = MessageBox.Show(message, Messages.UpdateConfirmation, MessageBoxButtons.YesNo);
            if (messageDialogResult == DialogResult.Yes)
            {
                UpdateOperationForUpdateButtons<Student>(_studentService, updatedStudent, ReBuildStudentPanel);
            }
        }

        private void btnStudentOperationsAdd_Click(object sender, EventArgs e)
        {
            if (cmbStudentOperationsAddDepartment.Items.Count < 1)
            {
                MessageBox.Show(Messages.ThereMustBeAtLeastOneDepartmentToBeAbleToAddAStudent, Messages.Warning);
                return;
            }

            if (cmbStudentOperationsAddAdviser.Items.Count < 1)
            {
                MessageBox.Show(Messages.ThereMustBeAtLeastOneInstructorToBeAbleToAddAStudent, Messages.Warning);
                return;
            }
            if (txtStudentOperationsAddFirstName.Text == string.Empty
                || txtStudentOperationsAddLastName.Text == string.Empty
                || txtStudentOperationsAddEmail.Text == string.Empty
                || txtStudentOperationsAddPhone.Text == string.Empty)
            {
                MessageBox.Show(Messages.MakeSureFillInAllFields, Messages.Warning);
                return;
            }

            if (!NumberStringValidator.ValidateString(txtStudentOperationsAddPhone.Text))
            {
                MessageBox.Show(Messages.OnlyNumberForPhone, Messages.Warning);
                return;
            }

            var password = $"{TurkishCharNormalizer.Normalization(txtStudentOperationsAddFirstName.Text.ToLowerInvariant())}.{TurkishCharNormalizer.Normalization(txtStudentOperationsAddLastName.Text.ToLowerInvariant())}";

            var addedStudent = new Student
            {
                StudentNo = 1,
                Email = txtStudentOperationsAddEmail.Text,
                FirstName = txtStudentOperationsAddFirstName.Text,
                LastName = txtStudentOperationsAddLastName.Text,
                Phone = txtStudentOperationsAddPhone.Text,
                AdviserNo = Convert.ToInt32(cmbStudentOperationsAddAdviser.SelectedValue),
                DepartmentNo = Convert.ToInt32(cmbStudentOperationsAddDepartment.SelectedValue),
                EnrollmentDate = short.Parse(DateTime.Now.Year.ToString()),
                Password = password,
                Semester = 1
            };
            var messageDialogResult = MessageBox.Show($"{addedStudent.FirstName} {addedStudent.LastName} isimli yeni bir öğrenci eklenecek. Onaylıyor musunuz?\n\nGiriş şifresi: {addedStudent.Password}", Messages.AddConfirmation, MessageBoxButtons.YesNo);
            if (messageDialogResult == DialogResult.Yes)
            {
                AddOperationForAddButtons<Student>(_studentService, addedStudent, ReBuildStudentPanel);
            }
        }

        private void btnStudentOperationsSearch_Click(object sender, EventArgs e)
        {
            var textBoxesToClearAfterProcess = new List<TextBox>
            {
                txtStudentOperationsInfoStudentNo,
                txtStudentOperationsInfoFirstName,
                txtStudentOperationsInfoLastName,
                txtStudentOperationsInfoEmail,
                txtStudentOperationsInfoPhone,
                txtStudentOperationsInfoSemester,
                txtStudentOperationsInfoEnrollmentDate,
                txtStudentOperationsInfoAdviserName
            };

            if (txtStudentOperationsSearchByStudentNo.Text != string.Empty && txtStudentOperationsSearchByStudentName.Text != string.Empty)
            {
                MessageBox.Show(Messages.NotAllSearchCriteriaCanBeFilledAtOnce, Messages.Warning);
            }
            else if (txtStudentOperationsSearchByStudentNo.Text != string.Empty &&
                     txtStudentOperationsSearchByStudentName.Text == string.Empty)
            {
                if (!NumberStringValidator.ValidateString(txtStudentOperationsSearchByStudentNo.Text))
                {
                    MessageBox.Show(Messages.StudentNoMustConsistOfNumbersOnly, Messages.Warning);
                    return;
                }
                var studentResult = _studentService.GetAllByStudentNo(Convert.ToInt32(txtStudentOperationsSearchByStudentNo.Text));
                ApplySearchToListbox(listBoxStudentOperationsStudentList, studentResult, textBoxesToClearAfterProcess);
            }
            else if (txtStudentOperationsSearchByStudentNo.Text == string.Empty &&
                     txtStudentOperationsSearchByStudentName.Text != string.Empty)
            {
                var studentResult = _studentService.GetAllContainStudentName(txtStudentOperationsSearchByStudentName.Text);
                ApplySearchToListbox(listBoxStudentOperationsStudentList, studentResult, textBoxesToClearAfterProcess);
            }
            else
            {
                MessageBox.Show(Messages.NotAllSearchCriteriaCanBeFilledAtOnce, Messages.Warning);
            }
        }

        private void btnStudentOperationsSearchReset_Click(object sender, EventArgs e)
        {
            ReBuildStudentPanel();
        }

        private void radBtnStudentOperationsFilterByDepartment_CheckedChanged(object sender, EventArgs e)
        {
            cmbStudentOperationsFilterDepartmentList.Enabled = radBtnStudentOperationsFilterByDepartment.Checked;
        }

        private void radBtnStudentOperationsFilterByAdviser_CheckedChanged(object sender, EventArgs e)
        {
            cmbStudentOperationsFilterAdviserList.Enabled = radBtnStudentOperationsFilterByAdviser.Checked;
        }

        private void btnStudentOperationsFilter_Click(object sender, EventArgs e)
        {
            if (!radBtnStudentOperationsFilterByDepartment.Checked && !radBtnStudentOperationsFilterByAdviser.Checked)
            {
                MessageBox.Show(Messages.AtLeastOneFilterMustBeOn, Messages.Warning);
                return;
            }

            if (radBtnStudentOperationsFilterByDepartment.Checked &&
                cmbStudentOperationsFilterDepartmentList.SelectedItem == null)
            {
                MessageBox.Show(Messages.MustSelectADepartmentToBeAbleToFilter, Messages.Warning);
                return;
            }

            if (radBtnStudentOperationsFilterByAdviser.Checked &&
                cmbStudentOperationsFilterAdviserList.SelectedItem == null)
            {
                MessageBox.Show(Messages.MustSelectAInstructorToBeAbleToFilter, Messages.Warning);
                return;
            }

            if (radBtnStudentOperationsFilterByDepartment.Checked)
            {
                FilteringTool.FilterListBox<Student>(listBoxStudentOperationsStudentList, _studentService, new List<Func<Student, bool>>
                    {
                        s => s.DepartmentNo == Convert.ToInt32(cmbStudentOperationsFilterDepartmentList.SelectedValue)
                    }, Messages.SomethingWentWrongWhileGettingCurrentStudents
                    );
            }
            else if (radBtnStudentOperationsFilterByAdviser.Checked)
            {
                FilteringTool.FilterListBox<Student>(listBoxStudentOperationsStudentList, _studentService, new List<Func<Student, bool>>
                    {
                        s => s.AdviserNo == Convert.ToInt32(cmbStudentOperationsFilterAdviserList.SelectedValue)
                    }, Messages.SomethingWentWrongWhileGettingCurrentStudents
                );
            }
        }

        private void btnStudentOperationsFilterReset_Click(object sender, EventArgs e)
        {
            ReBuildStudentPanel();
        }
    }
}
