using Bookclub.Core.DomainAggregates;
using System.Collections.Generic;

namespace Bookclub.Core.DomainAggregates
{
    public class BookResponse
    {
        // TODO: Make Generic Response to handle all CRUD functions

        public string ResponseMessage { get; set; }

        public List<Book> Books { get; set; }

        public Book Book { get; set; }
    }


}
