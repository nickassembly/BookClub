using Bookclub.BooksAggregateModels;
using Bookclub.Core.DomainAggregates;
using Bookclub.Core.Interfaces;
using Bookclub.Shared.Colors;
using Bookclub.Shared.Components.ContainerComponents;
using Bookclub.Shared.Interfaces;
using Bookclub.Views.Bases;
using Microsoft.AspNetCore.Components;
using System;
using System.Data.SqlTypes;
using System.Globalization;
using System.Threading.Tasks;

namespace Bookclub.Shared.Components
{
    public partial class EditBookComponent
    {
        private readonly IBookService _bookService;

        public EditBookComponent(IBookService bookService)
        {
            _bookService = bookService;
        }

        public EditBookComponent()
        {
        }

        [Inject]
        public IBookViewService BookViewService { get; set; }
        public Book BookToUpdate { get; set; }
        public BookView BookViewToUpdate { get; set; }

        [Inject]
        public IBookDataApiService BookDataApiService { get; set; }
        [Inject]
        public IBookService BookService { get; set; }

        [Parameter]
        public Book BookToEdit { get; set; }

        [Parameter]
        public string BookListPrice { get; set; }
        public ComponentState State { get; set; }
        // public AddBookComponentException Exception { get; set; }

        public TextBoxBase IsbnTextBox { get; set; }
        public TextBoxBase Isbn13TextBox { get; set; }
        public TextBoxBase TitleTextBox { get; set; }
        public TextBoxBase SubtitleTextBox { get; set; }
        public TextBoxBase AuthorTextBox { get; set; }
        //public DropDownBase<BookViewMediaType> MediaTypeDropDown { get; set; }
        public TextBoxBase PublisherTextBox { get; set; }
        public TextBoxBase ListPrice { get; set; }
        public DatePickerBase PublishDatePicker { get; set; }
        public ButtonBase ConfirmEditButton { get; set; }
        public ButtonBase CancelEditButton { get; set; }
        public ButtonBase AddBookDetailsButton { get; set; }
        public LabelBase StatusLabel { get; set; }

        // TODO: Need better way to handle publish date picker
        private DateTimeOffset _publishDateInput;
        [Parameter]
        public DateTimeOffset PublishDateInput
        {
            get => _publishDateInput;
            set
            {
                if (_publishDateInput == value) return;
                _publishDateInput = value;
                PublishDateInputChanged.InvokeAsync(value);
            }
        }

        [Parameter]
        public EventCallback<DateTimeOffset> PublishDateInputChanged { get; set; }

        protected async override Task OnInitializedAsync()
        {
            var bookToUpdate = await GetBookById(BookToEdit.Id);

            this.BookViewToUpdate = new BookView
            {
                Isbn = bookToUpdate.Isbn,
                Isbn13 = bookToUpdate.Isbn13,
                PrimaryAuthor = bookToUpdate.Author,
                Title = bookToUpdate.Title,
                Subtitle = bookToUpdate.Subtitle,
                Publisher = bookToUpdate.Publisher,
                PublishedDate = bookToUpdate.PublishDate,
                ListPrice = bookToUpdate.ListPrice.ToString(),
            };

            this.State = ComponentState.Content;
        }

        public async Task<Book> GetBookById(Guid bookId)
        {
            Book bookToUpdate = new Book();

            var bookResponse = await BookViewService.GetBookById(bookId);

            BookToUpdate = bookResponse.Book;

            return BookToUpdate;
        }

        public async void EditBookAsync(BookView bookToEdit)
        {
            try
            {
                Book book = new();

                decimal bookListPrice;

                if (decimal.TryParse(bookToEdit.ListPrice, out bookListPrice) == false)
                    book.ListPrice = 0;
                else
                    book.ListPrice = bookListPrice;

                book.Isbn = !string.IsNullOrWhiteSpace(bookToEdit.Isbn) ? bookToEdit.Isbn : "No Isbn Available";
                book.Isbn13 = !string.IsNullOrWhiteSpace(bookToEdit.Isbn13) ? bookToEdit.Isbn13 : "No Isbn13 Available";
                book.Title = !string.IsNullOrWhiteSpace(bookToEdit.Title) ? bookToEdit.Title : "No Title Available";
                book.Subtitle = !string.IsNullOrEmpty(bookToEdit.Subtitle) ? bookToEdit.Subtitle : "";
                book.Author = !string.IsNullOrEmpty(bookToEdit.PrimaryAuthor) ? bookToEdit.PrimaryAuthor : "";
                book.Publisher = !string.IsNullOrEmpty(bookToEdit.Publisher) ? bookToEdit.Publisher : "";

                book.PublishDate = PublishDateInput != default ? PublishDateInput : DateTimeOffset.MinValue;

                await BookViewService.EditBookAsync(book);
                ReportEditingSuccess();
                NavigationManager.NavigateTo("books", true);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw;
            }

        }

        public async void CancelEditAsync()
        {
            NavigationManager.NavigateTo("books", true);
        }

        public async void GetApiDataAsync(BookView bookToEdit)
        {
            GoogleApiRequest googleRequest = new();
            googleRequest.Isbn = bookToEdit.Isbn;
            googleRequest.Isbn13 = bookToEdit.Isbn13;
            googleRequest.Title = bookToEdit.Title;

            var apiBookData = await BookDataApiService.GetGoogleBookData(googleRequest);

            BookViewToUpdate.Isbn = apiBookData.Isbn;
            BookViewToUpdate.Isbn13 = apiBookData.Isbn13;
            BookViewToUpdate.PrimaryAuthor = apiBookData.PrimaryAuthor;
            BookViewToUpdate.Title = apiBookData.Title;
            BookViewToUpdate.Subtitle = apiBookData.Subtitle;
            BookViewToUpdate.Publisher = apiBookData.Publisher;
            BookViewToUpdate.PublishedDate = apiBookData.PublishedDate;
            BookViewToUpdate.ListPrice = apiBookData.ListPrice;

            StateHasChanged();

            // TODO: Add validation on form to require Isbn10 or 13 and title
        }

        public Book GetNewBookInfo()
        {
            Book newBookInfo = new Book();

            NavigationManager.NavigateTo("editbookcomponent", true);

            return newBookInfo;
        }

        private void ReportEditingSuccess()
        {
            this.StatusLabel.SetColor(Color.Green);
            this.StatusLabel.SetValue("Book Edited Successfully");
        }

    }
}
