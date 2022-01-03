using System.Windows.Forms;

namespace StudentManagementSystem.Application.Utilities
{
    public static class EnableStateSwitcher
    {
        public static void SwitchUpdateTextBoxEnabled(CheckBox controlCheckBox, TextBox targetTextBox)
        {
            targetTextBox.Enabled = true;
            targetTextBox.ReadOnly = !controlCheckBox.Checked;
        }
        public static void SwitchUpdateComboBoxEnabled(CheckBox controlCheckBox, ComboBox controlComboBox)
        {
            controlComboBox.Enabled = controlCheckBox.Checked;
        }
    }
}
