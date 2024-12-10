using JobPlatform.Exceptions;

namespace JobPlatform.Util
{
    public class FileProcessor
    {
        public static string UploadImage(IFormFile file)
        {
            List<string> validExtentions = new List<string>() { ".jpg", ".png", ".gif" };
            string extention = Path.GetExtension(file.FileName);
            if (!validExtentions.Contains(extention))
            {
                throw new FileUploadException($"Upload Error! The file must be of type {string.Join(", ", validExtentions)}");
            }
            long size = file.Length;
            if(size >2 *  1024 * 1024)
            {
                throw new FileUploadException("Upload Error. The image size must not exceed 2MB");
            }
            string fileName = Guid.NewGuid().ToString() + extention;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
            using FileStream stream = new FileStream(Path.Combine(path, fileName + extention), FileMode.Create);
            file.CopyTo(stream);

            return fileName;

        }

        public static string UploadPdf(IFormFile file)
        {
            string fileExtention = Path.GetExtension(file.FileName);
            if(!(fileExtention == ".pdf"))
            {
                throw new FileUploadException("Upload Error. The file must be of type pdf");
            }
            long size = file.Length;
            if(size > 5 *1024 * 1024)
            {
                throw new FileUploadException("Upload Error. The file size must not exceed 5MB");
            }
            string fileName = Guid.NewGuid().ToString() + fileExtention;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Pdfs");
            using FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create);
            file.CopyTo(stream);

            return fileName;
        }

        public static bool DeleteImage(string fileName)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", fileName);
            if(File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool DeletePdf(string fileName)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Pdfs", fileName);
            if(File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
