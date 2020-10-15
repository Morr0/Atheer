namespace AtheerBackend.DTOs
{
    public struct BlogPostPrimaryKey
    {
        public BlogPostPrimaryKey(int createdYear, string titleShrinked)
        {
            CreatedYear = createdYear;
            TitleShrinked = titleShrinked;
        }

        public int CreatedYear { get; set; }
        public string TitleShrinked { get; set; }

        public static bool operator ==(BlogPostPrimaryKey first, BlogPostPrimaryKey second)
        {
            return first.CreatedYear == second.CreatedYear && first.TitleShrinked == second.TitleShrinked;
        }

        public static bool operator !=(BlogPostPrimaryKey first, BlogPostPrimaryKey second)
        {
            return !(first == second);
        }
    }
}