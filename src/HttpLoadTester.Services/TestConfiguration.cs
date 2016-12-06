using System.Collections.Generic;

namespace HttpLoadTester.Services
{
    public class TestConfiguration
    {
        public string BaseUrl {get;set;}
        public int[] EpisodeIDs { get; set; }
        public int UserWaitSeconds { get; set; }
        public int ConcurrentUsersPerTest { get; set; }



    }
}