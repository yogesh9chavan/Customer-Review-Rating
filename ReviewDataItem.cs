using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomerReviewRating
{
    public class ReviewDataItem
    {
        public string reviewerID;
        public string asin;
        public string reviewerName;
        public object helpful;
        public string reviewText;
        public float overall;
        public string summary;
        public string unixReviewTime;
        public string reviewTime;
    }
}
