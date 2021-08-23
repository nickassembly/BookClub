using Blazored.SessionStorage;
using Bookclub.Core.DomainAggregates;
using Bookclub.Core.Interfaces;
using Bookclub.Views.Bases;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Color = Bookclub.Shared.Colors.Color;

namespace Bookclub.Views.Pages
{
    public partial class Books
    {
        public LabelBase StatusLabel { get; set; }
        bool ShowEditComponent { get; set; } = false;
        bool ShowAddComponent { get; set; } = false;
        bool ShowBookList { get; set; } = true;

        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _ctx;

        public Books(IBookService bookService, IUserService userService, IHttpContextAccessor ctx)
        {
            _bookService = bookService;
            _userService = userService;
            _ctx = ctx;
        }

        public Books()
        {
            // TODO: Research why a parameterless constructor is needed here
        }

        [Parameter]
        public Book BookToEdit { get; set; }

        [Inject]
        public IBookViewService BookViewService { get; set; }

        public List<Book> BookList { get; set; } = new List<Book>();

        protected override async Task OnInitializedAsync()
        {
            await GetBooks();
        }

        public async Task<IEnumerable<Book>> GetBooks()
        {
            List<Book> bookList = new List<Book>();

            var userEmail = await SessionStorageService.GetItemAsync<string>("emailAddress");

            // TODO: This is null
            User loggedInUser = await _userService.GetCurrentlyLoggedInUser(_ctx.HttpContext, userEmail);

            var bookResponse = await BookViewService.GetAllBooks();

            BookList = bookResponse.Books.Where(x => x.CreatedBy.ToString() == loggedInUser.Id.ToString()).ToList();
            
            return bookList;
        }

        public async Task<BookResponse> DeleteBookAsync(Guid bookId)
        {
            try
            {
                ApplyDeletingStatus();
                await BookViewService.DeleteBookAsync(bookId);
                ReportBookDeletionSucceeded();
                NavigationManager.NavigateTo("books", true);
            }
            catch (Exception ex)
            {
                return new BookResponse { ResponseMessage = $"Delete Book Exception: {ex.Message}" };
            }

            return null;
        }

        public void ToggleEdit(Book book)
        {
            BookToEdit = book;
            ShowEditComponent = true;
            ShowBookList = false;
        }

        public void ToggleAdd()
        {
            ShowAddComponent = true;
            ShowBookList = false;
        }

        private void ApplyDeletingStatus()
        {
            this.StatusLabel.SetColor(Color.Black);
            this.StatusLabel.SetValue("Deleting ... ");
        }

        private void ReportBookDeletionSucceeded()
        {
            this.StatusLabel.SetColor(Color.Red);
            this.StatusLabel.SetValue("Deleted Successfully");
        }

    }
}
