using System;
using System.Windows.Forms;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Business.Constants;
using StudentManagementSystem.Core.Entities;

namespace StudentManagementSystem.Application.Utilities
{
    public static class CrudForButtons
    {
        public static void AddOperationForAddButtons<T>(IEntityCrudService<T> service, T addedEntity, Delegate methodToRunAfterAdd)
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

        public static void DeleteOperationForDeleteButtons<T>(IEntityCrudService<T> service, T deletedEntity, Delegate methodToRunAfterDeletion)
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

        public static void UpdateOperationForUpdateButtons<T>(IEntityCrudService<T> service, T updatedEntity, Delegate methodToRunAfterUpdate)
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
    }
}
