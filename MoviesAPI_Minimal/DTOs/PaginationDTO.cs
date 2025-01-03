using Microsoft.IdentityModel.Tokens;

namespace MoviesAPI_Minimal.DTOs
{
    public class PaginationDTO
    {
        private const int pageInitialValue = 1;
        private const int recordsPerPageInitialValue = 10;
        public int Page { get; set; } = 1;
        private int recordsPerPage { get; set; } = 10;
        private readonly int recordsPerPageMax = 50;

        public int RecordsPerPage 
            {
            get
            {
                return recordsPerPage;
            }
            set 
            {
                if(value > recordsPerPageMax)
                {
                    recordsPerPage = recordsPerPageMax;
                }
                else
                {
                    recordsPerPage = value;
                }
            }
        }
        public static ValueTask<PaginationDTO> BindAsync(HttpContext context)
        {
            //nameof(page) = "page"
            var page = context.Request.Query[nameof(Page)];
            var recordsPerPage = context.Request.Query[nameof(RecordsPerPage)];

            var pageInt = page.IsNullOrEmpty() ? pageInitialValue : int.Parse(page.ToString());
            var recordsPerPageInt = recordsPerPage.IsNullOrEmpty() 
                ? recordsPerPageInitialValue : int.Parse(recordsPerPage.ToString());

            var response = new PaginationDTO
            {
                Page = pageInt,
                RecordsPerPage = recordsPerPageInt
            };

            return ValueTask.FromResult(response);
        }
    }
}
