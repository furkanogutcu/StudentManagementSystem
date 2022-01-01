using System.Windows.Forms;

namespace StudentManagementSystem.Application.Utilities
{
    public static class PanelCleaner
    {
        public static void Clean(Panel panel)
        {
            foreach (Control panelControl in panel.Controls)
            {
                CleanControl(panelControl);
            }   
        }

        private static void CleanControl(Control control)
        {
            foreach (Control subControl in control.Controls)
            {
                CleanControl(subControl);
            }

            switch (control)
            {
                case TextBox textBox:
                    textBox.Clear();
                    break;
                case RadioButton radioButton:
                    radioButton.Checked = false;
                    break;
                case CheckBox checkBox:
                    checkBox.Checked = false;
                    break;
                case CheckedListBox checkedListBox:
                    checkedListBox.DataSource = null;
                    checkedListBox.Items.Clear();
                    break;
                case ListBox listBox:
                    listBox.DataSource = null;
                    listBox.Items.Clear();
                    break;
                case ComboBox comboBox:
                    comboBox.DataSource = null;
                    comboBox.Items.Clear();
                    break;
            }
        }
    }
}
