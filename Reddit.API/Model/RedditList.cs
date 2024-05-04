namespace Reddit.API.Model
{
    public class RedditList
    {
        public string ListName { get; set; }

        public RedditList() { }

        public RedditList(string list)
        {
            ListName = list;
        }
    }
}
