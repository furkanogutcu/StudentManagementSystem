using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.Business.Abstract
{
    public interface IOfficerService : IEntityCrudService<Officer>
    {
        IDataResult<Officer> GetByOfficerNo(int officerNo);
    }
}
