using System.Collections;

namespace WebAPI_Test.DTO
{
    public class TodoList_SelectDTO
    {
        public int Id { get; set; }

        public string ColName { get; set; } = null!;

        public int ColAge { get; set; }

        public string ColAddress { get; set; } = null!;

        public string ColProfession { get; set; } = null!;

        public string ColPosition { get; set; } = null!;

        public string? ColNote { get; set; }

        public DateTime DateTime { get; set; }

        public ICollection<UploadDataDTO>? Upload { get; set; }
    }
}
