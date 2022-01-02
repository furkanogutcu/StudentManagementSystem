using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Business.Constants;
using StudentManagementSystem.Core.Entities;

namespace StudentManagementSystem.Application.Utilities
{
    public static class FilteringTool
    {
        public static void FilterListBox<T>(ListBox listboxToFilter, IEntityCrudService<T> service, List<Func<T, bool>> conditions, string errorMessage)
            where T : class, IEntity, new()
        {
            var entityListResult = service.GetAll();

            if (entityListResult.Success)
            {
                List<T> filteredList = entityListResult.Data;
                foreach (var condition in conditions)
                {
                    filteredList = filteredList.Where(condition).ToList();
                }
                DataSetterToBoxes.SetDataToListBox(listboxToFilter, filteredList);
                MessageBox.Show(Messages.CreateFilterResultMessage(filteredList.Count), Messages.Information);
            }
            else
            {
                MessageBox.Show(errorMessage, Messages.ServerError);
            }
        }
    }
}
