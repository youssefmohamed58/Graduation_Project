namespace Project.Apis.Extensions
{
    public static class ImagePath
    {
        public static string SaveImageFile(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null; // Handle the case where no file is provided or the file is empty
            }

            // Generate a unique filename for the image (you may want to include user-specific information)
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

            // Specify the folder where you want to save the image
            var filePath = Path.Combine("wwwroot/images/profile", fileName);

            // Save the image to the specified path
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            // Return the relative path to the saved image
            return $"/images/profile/{fileName}";
        }

        
    }
}
