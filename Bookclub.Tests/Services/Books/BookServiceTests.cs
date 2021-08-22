//using Moq;
//using System;
//using System.Linq.Expressions;
//using Tynamix.ObjectFiller;

//namespace Bookclub.Tests.Services.Books
//{
//    public partial class BookServiceTests
//    {
//        private readonly Mock<IApiBroker> _apiBrokerMock;
//        private readonly Mock<ILoggingBroker> _loggingBrokerMock;
//        private readonly IBookService _bookService;

//        public BookServiceTests()
//        {
//            _apiBrokerMock = new Mock<IApiBroker>();
//            _loggingBrokerMock = new Mock<ILoggingBroker>();

//            _bookService = new BookService(_apiBrokerMock.Object, _loggingBrokerMock.Object);
//        }

//        private static Book CreateRandomBook() => CreateBookFiller().Create();

//        private Expression<Func<Exception, bool>> SameExceptionAs(Exception expectedException)
//        {
//            return actualException => actualException.Message == expectedException.Message
//            && actualException.InnerException.Message == expectedException.InnerException.Message;
//        }

//        private static string GetRandomString() => new MnemonicString().GetValue();

//        private static Filler<Book> CreateBookFiller()
//        {
//            var filler = new Filler<Book>();

//            filler.Setup().OnType<DateTimeOffset>().Use(DateTimeOffset.UtcNow);

//            return filler;
//        }


//    }
//}
