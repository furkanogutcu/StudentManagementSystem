using System;
using System.Collections.Generic;
using System.Windows.Forms;
using StudentManagementSystem.Application.Utilities;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Business.Constants;
using StudentManagementSystem.Core.Entities;
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

        public OfficerForm(Officer officer, IOfficerService officerService, IDepartmentService departmentService, IInstructorService instructorService, IStudentService studentService)
        {
            _officer = officer;
            _officerService = officerService;
            _departmentService = departmentService;
            _instructorService = instructorService;
            _studentService = studentService;
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
            PanelCleaner.Clear(pnlGlobalProfile);
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
            PanelCleaner.Clear(pnlGlobalDepartmentOperations);
            var departmentResult = _departmentService.GetAll();
            if (departmentResult.Success)
            {
                SetDataToListBox<Department>(listBoxDepartmentOperationsCurrentDepartments, departmentResult.Data);
                grbxDepartmentOperationsCurrentDepartments.Text = $@"Mevcut bölümler (Toplam kayıtlı bölüm sayısı: {departmentResult.Data.Count})";
            }
            else
            {
                MessageBox.Show($@"{Messages.SomethingWentWrongWhileGettingCurrentDepartments}\n\n{departmentResult.Message}", Messages.ServerError);
                return;
            }
        }

        //Course
        private void btnGlobalCourseOperations_Click(object sender, System.EventArgs e)
        {
            PanelSwitcher.ShowPanel(pnlGlobalCourseOperations, _panels);
        }

        //Instructor
        private void btnGlobalInstructorOperations_Click(object sender, System.EventArgs e)
        {
            PanelSwitcher.ShowPanel(pnlGlobalInstructorOperations, _panels);
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
                    listBox.DisplayMember = $"{nameof(Student.FirstName)} + {nameof(Student.LastName)}";    //CHECK
                    listBox.ValueMember = nameof(Student.StudentNo);
                    break;
                case nameof(Officer):
                    listBox.DisplayMember = $"{nameof(Officer.FirstName)} + {nameof(Officer.LastName)}";    //CHECK
                    listBox.ValueMember = nameof(Officer.OfficerNo);
                    break;
                case nameof(Instructor):
                    listBox.DisplayMember = $"{nameof(Instructor.FirstName)} + {nameof(Instructor.LastName)}";
                    listBox.ValueMember = nameof(Instructor.InstructorNo);
                    break;
                case nameof(EnrolledCourse):
                    listBox.DisplayMember = "TEST";     //CHECK
                    listBox.ValueMember = nameof(EnrolledCourse.Id);
                    break;
                case nameof(CatalogCourse):
                    listBox.DisplayMember = nameof(CatalogCourse.CourseName);
                    listBox.ValueMember = nameof(CatalogCourse.CourseNo);
                    break;
                case nameof(AdviserApproval):
                    listBox.DisplayMember = "TEST2";    //CHECK
                    listBox.ValueMember = nameof(AdviserApproval.Id);
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

        // Event Methods

        private void btnProfileUpdate_Click(object sender, System.EventArgs e)
        {
            if (txtProfileUpdateEmail.Text == string.Empty && txtProfileUpdateFirstName.Text == string.Empty &&
                txtProfileUpdateLastName.Text == string.Empty && txtProfileUpdatePhone.Text == string.Empty)
            {
                MessageBox.Show(Messages.MustFillInTheFieldsWantToUpdate, Messages.Warning);
                return;
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
                    return;
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
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show($@"{Messages.SomethingWentWrongWhileCheckPassword}:\n\n{officerResult.Message}", Messages.ServerError);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show(Messages.MakeSureFillInAllFields, Messages.Warning);
                return;
            }
        }

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
            }
            else
            {
                MessageBox.Show($@"{Messages.SomethingWentWrongWhileGettingDepartmentDetails}:\n\n{selectedDepartmentResult.Message}", Messages.ServerError);
                return;
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
            if (txtDepartmentOperationsUpdateDepartmentName.Text == string.Empty)
            {
                MessageBox.Show(Messages.DepartmentNameCannotBeEmpty, Messages.Warning);
                return;
            }

            int selectedDepartmentNo = GetUniqueValueOfSelectedItemInListbox<Department>(listBoxDepartmentOperationsCurrentDepartments);
            var updatedDepartmentDetailsResult = _departmentService.GetByDepartmentNo(selectedDepartmentNo);

            if (!updatedDepartmentDetailsResult.Success)
            {
                MessageBox.Show($@"{Messages.SomethingWentWrongWhileGettingDepartmentDetails}:\n\n{updatedDepartmentDetailsResult.Message}", Messages.ServerError);
                return;
            }

            var messageDialogResult = MessageBox.Show($@"{updatedDepartmentDetailsResult.Data.DepartmentName} isimli bölüm güncellenecek. Onaylıyor musunuz?", Messages.UpdateConfirmation, MessageBoxButtons.YesNo);
            if (messageDialogResult == DialogResult.Yes)
            {
                updatedDepartmentDetailsResult.Data.DepartmentName = txtDepartmentOperationsUpdateDepartmentName.Text;
                UpdateOperationForUpdateButtons<Department>(_departmentService, updatedDepartmentDetailsResult.Data, ReBuildDepartmentPanel);
            }
        }

        private void btnDepartmentOperationsAddDepartment_Click(object sender, EventArgs e)
        {
            if (txtDepartmentOperationsAddDepartmentName.Text == string.Empty)
            {
                MessageBox.Show(Messages.DepartmentNameCannotBeEmpty, Messages.Warning);
                return;
            }

            var addedDepartment = new Department 
            {   
                DepartmentName = txtDepartmentOperationsAddDepartmentName.Text,
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
                return;
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
                    return;
                }
                else
                {
                    MessageBox.Show($@"{Messages.SomethingWentWrongWhileSearching}:\n\n{departmentResult.Message}", Messages.ServerError);
                    return;
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
                    return;
                }
                else
                {
                    MessageBox.Show($@"{Messages.SomethingWentWrongWhileSearching}:\n\n{departmentResult.Message}", Messages.ServerError);
                    return;
                }
            }
            else
            {
                MessageBox.Show(Messages.NotAllSearchCriteriaCanBeFilledAtOnce, Messages.Warning);
                return;
            }

        }

        private void btnDepartmentOperationsSearchReset_Click(object sender, EventArgs e)
        {
            ReBuildDepartmentPanel();
        }
    }
}
