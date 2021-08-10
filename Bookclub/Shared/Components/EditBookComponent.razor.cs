using Bookclub.BooksAggregateModels;
using Bookclub.Core.DomainAggregates;
using Bookclub.Core.Interfaces;
using Bookclub.Shared.Colors;
using Bookclub.Shared.Components.ContainerComponents;
using Bookclub.Shared.Interfaces;
using Bookclub.Views.Bases;
using Microsoft.AspNetCore.Components;
using System;
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
        public BookView BookView { get; set; }
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

        private string Isbn { get; set; }
        private string Isbn13 { get; set; }
        private string Author { get; set; }
        private string Title { get; set; }
        private string Subtitle { get; set; }
        private string Publisher { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Guid testGuid = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6");

            await GetBookById(testGuid);
        }

        public async Task<Book> GetBookById(Guid bookId)
        {
            Book bookToUpdate = new Book();

            var bookResponse = await BookViewService.GetBookById(bookId);

            BookToUpdate = bookResponse.Book;

            return BookToUpdate;
        }

        //protected override void OnInitialized()
        //{
        //    // TODO: Need better way to handle dataflow
        //    // Add GetById so that Razor page pulls a book in from database directly.

        //    this.BookView = new BookView
        //    {
        //        Id = BookToEdit.Id,
        //        Isbn = BookToEdit.Isbn,
        //        Isbn13 = BookToEdit.Isbn13,
        //        Title = BookToEdit.Title,
        //        Subtitle = BookToEdit.Subtitle,
        //        PrimaryAuthor = BookToEdit.Author,
        //        Publisher = BookToEdit.Publisher,
        //        ListPrice = BookToEdit.ListPrice.ToString(),
        //        PublishedDate = BookToEdit.PublishDate
        //        // MediaType = BookToEdit.MediaType
        //        // TODO: Fix Media Type
        //    };

        //    this.State = ComponentState.Content;
        //}

        // TODO: need client side validation.

        public async void EditBookAsync(Book bookToEdit)
        {

            decimal uneditedListPrice = bookToEdit.ListPrice;

            try
            {

                bookToEdit.Isbn = !string.IsNullOrEmpty(Isbn) ? Isbn : bookToEdit.Isbn;
                bookToEdit.Author = !string.IsNullOrEmpty(Author) ? Author : bookToEdit.Author;
                bookToEdit.Isbn13 = !string.IsNullOrEmpty(Isbn13) ? Isbn13 : bookToEdit.Isbn13;
                bookToEdit.Title = !string.IsNullOrEmpty(Title) ? Title : bookToEdit.Title;
                bookToEdit.Subtitle = !string.IsNullOrEmpty(Subtitle) ? Subtitle : bookToEdit.Subtitle;
                bookToEdit.Publisher = !string.IsNullOrEmpty(Publisher) ? Publisher : bookToEdit.Publisher;

                // TODO: Date not being edited properly
                bookToEdit.PublishDate = PublishDateInput != default ? PublishDateInput : bookToEdit.PublishDate;

                if (Convert.ToDecimal(BookListPrice) == 0)
                    bookToEdit.ListPrice = uneditedListPrice;
                else
                    bookToEdit.ListPrice = Convert.ToDecimal(BookListPrice);

                await BookViewService.EditBookAsync(bookToEdit);
                ReportEditingSuccess();
                NavigationManager.NavigateTo("books", true);
            }
            catch (System.Exception)
            {

                throw;
            }

        }

        public async void CancelEditAsync()
        {
            NavigationManager.NavigateTo("books", true);
        }

        public async void GetApiDataAsync()
        {
            GoogleApiRequest googleRequest = new();
            googleRequest.Isbn = Isbn;
            googleRequest.Isbn13 = Isbn13;
            googleRequest.Title = Title;

            var apiBookData = await BookDataApiService.GetGoogleBookData(googleRequest);

            BookView.Isbn = apiBookData.Isbn;
            BookView.Isbn13 = apiBookData.Isbn13;
            BookView.PrimaryAuthor = apiBookData.PrimaryAuthor;
            BookView.Title = apiBookData.Title;
            BookView.Subtitle = apiBookData.Subtitle;
            BookView.Publisher = apiBookData.Publisher;
            BookView.PublishedDate = apiBookData.PublishedDate;
            BookView.ListPrice = apiBookData.ListPrice;

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
