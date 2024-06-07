namespace Project.Apis.DTOS
{
    public class CurrentUserDto
    {
        public string FullName { get; set; }

        public string Email { get; set; }
        public string Token { get; set; }
        public string PictureUrl { get; set; }
    }
}
