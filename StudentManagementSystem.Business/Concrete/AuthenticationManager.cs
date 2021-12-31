using System;
using System.Collections.Generic;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Business.Constants;
using StudentManagementSystem.Core.Entities;
using StudentManagementSystem.Core.Utilities.Results;
using StudentManagementSystem.DataAccess.Concrete.Sql;

namespace StudentManagementSystem.Business.Concrete
{
    public class AuthenticationManager : IAuthenticationService
    {
        public IDataResult<IEntity> Login(string username, string password)
        {
            if (username.StartsWith(UsernameConfiguration.StudentUsernameStart))
            {
                var dal = new SqlStudentDal();
                var studentNo = username.Replace(UsernameConfiguration.StudentUsernameStart, String.Empty);
                var result = dal.Get(new Dictionary<string, dynamic>()
                    {{"ogrenci_no", studentNo}, {"sifre", password}});

                if (!result.Success)
                {
                    return new ErrorDataResult<IEntity>(Messages.IncorrectLoginInformation);
                }
                return new SuccessDataResult<IEntity>(result.Data);
            }

            if (username.StartsWith(UsernameConfiguration.OfficerUsernameStart))
            {
                var dal = new SqlOfficerDal();
                var officerNo = username.Replace(UsernameConfiguration.OfficerUsernameStart, String.Empty);
                var result = dal.Get(new Dictionary<string, dynamic>()
                    {{"memur_no", officerNo}, {"sifre", password}});

                if (!result.Success)
                {
                    return new ErrorDataResult<IEntity>(Messages.IncorrectLoginInformation);
                }
                return new SuccessDataResult<IEntity>(result.Data);
            }

            if (username.StartsWith(UsernameConfiguration.InstructorUsernameStart))
            {
                var dal = new SqlInstructorDal();
                var instructorNo = username.Replace(UsernameConfiguration.InstructorUsernameStart, String.Empty);
                var result = dal.Get(new Dictionary<string, dynamic>()
                    {{"ogretim_uye_no", instructorNo}, {"sifre", password}});

                if (!result.Success)
                {
                    return new ErrorDataResult<IEntity>(Messages.IncorrectLoginInformation);
                }
                return new SuccessDataResult<IEntity>(result.Data);
            }

            return new ErrorDataResult<IEntity>(Messages.UsernameIsIncorrect);
        }
    }
}
