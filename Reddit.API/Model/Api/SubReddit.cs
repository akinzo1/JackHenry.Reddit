using StackExchange.Redis;

namespace Reddit.API.Model.Api
{
    public class SubReddit : APILimits
    {
        public required string Kind { get; set; }


        public required ListingData Data { get; set; }


        public class ListingData
        {
            public int Dist { get; set; }
            public required List<SubRedditList> Children { get; set; }


        }

        public class SubRedditList
        {
            public required string Kind { get; set; }
            public required SubRedditData Data { get; set; }

        }

        public class SubRedditData
        {

            public required string Author { get; set; }
            public required int Ups { get; set; }
            public required int Downs { get; set; }
            public required string Title { get; set; }
            public required long Created { get; set; }
            public required string SelfText { get; set; }
            public required string SelfText_HTML { get; set; }
            public required int num_comments { get; set; }

            //Derived property that would
            //convert UnixTime to standard datetime
            public DateTime CreatedDate
            {
                get
                {
                    return Extensions.UnixSecondsToDateTime(Created, true);
                }

            }
        }

        public class UserCounts
        {
            public required string Author { get; set; }
            public int TotalPosts { get; set; }
        }

    }

    //Other/Similar datatypes could inherit from this class
    //to determine how to adjust calls that need to be made
    public class APILimits
    {
        public int RateLimit_Remaining { get; set; }
        public int RateLimit_Used { get; set; }
        public int RateLimit_Reset { get; set; }
    }
}
