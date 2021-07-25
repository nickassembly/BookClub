using Blazored.SessionStorage;
using Bookclub.ApiModels.CreateBook;
using Bookclub.Core.DomainAggregates;
using Bookclub.Core.Interfaces;
using Bookclub.Core.Services.Books;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bookclub.Services.BookViews

{
    public partial class BookViewService : IBookViewService
    {
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly ISessionStorageService _sessionStorage;
        private readonly IHttpContextAccessor _ctx;

        private readonly HttpClient _httpClient;

        public BookViewService(
            IBookService bookService,
            IUserService userService,
            HttpClient httpClient,
            ISessionStorageService sessionStorage,
            IHttpContextAccessor ctx)
        {
            _bookService = bookService;
            _userService = userService;
            _httpClient = httpClient;
            _sessionStorage = sessionStorage;
            _ctx = ctx;
        }

        public Task<BookResponse> GetAllBooks()
        {
            return _bookService.GetAllBooks();
        }

        public async ValueTask<BookView> AddBookViewAsync(BookView bookView)
        {
            Book book = await MapToBook(bookView);
            await _bookService.AddBookAsync(book);
            return bookView;
        }


        public async Task<BookResponse> EditBookAsync(Book bookToEdit)
        {
            // TODO: Need to move this logic to different class IBookDataApi implementation

            var bookDetails = new Google.Apis.Books.v1.Data.Volume();

            bookDetails = await SearchISBN(bookToEdit.Isbn);

            bookToEdit.Author = bookDetails.VolumeInfo.Authors[0]; //TODO: Authors is array, should change book model to match
            bookToEdit.Title = bookDetails.VolumeInfo.Title;
            bookToEdit.Subtitle = bookDetails.VolumeInfo.Subtitle;
            //bookToEdit.PublishDate = Date(bookDetails.VolumeInfo.PublishedDate);
            bookToEdit.Publisher = bookDetails.VolumeInfo.Publisher;
            bookToEdit.Title = bookDetails.VolumeInfo.Title;
            //bookToEdit.ListPrice = (double)bookDetails.SaleInfo.ListPrice.Amount;
            
            //bookToEdit.Isbn = bookDetails.VolumeInfo.IndustryIdentifiers[0]["identifier"] TODO: Check for IndustryIdentifiers array for type ISBN_10 or ISBN_13

            return await _bookService.EditBookAsync(bookToEdit);
        }

        public Task<BookResponse> DeleteBookAsync(Guid bookId)
        {
            return _bookService.DeleteBookAsync(bookId);
        }

        // TODO: Remove this code once method is abstracted to interface and working properly
        public static async Task<Google.Apis.Books.v1.Data.Volume> SearchISBN(string isbn)
        {
            var result = await service.Volumes.List(isbn).ExecuteAsync();
            if (result != null && result.Items != null)
            {
                var item = result.Items.FirstOrDefault();
                return item;
            }
            return null;
        }

        public static Google.Apis.Books.v1.BooksService service = new Google.Apis.Books.v1.BooksService(
               new BaseClientService.Initializer
               {
                   ApplicationName = "BookClub",  // is this right? not sure if it matters
                   ApiKey = "AIzaSyCjqD7OtvMLj-JMh3erdPRh_qWyRJvnvxw", //nicky API key
               });

        public async Task<Book> MapToBook(BookView bookView)
        {

            var userEmail = await _sessionStorage.GetItemAsync<string>("emailAddress");

            User loggedInUser = await _userService.GetCurrentlyLoggedInUser(_ctx.HttpContext, userEmail);

            if (loggedInUser == null)
                return await Task.FromResult<Book>(null);

            DateTimeOffset currentDateTime = DateTimeOffset.UtcNow;

            bool isValidPriceInput = Decimal.TryParse(bookView.ListPrice, out decimal bookListPrice);

            if (!isValidPriceInput)
                bookListPrice = 0.00m;

            return new Book
            {
                Id = Guid.NewGuid(),
                Isbn = bookView.Isbn,
                Isbn13 = bookView.Isbn13,
                Author = bookView.PrimaryAuthor,
                Title = bookView.Title,
                Subtitle = bookView.Subtitle,
                PublishDate = bookView.PublishedDate,
                Publisher = bookView.Publisher,
                CreatedBy = loggedInUser.Id,
                UpdatedBy = loggedInUser.Id,
                CreatedDate = currentDateTime,
                UpdatedDate = currentDateTime,
                ListPrice = bookListPrice
            };

        }


    }
}
