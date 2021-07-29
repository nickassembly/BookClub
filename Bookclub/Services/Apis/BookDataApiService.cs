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

            string publishDate = bookApiDetails.PublishedDate.ToString();

            // TODO: Need to add Checks for Index Out of range
            // TODO: Need to add checks for null
            // Current terenary operations not catching exceptions. 

            bookApiDetails.Isbn = bookDetails.VolumeInfo.IndustryIdentifiers[0].Identifier != null
                ? bookDetails.VolumeInfo.IndustryIdentifiers[0].Identifier : " ";
            bookApiDetails.Isbn13 = bookDetails.VolumeInfo.IndustryIdentifiers[1].Identifier != null
                ? bookDetails.VolumeInfo.IndustryIdentifiers[1].Identifier : " ";
            bookApiDetails.PrimaryAuthor = bookDetails.VolumeInfo.Authors.First() != null
                ? bookDetails.VolumeInfo.Authors.First() : " ";
            bookApiDetails.Publisher = bookDetails.VolumeInfo.Publisher != null
                ? bookDetails.VolumeInfo.Publisher : " ";
            bookApiDetails.PublishedDate = GetPublishDate(publishDate);
            bookApiDetails.ListPrice = bookDetails.SaleInfo.ListPrice.ToString() != null
                ? bookDetails.SaleInfo.ListPrice.ToString() : " ";

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

        public DateTimeOffset GetPublishDate(string publishDate)
        {
            var convertedDate = Convert.ToDateTime(publishDate);

            return convertedDate;
        }

        public static BooksService service = new BooksService(
               new BaseClientService.Initializer
               {
                   ApplicationName = "BookClub",  // is this right? not sure if it matters
                   ApiKey = "AIzaSyCjqD7OtvMLj-JMh3erdPRh_qWyRJvnvxw", //API key, needs to be moved somewhere safer
               });
    }
}
