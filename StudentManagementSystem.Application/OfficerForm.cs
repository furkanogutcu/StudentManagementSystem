using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using StudentManagementSystem.Application.Utilities;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Business.Constants;
using StudentManagementSystem.Core.Entities;
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
                SetDataToListBox<Department>(listBoxDepartmentOperationsCurrentDepartments, departmentResult.Data);
                grbxDepartmentOperationsCurrentDepartments.Text = $@"Mevcut bölümler (Toplam kayıtlı bölüm sayısı: {departmentResult.Data.Count})";
            }
            else
            {
                MessageBox.Show($@"{Messages.SomethingWentWrongWhileGettingCurrentDepartments}:\n\n{departmentResult.Message}", Messages.ServerError);
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
            if (courseResult.Success && departmentListForUpdateResult.Success && instructorListForUpdateResult.Success)
            {
                SetDataToListBox<CatalogCourse>(listBoxCourseOperationsListCourses, courseResult.Data);
                grbxCourseOperationsCurrentCourses.Text = $@"Dersleri listele (Toplam kayıtlı ders sayısı: {courseResult.Data.Count})";
                SetDataToComboBox(cmbCourseOperationsCourseInfoDepartment, departmentListForUpdateResult.Data);
                SetDataToComboBox(cmbCourseOperationsCourseInfoInstructor, instructorListForUpdateResult.Data);
                SetDataToComboBox(cmbCourseOperationsAddCourseDepartment, departmentListForUpdateResult.Data);
                SetDataToComboBox(cmbCourseOperationsAddCourseInstructor, instructorListForUpdateResult.Data);
                SetDataToComboBox(cmbCourseOperationsFilterCourseDepartment, departmentListForUpdateResult.Data);
            }
            else
            {
                MessageBox.Show($@"{Messages.SomethingWentWrongWhileGettingCurrentCourses}:\n\n{ErrorMessageBuilder.CreateErrorMessageFromStringList(
                    new List<string> { courseResult.Message, departmentListForUpdateResult.Message, instructorListForUpdateResult.Message })}", Messages.ServerError);
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
            if (instructorResult.Success)
            {
                SetDataToListBox<Instructor>(listBoxInstructorOperationsInstructorList, instructorResult.Data);
                grbxInstructorOperationsCurrentInstructors.Text = $@"Öğretim Görevlisi Listesi (Toplam kayıtlı öğretim görevlisi sayısı: {instructorResult.Data.Count})";
            }
            else
            {
                MessageBox.Show($@"{Messages.SomethingWentWrongWhileGettingCurrentInstructors}:\n\n{instructorResult.Message}", Messages.ServerError);
            }
        }

        //Student
        private void btnGlobalStudentOperations_Click(object sender, System.EventArgs e)
        {
            PanelSwitcher.ShowPanel(pnlGlobalStudentOperations, _panels);
        }

        //Assign advisor
        private void btnGlobalAssignAdvisor_Click(object sender, System.EventArgs e)
        {
            PanelSwitcher.ShowPanel(pnlGlobalAssignAdviser, _panels);
        }

        private void SetDataToListBox<T>(ListBox listBox, List<T> data)
            where T : class, IEntity, new()
        {
            listBox.DataSource = data;
            switch (typeof(T).Name)
            {
                case nameof(Department):
                    listBox.DisplayMember = nameof(Department.DepartmentName);
                    listBox.ValueMember = nameof(Department.DepartmentNo);
                    break;
                case nameof(Student):
                    listBox.DisplayMember = nameof(Student.FirstName);
                    listBox.ValueMember = nameof(Student.StudentNo);
                    break;
                case nameof(Officer):
                    listBox.DisplayMember = nameof(Officer.FirstName);
                    listBox.ValueMember = nameof(Officer.OfficerNo);
                    break;
                case nameof(Instructor):
                    listBox.DisplayMember = nameof(Instructor.FirstName);
                    listBox.ValueMember = nameof(Instructor.InstructorNo);
                    break;
                case nameof(CatalogCourse):
                    listBox.DisplayMember = nameof(CatalogCourse.CourseName);
                    listBox.ValueMember = nameof(CatalogCourse.CourseNo);
                    break;
                default:
                    throw new Exception("Tip eşleme hatası");
            }
        }

        private void SetDataToComboBox<T>(ComboBox comboBox, List<T> dataList)
            where T : class, IEntity, new()
        {
            var finalData = new List<T>();
            foreach (var data in dataList)
            {
                finalData.Add(data);
            }

            comboBox.DataSource = finalData;
            switch (typeof(T).Name)
            {
                case nameof(Department):
                    comboBox.DisplayMember = nameof(Department.DepartmentName);
                    comboBox.ValueMember = nameof(Department.DepartmentNo);
                    break;
                case nameof(Student):
                    comboBox.DisplayMember = nameof(Student.FirstName);
                    comboBox.ValueMember = nameof(Student.StudentNo);
                    break;
                case nameof(Officer):
                    comboBox.DisplayMember = nameof(Officer.FirstName);
                    comboBox.ValueMember = nameof(Officer.OfficerNo);
                    break;
                case nameof(Instructor):
                    comboBox.DisplayMember = nameof(Instructor.FirstName);
                    comboBox.ValueMember = nameof(Instructor.InstructorNo);
                    break;
                case nameof(CatalogCourse):
                    comboBox.DisplayMember = nameof(CatalogCourse.CourseName);
                    comboBox.ValueMember = nameof(CatalogCourse.CourseNo);
                    break;
                default:
                    throw new Exception("Tip eşleme hatası");
            }
        }

        private int GetUniqueValueOfSelectedItemInListbox<T>(ListBox listBox)
        {
            int uniqueValueOfSelectedIndex;
            // The first item selected in the listbox is of T type. The next selected items are of type int.
            var tempObject = listBox.SelectedItem;

            switch (typeof(T).Name)
            {
                case nameof(Department):
                    uniqueValueOfSelectedIndex = ((Department)tempObject).DepartmentNo;
                    break;
                case nameof(Student):
                    uniqueValueOfSelectedIndex = ((Student)tempObject).StudentNo;
                    break;
                case nameof(Officer):
                    uniqueValueOfSelectedIndex = ((Officer)tempObject).OfficerNo;
                    break;
                case nameof(Instructor):
                    uniqueValueOfSelectedIndex = ((Instructor)tempObject).InstructorNo;
                    break;
                case nameof(EnrolledCourse):
                    uniqueValueOfSelectedIndex = ((EnrolledCourse)tempObject).Id;
                    break;
                case nameof(CatalogCourse):
                    uniqueValueOfSelectedIndex = ((CatalogCourse)tempObject).CourseNo;
                    break;
                case nameof(AdviserApproval):
                    uniqueValueOfSelectedIndex = ((AdviserApproval)tempObject).Id;
                    break;
                default:
                    uniqueValueOfSelectedIndex = Convert.ToInt32(listBox.SelectedItem);
                    break;
            }

            return uniqueValueOfSelectedIndex;
        }

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
                MessageBox.Show($@"{Messages.SomethingWentWrongWhileAddition}:\n\n{addResult.Message}", Messages.ServerError);
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
                MessageBox.Show($@"{Messages.SomethingWentWrongWhileDeletion}:\n\n{deleteResult.Message}", Messages.ServerError);
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
                MessageBox.Show($@"{Messages.SomethingWentWrongWhileUpdate}:\n\n{updateResult.Message}", Messages.ServerError);
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

        private Dictionary<string, bool> FindEntityDifferent<T>(T firstEntity, T secondEntity)
            where T : class, IEntity, new()
        {
            var entitiesProperties = firstEntity.GetType().GetProperties();

            Dictionary<string, bool> returnDictionary = new Dictionary<string, bool>();

            foreach (var propertyInfo in entitiesProperties)
            {
                if (propertyInfo.PropertyType == typeof(DateTime?))
                {
                    if (propertyInfo.GetValue(firstEntity) == null && propertyInfo.GetValue(secondEntity) == null)
                    {
                        returnDictionary.Add(propertyInfo.Name, false);
                    }
                    else
                    {
                        returnDictionary.Add(propertyInfo.Name, ((DateTime)propertyInfo.GetValue(firstEntity)).Date != ((DateTime)propertyInfo.GetValue(secondEntity)).Date);
                    }
                }
                else
                {
                    returnDictionary.Add(propertyInfo.Name, !propertyInfo.GetValue(firstEntity).Equals(propertyInfo.GetValue(secondEntity)));
                }
            }

            return returnDictionary;
        }

        private bool NumberString(string input)
        {
            foreach (var chr in input)
            {
                if (!Char.IsNumber(chr))
                {
                    return false;
                }
            }

            return true;
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
            int selectedDepartmentNo = GetUniqueValueOfSelectedItemInListbox<Department>(listBoxDepartmentOperationsCurrentDepartments);

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
            int departmentNo = GetUniqueValueOfSelectedItemInListbox<Department>(listBoxDepartmentOperationsCurrentDepartments);
            var deletedDepartment = _departmentService.GetByDepartmentNo(departmentNo);
            var messageDialogResult = MessageBox.Show($@"{deletedDepartment.Data.DepartmentName} isimli bölüm silinecek. Onaylıyor musunuz?", Messages.DeleteConfirmation, MessageBoxButtons.YesNo);
            if (messageDialogResult == DialogResult.Yes)
            {
                DeleteOperationForDeleteButtons<Department>(_departmentService, deletedDepartment.Data, ReBuildDepartmentPanel);
            }
        }
        private void btnDepartmentOperationsUpdateDepartmentName_Click(object sender, EventArgs e)
        {
            if (txtDepartmentOperationsUpdateDepartmentName.Text == string.Empty && txtDepartmentOperationsUpdateNumberOfSemester.Text == string.Empty)
            {
                MessageBox.Show(Messages.MustFillInTheFieldsWantToUpdate, Messages.Warning);
                return;
            }

            if (!NumberString(txtDepartmentOperationsUpdateNumberOfSemester.Text))
            {
                MessageBox.Show(Messages.DepartmentNumberOfSemesterMustConsistOfNumbersOnly, Messages.Warning);
                return;
            }

            int selectedDepartmentNo = GetUniqueValueOfSelectedItemInListbox<Department>(listBoxDepartmentOperationsCurrentDepartments);
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

            if (FindEntityDifferent(updateDepartment, updatedDepartmentDetailsResult.Data).Values.Contains(true))
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

            if (!NumberString(txtDepartmentOperationsAddNumberOfSemester.Text))
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
            if (txtDepartmentOperationsSearchByDepartmentNo.Text != string.Empty &&
                txtDepartmentOperationsSearchByDepartmentName.Text != string.Empty)
            {
                MessageBox.Show(Messages.NotAllSearchCriteriaCanBeFilledAtOnce, Messages.Warning);
            }
            else if (txtDepartmentOperationsSearchByDepartmentNo.Text != string.Empty &&
                     txtDepartmentOperationsSearchByDepartmentName.Text == string.Empty)
            {
                if (!NumberString(txtDepartmentOperationsSearchByDepartmentNo.Text))
                {
                    MessageBox.Show(Messages.DepartmentNumberMustConsistOfNumbersOnly, Messages.Warning);
                    return;
                }
                var departmentResult = _departmentService.GetAllByDepartmentNo(Convert.ToInt32(txtDepartmentOperationsSearchByDepartmentNo.Text));
                if (departmentResult.Success)
                {
                    txtDepartmentOperationsDepartmentName.Text = string.Empty;
                    txtDepartmentOperationsDepartmentNo.Text = string.Empty;
                    txtDepartmentOperationsTotalInstructor.Text = string.Empty;
                    txtDepartmentOperationsTotalStudents.Text = string.Empty;
                    SetDataToListBox<Department>(listBoxDepartmentOperationsCurrentDepartments, departmentResult.Data);
                    MessageBox.Show(Messages.CreateSearchResultMessage(departmentResult.Data.Count), Messages.Information);
                }
                else
                {
                    MessageBox.Show($@"{Messages.SomethingWentWrongWhileSearching}:\n\n{departmentResult.Message}", Messages.ServerError);
                }

            }
            else if (txtDepartmentOperationsSearchByDepartmentNo.Text == string.Empty &&
                     txtDepartmentOperationsSearchByDepartmentName.Text != string.Empty)
            {
                var departmentResult = _departmentService.GetAllContainDepartmentName(txtDepartmentOperationsSearchByDepartmentName.Text);
                if (departmentResult.Success)
                {
                    txtDepartmentOperationsDepartmentName.Text = string.Empty;
                    txtDepartmentOperationsDepartmentNo.Text = string.Empty;
                    txtDepartmentOperationsTotalInstructor.Text = string.Empty;
                    txtDepartmentOperationsTotalStudents.Text = string.Empty;
                    SetDataToListBox<Department>(listBoxDepartmentOperationsCurrentDepartments, departmentResult.Data);
                    MessageBox.Show(Messages.CreateSearchResultMessage(departmentResult.Data.Count), Messages.Information);
                }
                else
                {
                    MessageBox.Show($@"{Messages.SomethingWentWrongWhileSearching}:\n\n{departmentResult.Message}", Messages.ServerError);
                }
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

            int selectedCourseNo = GetUniqueValueOfSelectedItemInListbox<CatalogCourse>(listBoxCourseOperationsListCourses);

            var selectedCourseResult = _catalogCourseService.GetByCourseNo(selectedCourseNo);

            if (selectedCourseResult.Success)
            {
                var departmentResult = _departmentService.GetByDepartmentNo(selectedCourseResult.Data.DepartmentNo);
                var instructorResult = _instructorService.GetByInstructorNo(selectedCourseResult.Data.InstructorNo);

                if (departmentResult.Success && instructorResult.Success)
                {
                    txtCourseOperationsCourseInfoCourseNo.Text = selectedCourseResult.Data.CourseNo.ToString();
                    txtCourseOperationsCourseInfoCourseName.Text = selectedCourseResult.Data.CourseName;


                    cmbCourseOperationsCourseInfoDepartment.SelectedItem = departmentResult.Data;
                    cmbCourseOperationsCourseInfoInstructor.SelectedItem = instructorResult.Data;

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

        // Instructor Operations Methods

        private void listBoxInstructorOperationsInstructorList_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = $@"{((Instructor)e.ListItem).FirstName} {((Instructor)e.ListItem).LastName}";
        }

        private void btnCourseOperationsUpdate_Click(object sender, EventArgs e)
        {
            if (!NumberString(txtCourseOperationsCourseInfoCredit.Text))
            {
                MessageBox.Show($@"{Messages.CourseCreditMustConsistOfNumbersOnly}", Messages.Warning);
                return;
            }

            if (!NumberString(txtCourseOperationsCourseInfoCourseYear.Text))
            {
                MessageBox.Show($@"{Messages.CourseYearMustConsistOfNumbersOnly}", Messages.Warning);
                return;
            }

            if (!NumberString(txtCourseOperationsCourseInfoSemester.Text))
            {
                MessageBox.Show($@"{Messages.CourseSemesterMustConsistOfNumbersOnly}", Messages.Warning);
                return;
            }

            int selectedCourseNo = GetUniqueValueOfSelectedItemInListbox<CatalogCourse>(listBoxCourseOperationsListCourses);
            var updatedCourseDetailsResult = _catalogCourseService.GetByCourseNo(selectedCourseNo);

            if (!updatedCourseDetailsResult.Success)
            {
                MessageBox.Show($@"{Messages.SomethingWentWrongWhileGettingCourseDetails}:\n\n{updatedCourseDetailsResult.Message}", Messages.ServerError);
                return;
            }

            updatedCourseDetailsResult.Data.CourseName =
                updatedCourseDetailsResult.Data.CourseName == txtCourseOperationsCourseInfoCourseName.Text
                    ? updatedCourseDetailsResult.Data.CourseName
                    : txtCourseOperationsCourseInfoCourseName.Text;

            updatedCourseDetailsResult.Data.Credit =
                updatedCourseDetailsResult.Data.Credit.ToString() == txtCourseOperationsCourseInfoCredit.Text
                    ? updatedCourseDetailsResult.Data.Credit
                    : Convert.ToInt32(txtCourseOperationsCourseInfoCredit.Text);

            updatedCourseDetailsResult.Data.CourseYear =
                updatedCourseDetailsResult.Data.CourseYear.ToString() == txtCourseOperationsCourseInfoCourseYear.Text
                    ? updatedCourseDetailsResult.Data.CourseYear
                    : short.Parse(txtCourseOperationsCourseInfoCourseYear.Text);

            updatedCourseDetailsResult.Data.CourseSemester =
                updatedCourseDetailsResult.Data.CourseSemester.ToString() == txtCourseOperationsCourseInfoSemester.Text
                    ? updatedCourseDetailsResult.Data.CourseSemester
                    : Convert.ToInt32(txtCourseOperationsCourseInfoSemester.Text);

            var selectedDepartmentNo = ((Department)cmbCourseOperationsCourseInfoDepartment.SelectedItem).DepartmentNo;

            updatedCourseDetailsResult.Data.DepartmentNo =
                updatedCourseDetailsResult.Data.DepartmentNo == selectedDepartmentNo
                    ? updatedCourseDetailsResult.Data.DepartmentNo
                    : selectedDepartmentNo;

            var selectedInstructorNo = ((Instructor)cmbCourseOperationsCourseInfoInstructor.SelectedItem).InstructorNo;

            updatedCourseDetailsResult.Data.InstructorNo =
                updatedCourseDetailsResult.Data.InstructorNo == selectedInstructorNo
                ? updatedCourseDetailsResult.Data.InstructorNo
                : selectedInstructorNo;

            var updatedDepartmentName = _departmentService.GetByDepartmentNo(updatedCourseDetailsResult.Data.DepartmentNo).Data.DepartmentName;
            var updatedInstructor = _instructorService.GetByInstructorNo(updatedCourseDetailsResult.Data.InstructorNo).Data;
            var updatedInstructorFullName = $"{updatedInstructor.FirstName} {updatedInstructor.LastName}";

            var message = $"{updatedCourseDetailsResult.Data.CourseName} isimli ders aşağıdaki gibi güncellenecek. Onaylıyor musunuz?\n";
            message += $"\n- Ders adı: {updatedCourseDetailsResult.Data.CourseName}";
            message += $"\n- Bölüm: {updatedDepartmentName}";
            message += $"\n- Öğretim görevlisi {updatedInstructorFullName}";
            message += $"\n- Kredi: {updatedCourseDetailsResult.Data.Credit}";
            message += $"\n- Ders yılı: {updatedCourseDetailsResult.Data.CourseYear}";
            message += $"\n- Ders dönemi: {updatedCourseDetailsResult.Data.CourseSemester}";

            var messageDialogResult = MessageBox.Show(message, Messages.UpdateConfirmation, MessageBoxButtons.YesNo);
            if (messageDialogResult == DialogResult.Yes)
            {
                UpdateOperationForUpdateButtons<CatalogCourse>(_catalogCourseService, updatedCourseDetailsResult.Data, ReBuildCoursePanel);
            }
        }
    }
}
