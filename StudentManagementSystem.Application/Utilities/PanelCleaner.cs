using System.Windows.Forms;

namespace StudentManagementSystem.Application.Utilities
{
    public static class PanelCleaner
    {
        public static void Clear(Panel panel)
        {
            foreach (Control panelControl in panel.Controls)
            {
                ClearControl(panelControl);
            }   
        }

        private static void ClearControl(Control control)
        {
            foreach (Control subControl in control.Controls)
            {
                ClearControl(subControl);
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
