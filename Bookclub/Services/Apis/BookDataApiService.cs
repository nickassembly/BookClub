using Bookclub.BooksAggregateModels;
using Bookclub.Core.DomainAggregates;
using Bookclub.Core.Interfaces;
using Bookclub.Shared.Interfaces;
using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookclub.Services.Apis
{
    public class BookDataApiService : IBookDataApiService
    {
        public async Task<BookView> GetGoogleBookData(GoogleApiRequest apiRequest)
        {
            var bookDetails = new Volume();

            bookDetails = await GetVolume(apiRequest.Isbn);

            var bookReturned = JsonConvert.SerializeObject(bookDetails);

            BookView bookApiDetails = new();

           //TODO figure out how to get Media type, links, images, etc
           // Possibly extract to a separate method
            bookApiDetails.Isbn = bookDetails.VolumeInfo.IndustryIdentifiers[0].Identifier;
            bookApiDetails.Isbn13 = bookDetails.VolumeInfo.IndustryIdentifiers[1].Identifier;
            bookApiDetails.PrimaryAuthor = bookDetails.VolumeInfo.Authors.FirstOrDefault();
            bookApiDetails.Publisher = bookDetails.VolumeInfo.Publisher;
            bookApiDetails.PublishedDate = Convert.ToDateTime(bookDetails.VolumeInfo.PublishedDate);
            bookApiDetails.ListPrice = bookDetails.SaleInfo.ListPrice.ToString();

            return bookApiDetails;
        }

        public static async Task<Volume> GetVolume(string isbn)
        {
            var result = await service.Volumes.List(isbn).ExecuteAsync();
            if (result != null && result.Items != null)
            {
                var item = result.Items.FirstOrDefault();
                return item;
            }
            return null;
        }

        public static BooksService service = new BooksService(
               new BaseClientService.Initializer
               {
                   ApplicationName = "BookClub",  // is this right? not sure if it matters
                   ApiKey = "AIzaSyCjqD7OtvMLj-JMh3erdPRh_qWyRJvnvxw", //API key, needs to be moved somewhere safer
               });
    }
}
