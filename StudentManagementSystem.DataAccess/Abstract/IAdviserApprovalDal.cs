using StudentManagementSystem.Core.DataAccess.Sql;
using StudentManagementSystem.Entities.Concrete;

namespace StudentManagementSystem.DataAccess.Abstract
{
    public interface IAdviserApprovalDal : IEntityRepository<AdviserApproval>
    {
    }
}
