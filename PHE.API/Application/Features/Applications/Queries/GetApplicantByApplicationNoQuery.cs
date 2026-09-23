namespace PHE.API.Application.Features.Applications.Queries
{
    public class GetApplicantByApplicationNoQuery
    {
        public GetApplicantByApplicationNoQuery(string applicationNo)
        {
            ApplicationNo = applicationNo;
        }

        public string ApplicationNo { get; }
    }
}
