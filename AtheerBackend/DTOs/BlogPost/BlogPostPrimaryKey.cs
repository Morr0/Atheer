namespace AtheerBackend.DTOs.BlogPost
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
    }
}