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

            string searchValue =
                !string.IsNullOrWhiteSpace(apiRequest.Isbn13)
                ? apiRequest.Isbn13
                : !string.IsNullOrWhiteSpace(apiRequest.Isbn)
                ? apiRequest.Isbn
                : !string.IsNullOrWhiteSpace(apiRequest.Title)
                ? apiRequest.Title
                : string.Empty;

            if (!string.IsNullOrWhiteSpace(searchValue))
                bookDetails = await GetVolume(searchValue);

            if (bookDetails == null || string.IsNullOrWhiteSpace(searchValue))
            {
                BookView emptyBookViewData = new();

                return emptyBookViewData;
            }
            else
            {
                BookView bookApiDetails = new();

                string publishDate = bookApiDetails.PublishedDate.ToString();

                var industryIdentifiers = bookDetails?.VolumeInfo?.IndustryIdentifiers.ToList();

                if (industryIdentifiers.Count() > 1)
                {
                    bookApiDetails.Isbn = industryIdentifiers[0].Type == "ISBN_10"
                        ? industryIdentifiers[0].Identifier : string.Empty;
                    bookApiDetails.Isbn13 = industryIdentifiers[1].Type == "ISBN_13"
                        ? industryIdentifiers[1].Identifier : string.Empty;
                }

                bookApiDetails.PrimaryAuthor = bookDetails?.VolumeInfo?.Authors?.First() ?? string.Empty;
                bookApiDetails.Publisher = bookDetails?.VolumeInfo?.Publisher ?? string.Empty;
                bookApiDetails.PublishedDate = GetPublishDate(publishDate);
                bookApiDetails.ListPrice = bookDetails?.SaleInfo?.ListPrice?.Amount.ToString() ?? string.Empty;

                return bookApiDetails;
            }

        }

        public static async Task<Volume> GetVolume(string searchValue)
        {
            var result = await service.Volumes.List(searchValue).ExecuteAsync();

            if (result != null && result.Items != null)
            {
                // TODO: Need method to look at list of results return and get the best one,
                // currently it just gets the first result.
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
                   ApplicationName = "BookClub", 
                   ApiKey = "AIzaSyCjqD7OtvMLj-JMh3erdPRh_qWyRJvnvxw", // TODO: Move Api key to better area or config file
               });
    }
}
