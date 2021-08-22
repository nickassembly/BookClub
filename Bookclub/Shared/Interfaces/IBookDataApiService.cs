using Bookclub.BooksAggregateModels;
using Bookclub.Core.DomainAggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookclub.Shared.Interfaces
{
    public interface IBookDataApiService
    {
        Task<BookView> GetGoogleBookData(GoogleApiRequest apiRequest);
    }
}
