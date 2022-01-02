namespace StudentManagementSystem.Business.Constants
{
    public static class Messages
    {
        public static string IncorrectLoginInformation = "Kullanici adiniz veya sifreniz hatali. Kontrol edip tekrar deneyiniz";
        public static string UsernameIsIncorrect = "Kullanici adiniz hatali. Kontrol edip tekrar deneyiniz";
        public static string OnlyNumberForPhone = "'Telefon', alanına yalnızca rakam girebilirsiniz";

        public static string ServerError = "Sunucu hatası";
        public static string Error = "Hata";
        public static string Warning = "Uyarı";
        public static string Successful = "Başarılı";
        public static string Information = "Bilgilendirme";

        private static readonly string _errorStatement = "bir şeyler ters gitti";

        public static string SomethingWentWrongWhileGettingCurrentDepartments = $@"Mevcut bölümleri alırken {_errorStatement}";
        public static string SomethingWentWrongWhileGettingCurrentCourses = $@"Mevcut dersleri alırken {_errorStatement}";
        public static string SomethingWentWrongWhileGettingCurrentInstructors = $@"Mevcut öğretim görevlisi listesini alırken {_errorStatement}";
        public static string SomethingWentWrongWhileGettingDepartmentDetails = $@"Bölüm detaylarını alırken {_errorStatement}";
        public static string SomethingWentWrongWhileGettingCourseDetails = $@"Ders detaylarını alırken {_errorStatement}";
        public static string SomethingWentWrongWhileGettingNewProfileInfos = $@"Yeni profil bilgilerini alırken {_errorStatement}";
        public static string SomethingWentWrongWhilePasswordChange = $@"Şifre değişikliği sırasında {_errorStatement}";
        public static string SomethingWentWrongWhileCheckPassword = $@"Şifrenizi kontrol ederken {_errorStatement}";
        public static string SomethingWentWrongWhileUpdate = $@"Güncelleme işlemi sırasında {_errorStatement}";
        public static string SomethingWentWrongWhileAddition = $@"Ekleme işlemi sırasında {_errorStatement}";
        public static string SomethingWentWrongWhileDeletion = $@"Silme işlemi sırasında {_errorStatement}";
        public static string SomethingWentWrongWhileSearching = $@"Arama yapılırken {_errorStatement}";

        public static string ProfileHasBeenUpdated = "Profil başarıyla güncellendi";
        public static string DeletionComplete = "Silme işlemi başarıyla tamamlandı";
        public static string AdditionComplete = "Ekleme işlemi başarıyla tamamlandı";
        public static string UpdateComplete = "Güncelleme işlemi başarıyla tamamlandı";

        public static string DeleteConfirmation = "Silme onayı";
        public static string AddConfirmation = "Ekleme onayı";
        public static string UpdateConfirmation = "Güncelleme onayı";

        public static string MustFillInTheFieldsWantToUpdate = "Tüm alanlar boş olamaz.Güncellemek istediğiniz alanları doldurmalısınız";
        public static string TheDataToBeUpdatedIsTheSameAsTheOldData = "Güncellemek istediğiniz veriler eski veriler ile aynı";
        public static string MakeSureFillInAllFields = "Tüm alanları doldurduğunuzdan emin olunuz";
        public static string NotAllSearchCriteriaCanBeFilledAtOnce = "Tüm arama kriterleri aynı anda doldurulamaz";
        public static string ApplicationIsRestarting = "Uygulama yeniden başlatılıyor";
        
        public static string NewPasswordsDoNotMatch = "Yeni şifreler eşleşmiyor";
        public static string NewPasswordCannotBeSameAsOldPassword = "Yeni şifre, eski şifre ile aynı olamaz";
        public static string PasswordHasBeenChanged = "Şifreniz başarıyla değiştirildi";
        public static string LoginAgainWithNewPassword = "Yeni şifrenizle tekrar giriş yapınız";
        public static string OldPasswordEnteredDoesNotMatchCurrentPassword = "Girilen eski şifre, mevcut şifreyle eşleşmiyor";
        
        public static string DepartmentNumberMustConsistOfNumbersOnly= "Bölüm numarası yalnızca rakamlardan oluşmalıdır";
        public static string DepartmentNumberOfSemesterMustConsistOfNumbersOnly= "Bölüm dönemi yalnızca rakamlardan oluşmalıdır";
        public static string CourseCreditMustConsistOfNumbersOnly= "Ders kredisi yalnızca rakamlardan oluşmalıdır";
        public static string CourseYearMustConsistOfNumbersOnly= "Ders yılı yalnızca rakamlardan oluşmalıdır";
        public static string CourseSemesterMustConsistOfNumbersOnly= "Ders dönemi yalnızca rakamlardan oluşmalıdır";
        public static string CourseNoMustConsistOfNumbersOnly= "Ders kodu yalnızca rakamlardan oluşmalıdır";

        public static string ThereMustBeAtLeastOneDepartmentToBeAbleToAddACourse= "Ders ekleyebilmek için en az bir bölüm olmalıdır";
        public static string ThereMustBeAtLeastOneInstructorToBeAbleToAddACourse= "Ders ekleyebilmek için en az bir öğretim görevlisi olmalıdır";
        public static string AtLeastOneFilterMustBeOn = "Filtreleme yapabilmek için en az bir filtre açık olmalıdır";

        public static string OfficerLogin = "Memur girişi";
        public static string StudentLogin = "Öğrenci girişi";
        public static string InstructorLogin = "Öğretim görevlisi girişi";

        public static string CreateSearchResultMessage(int recordFoundInSearch)
        {
            return $@"Arama sonucunda {recordFoundInSearch} adet kayıt bulundu";
        }
        public static string CreateFilterResultMessage(int recordFoundInFilter)
        {
            return $@"Filtreleme sonucunda {recordFoundInFilter} adet kayıt kaldı";
        }
    }
}
