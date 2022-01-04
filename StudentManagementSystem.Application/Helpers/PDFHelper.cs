using System;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using StudentManagementSystem.Entities.Concrete;
using StudentManagementSystem.Entities.Views;

namespace StudentManagementSystem.Application.Helpers
{
    public static class PDFHelper
    {
        public static PdfPTable CreateA4TranskriptBody(Student student, Department department, Dictionary<EnrolledCourse, CatalogCourse> enrolledCourses, bool isRotating)
        {
            // https://stackoverflow.com/questions/4023118/html-to-pdf-turkish-character-problem
            // https://www.c-sharpcorner.com/blogs/create-table-in-pdf-using-c-sharp-and-itextsharp

            BaseFont STF_Helvetica_Turkish = BaseFont.CreateFont("Helvetica", "CP1254", BaseFont.NOT_EMBEDDED);
            var fontNormal = new iTextSharp.text.Font(STF_Helvetica_Turkish, 9, iTextSharp.text.Font.NORMAL);
            var fontBold = new iTextSharp.text.Font(STF_Helvetica_Turkish, 9, iTextSharp.text.Font.BOLD);
            var headerFont = new iTextSharp.text.Font(STF_Helvetica_Turkish, 14, iTextSharp.text.Font.BOLD);
            BaseColor baseColorGray = new BaseColor(224, 224, 224);

            PdfPTable transcriptTable = new PdfPTable(5);
            transcriptTable.HorizontalAlignment = 0;
            transcriptTable.TotalWidth = isRotating ? 820f : 575f;
            transcriptTable.LockedWidth = true;
            float[] widths = new float[] { 20f, 60f, 40f, 30f, 40f };
            transcriptTable.SetWidths(widths);
            transcriptTable.AddCell(new PdfPCell(new Phrase($"{student.FirstName.ToUpper()} {student.LastName.ToUpper()}\nBAŞARI DURUM BELGESİ", headerFont))
            {
                Colspan = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = baseColorGray,
                MinimumHeight = 60f,
                FixedHeight = 60f,
            });

            var emptyFullCell = new PdfPCell(new Phrase(" ", fontNormal))
            {
                Colspan = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            };

            var dateTime = DateTime.Now;
            transcriptTable.AddCell(new PdfPCell(new Phrase($"Tarih: {(dateTime.Day.ToString().Length == 1 ? "0" : "")}{dateTime.Day}/{(dateTime.Day.ToString().Length == 1 ? "0" : "")}{dateTime.Month}/{dateTime.Year}", fontBold))
            {
                Colspan = 5,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                MinimumHeight = 40f,
            });

            transcriptTable.AddCell(new PdfPCell(new Phrase("Öğrenci no:", fontBold)) { BackgroundColor = baseColorGray, VerticalAlignment = Element.ALIGN_MIDDLE });
            transcriptTable.AddCell(new PdfPCell(new Phrase($"{student.StudentNo}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE });
            transcriptTable.AddCell(new PdfPCell(new Phrase(" ", fontBold)) { Rowspan = 3, Colspan = 1 });
            transcriptTable.AddCell(new PdfPCell(new Phrase("Bölümü:", fontBold)) { BackgroundColor = baseColorGray, VerticalAlignment = Element.ALIGN_MIDDLE });
            transcriptTable.AddCell(new PdfPCell(new Phrase($"{department.DepartmentName}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE });

            transcriptTable.AddCell(new PdfPCell(new Phrase("Adı:", fontBold)) { BackgroundColor = baseColorGray, VerticalAlignment = Element.ALIGN_MIDDLE });
            transcriptTable.AddCell(new PdfPCell(new Phrase($"{student.FirstName.ToUpper()}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE });
            transcriptTable.AddCell(new PdfPCell(new Phrase("Soyadı:", fontBold)) { BackgroundColor = baseColorGray, VerticalAlignment = Element.ALIGN_MIDDLE });
            transcriptTable.AddCell(new PdfPCell(new Phrase($"{student.LastName.ToUpper()}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE });

            transcriptTable.AddCell(new PdfPCell(new Phrase("Kayıt yılı:", fontBold)) { BackgroundColor = baseColorGray, VerticalAlignment = Element.ALIGN_MIDDLE });
            transcriptTable.AddCell(new PdfPCell(new Phrase($"{student.EnrollmentDate}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE });
            transcriptTable.AddCell(new PdfPCell(new Phrase("Mezuniyet yılı:", fontBold)) { BackgroundColor = baseColorGray, VerticalAlignment = Element.ALIGN_MIDDLE });
            transcriptTable.AddCell(new PdfPCell(new Phrase($"{(department.NumberOfSemester / 2) + student.EnrollmentDate}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE });

            transcriptTable.AddCell(emptyFullCell);

            for (int i = 0; i < student.Semester; i++)
            {
                var addedYear = Convert.ToInt32(i / 2);
                var first = "Güz";
                var second = "Bahar";

                transcriptTable.AddCell(new PdfPCell(new Phrase($"{i + 1}. Yarıyıl : {student.EnrollmentDate + addedYear} - {student.EnrollmentDate + addedYear + 1} {(i % 2 == 0 ? first : second)}", fontBold))
                {
                    Colspan = 5,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    BackgroundColor = baseColorGray
                });

                transcriptTable.AddCell(new PdfPCell(new Phrase("Ders Kodu", fontBold)) { BackgroundColor = baseColorGray, MinimumHeight = 25, FixedHeight = 25, VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });
                transcriptTable.AddCell(new PdfPCell(new Phrase("Ders Adı", fontBold)) { BackgroundColor = baseColorGray, MinimumHeight = 25, FixedHeight = 25, VerticalAlignment = Element.ALIGN_MIDDLE });
                transcriptTable.AddCell(new PdfPCell(new Phrase("Kredisi", fontBold)) { BackgroundColor = baseColorGray, MinimumHeight = 25, FixedHeight = 25, VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });
                transcriptTable.AddCell(new PdfPCell(new Phrase("Başarı Notu", fontBold)) { BackgroundColor = baseColorGray, MinimumHeight = 25, FixedHeight = 25, VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });
                transcriptTable.AddCell(new PdfPCell(new Phrase("Durumu", fontBold)) { BackgroundColor = baseColorGray, MinimumHeight = 25, FixedHeight = 25, VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });

                var semesterTotalCourse = 0;
                var totalCredit = 0;
                var totalCompleteCredit = 0;
                var semesterTotalGrade = 0;

                foreach (var enrolledCourse in enrolledCourses)
                {
                    var courseDetails = enrolledCourse.Value;

                    if (courseDetails.CourseSemester != i + 1)
                    {
                        continue;
                    }

                    semesterTotalCourse++;

                    var enrolledCourseView = new EnrolledCourseView
                    {
                        CourseName = courseDetails.CourseName,
                        CourseNo = courseDetails.CourseNo,
                        InstructorFullName = "",
                        VizeResult = enrolledCourse.Key.VizeResult,
                        FinalResult = enrolledCourse.Key.FinalResult,
                        ButunlemeResult = enrolledCourse.Key.ButunlemeResult
                    };

                    transcriptTable.AddCell(new PdfPCell(new Phrase($"{courseDetails.CourseNo}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE, MinimumHeight = 25, FixedHeight = 25, HorizontalAlignment = Element.ALIGN_CENTER });
                    transcriptTable.AddCell(new PdfPCell(new Phrase($"{courseDetails.CourseName}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE, MinimumHeight = 25, FixedHeight = 25 });
                    transcriptTable.AddCell(new PdfPCell(new Phrase($"{courseDetails.Credit}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE, MinimumHeight = 25, FixedHeight = 25, HorizontalAlignment = Element.ALIGN_CENTER });
                    transcriptTable.AddCell(new PdfPCell(new Phrase($"{enrolledCourseView.GetCompletionGrade()}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE, MinimumHeight = 25, FixedHeight = 25, HorizontalAlignment = Element.ALIGN_CENTER });
                    transcriptTable.AddCell(new PdfPCell(new Phrase($"{(enrolledCourseView.GetStatus() == null ? "" : enrolledCourseView.GetStatus() == true ? "BAŞARILI" : "BAŞARISIZ")}", fontNormal)) { VerticalAlignment = Element.ALIGN_MIDDLE, MinimumHeight = 25, FixedHeight = 25, HorizontalAlignment = Element.ALIGN_CENTER });

                    totalCredit += courseDetails.Credit;
                    totalCompleteCredit += enrolledCourseView.GetStatus() == null ? 0 :
                        enrolledCourseView.GetStatus() == true ? courseDetails.Credit : 0;
                    semesterTotalGrade += enrolledCourseView.GetCompletionGrade() == null
                        ? 0
                        : Convert.ToInt32(enrolledCourseView.GetCompletionGrade() * courseDetails.Credit);
                }

                transcriptTable.AddCell(new PdfPCell(new Phrase(" ", fontBold)) { Rowspan = 2, Colspan = 2 });
                transcriptTable.AddCell(new PdfPCell(new Phrase("Alınan Ders Sayısı", fontBold)) { BackgroundColor = baseColorGray, HorizontalAlignment = Element.ALIGN_CENTER });
                transcriptTable.AddCell(new PdfPCell(new Phrase("A/T Kredi", fontBold)) { BackgroundColor = baseColorGray, HorizontalAlignment = Element.ALIGN_CENTER });
                transcriptTable.AddCell(new PdfPCell(new Phrase("Dönem Puanı", fontBold)) { BackgroundColor = baseColorGray, HorizontalAlignment = Element.ALIGN_CENTER });

                transcriptTable.AddCell(new PdfPCell(new Phrase($"{semesterTotalCourse}", fontNormal)) { HorizontalAlignment = Element.ALIGN_CENTER });
                transcriptTable.AddCell(new PdfPCell(new Phrase($"{totalCredit} | {totalCompleteCredit}", fontNormal)) { HorizontalAlignment = Element.ALIGN_CENTER });
                transcriptTable.AddCell(new PdfPCell(new Phrase($"{Math.Round(((float)semesterTotalGrade / totalCredit), 2)}", fontNormal)) { HorizontalAlignment = Element.ALIGN_CENTER });

                transcriptTable.AddCell(new PdfPCell(new Phrase(" "))
                {
                    Colspan = 5,
                    MinimumHeight = 10,
                    FixedHeight = 10,
                });
            }

            return transcriptTable;
        }
    }
}
