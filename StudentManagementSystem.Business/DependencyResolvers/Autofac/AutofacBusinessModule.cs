using Autofac;
using StudentManagementSystem.Business.Abstract;
using StudentManagementSystem.Business.Concrete;
using StudentManagementSystem.DataAccess.Abstract;
using StudentManagementSystem.DataAccess.Concrete.Sql;

namespace StudentManagementSystem.Business.DependencyResolvers.Autofac
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AdviserApprovalManager>().As<IAdviserApprovalService>().SingleInstance();
            builder.RegisterType<SqlAdviserApprovalDal>().As<IAdviserApprovalDal>().SingleInstance();

            builder.RegisterType<CatalogCourseManager>().As<ICatalogCourseService>().SingleInstance();
            builder.RegisterType<SqlCatalogCourseDal>().As<ICatalogCourseDal>().SingleInstance();

            builder.RegisterType<DepartmentManager>().As<IDepartmentService>().SingleInstance();
            builder.RegisterType<SqlDepartmentDal>().As<IDepartmentDal>().SingleInstance();

            builder.RegisterType<EnrolledCourseManager>().As<IEnrolledCourseService>().SingleInstance();
            builder.RegisterType<SqlEnrolledCourseDal>().As<IEnrolledCourseDal>().SingleInstance();

            builder.RegisterType<InstructorManager>().As<IInstructorService>().SingleInstance();
            builder.RegisterType<SqlInstructorDal>().As<IInstructorDal>().SingleInstance();

            builder.RegisterType<OfficerManager>().As<IOfficerService>().SingleInstance();
            builder.RegisterType<SqlOfficerDal>().As<IOfficerDal>().SingleInstance();

            builder.RegisterType<StudentManager>().As<IStudentService>().SingleInstance();
            builder.RegisterType<SqlStudentDal>().As<IStudentDal>().SingleInstance();

            builder.RegisterType<AuthenticationManager>().As<IAuthenticationService>().SingleInstance();
        }
    }
}
