using Blazored.SessionStorage;
using Bookclub.Core.DomainAggregates;
using Bookclub.Core.Interfaces;
using Bookclub.Views.Bases;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        [CascadingParameter]
        private Task<AuthenticationState> _authState { get; set; }

        private AuthenticationState authState;

        private readonly IUserService _userService;
        private readonly IBookService _bookService;

        public Books(IBookService bookService, IUserService userService)
        {
            _bookService = bookService;
            _userService = userService;
      
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

        // TODO: Difference between injecting in Razor and code behind
        // do i need to use DI in both places or one or the other? 
        public async Task<IEnumerable<Book>> GetBooks()
        {
            List<Book> bookList = new List<Book>();

            var authState = await _authState;

            if (authState != null)
            {
 
            }


            var bookResponse = await BookViewService.GetAllBooks();

            BookList = bookResponse.Books;
            
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
