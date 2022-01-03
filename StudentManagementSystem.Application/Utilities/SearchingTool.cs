using System.Collections.Generic;
using System.Windows.Forms;
using StudentManagementSystem.Business.Constants;
using StudentManagementSystem.Core.Entities;
using StudentManagementSystem.Core.Utilities.Results;

namespace StudentManagementSystem.Application.Utilities
{
    public static class SearchingTool
    {
        public static void ApplySearchToListbox<T>(ListBox targetListbox, IDataResult<List<T>> searchResult, List<TextBox> textBoxesToClearAfterProcess)
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
    }
}
