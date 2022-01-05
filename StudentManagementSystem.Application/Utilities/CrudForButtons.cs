using System;
using System.Windows.Forms;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Business.Concrete;
using StudentManagementSystem.Business.Constants;
using StudentManagementSystem.Core.Entities;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.DataAccess.Concrete.Sql;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Application.Utilities
{
    public static class CrudForButtons
    {
        public static void AddOperationForAddButtons<T>(IEntityCrudService<T> service, T addedEntity, Delegate methodToRunAfterAdd)
            where T : class, IEntity, new()
        {
            IResult addResult;

            if (typeof(T) == typeof(CatalogCourse))
            {
                CatalogCourse addedCourse = (CatalogCourse)Convert.ChangeType(addedEntity, typeof(CatalogCourse));
                var departmentService = new DepartmentManager(new SqlDepartmentDal());
                var result = departmentService.GetByDepartmentNo(addedCourse.DepartmentNo);
                if (!result.Success)
                {
                    MessageBox.Show(Messages.SomethingWentWrongWhileFetchingData, Messages.ServerError);
                    return;
                }

                addResult = ((ICatalogCourseService)service).AddWithDepartmentTotalSemester(addedCourse, result.Data.NumberOfSemester);
            }
            else
            {
                addResult = service.Add(addedEntity);
            }

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
            IResult updateResult;

            if (typeof(T) == typeof(CatalogCourse))
            {
                CatalogCourse updatedCourse = (CatalogCourse)Convert.ChangeType(updatedEntity, typeof(CatalogCourse));
                var departmentService = new DepartmentManager(new SqlDepartmentDal());
                var result = departmentService.GetByDepartmentNo(updatedCourse.DepartmentNo);
                if (!result.Success)
                {
                    MessageBox.Show(Messages.SomethingWentWrongWhileFetchingData, Messages.ServerError);
                    return;
                }

                updateResult = ((ICatalogCourseService)service).UpdateWithDepartmentTotalSemester(updatedCourse, result.Data.NumberOfSemester);
            }else if (typeof(T) == typeof(Student))
            {
                Student updatedStudent = (Student)Convert.ChangeType(updatedEntity, typeof(Student));
                var departmentService = new DepartmentManager(new SqlDepartmentDal());
                var result = departmentService.GetByDepartmentNo(updatedStudent.DepartmentNo);
                if (!result.Success)
                {
                    MessageBox.Show(Messages.SomethingWentWrongWhileFetchingData, Messages.ServerError);
                    return;
                }

                updateResult = ((IStudentService)service).UpdateWithDepartmentTotalSemester(updatedStudent, result.Data.NumberOfSemester);
            }
            else
            {
                updateResult = service.Update(updatedEntity);
            }

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
