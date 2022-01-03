using System.Collections.Generic;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Abstract
{
    public interface IAdviserApprovalService : IEntityCrudService<AdviserApproval>
    {
        IDataResult<List<AdviserApproval>> GetAllByStudentNo(int studentNo);
        IDataResult<List<AdviserApproval>> GetAllByCourseNo(int courseNo);
    }
}
