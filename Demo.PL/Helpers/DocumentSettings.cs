using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.Language;
using System;
using System.IO;

namespace Demo.PL.Helpers
{
    public class DocumentSettings
    {
        public static string UploadFile(IFormFile file,string folderName)
        {
            //1. Get Located Folder Path
            //string folderPath= "E:\\Courses\\ASP.NET (ROUTE)\\05 MVC\\Session 03\\WebApplication2\\Demo.PL\\wwwroot\\Fils\\" + folderName;
            //string folderPath = Directory.GetCurrentDirectory() + @"\wwwroot\Fils\" + folderName;
            string folderPath =Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Fils", folderName);

            //2. Get File Name and Make it UNIQUE
            string fileName = $"{Guid.NewGuid()}{file?.FileName?? "Person.jpg"}";

            //3. Get File Path
            string filePath = Path.Combine(folderPath, fileName);

            //4. Save File as Streems : [Data Per Time]
            using var fileStream = new FileStream(filePath, FileMode.Create);

            file?.CopyTo(fileStream);
             
            return fileName;
        }
        public static void DeleteFile(string fileName,string folderName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Fils", folderName,fileName);
            if(File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
