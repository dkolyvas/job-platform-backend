namespace JobPlatform.DTO.User
{
    public class UserViewDTO
    {
        public string? Username { get; set; }
        public long? Id { get; set; }   


        public string? Email { get; set; }

        public string? Role { get; set; }
        public long? EntityId { get; set; }
        public int? UnauthorizedCount { get; set; }
        public string? RestoreCode {  get; set; }
    }
}
