using StudentManagementSystem.Core.Entities;
using StudentManagementSystem.Core.Utilities.Results;

namespace StudentManagementSystem.Business.Abstract
{
    public interface IAuthenticationService
    {
        IDataResult<IEntity> Login(string username, string password);
    }
}
