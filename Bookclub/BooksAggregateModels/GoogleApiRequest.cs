using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookclub.BooksAggregateModels
{
    public class GoogleApiRequest
    {
        public string Isbn { get; set; }
        public string Isbn13 { get; set; }
        public string Title { get; set; }
        public List<string> Authors { get; set; }
    }
}
