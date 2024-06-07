namespace Project.Apis.Extensions
{
    public static class FlaskApiService
    {
        public static async Task<string> GetPredictionFromFlaskAPI(IFormFile image)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://127.0.0.1:5000");

                byte[] imageData;
                using (var ms = new MemoryStream())
                {
                    await image.CopyToAsync(ms);
                    imageData = ms.ToArray();
                }

                var content = new ByteArrayContent(imageData);

                var response = await client.PostAsync("/predict", new MultipartFormDataContent
                  {
                     { content, "image", "image.jpg" } 
                  });

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                return null;
            }
        }
    }
}
