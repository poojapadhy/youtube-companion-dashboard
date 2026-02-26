using System;
using System.Collections.Generic;
using System.Text;

namespace YoutubeCompanion.Application.DTOs
{
    public class VideoDetailsDto
    {
        public string VideoId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long ViewCount { get; set; }
        public long LikeCount { get; set; }
        public long CommentCount { get; set; }
    }
}
