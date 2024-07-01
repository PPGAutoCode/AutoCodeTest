
namespace ProjectName.Types
{
    public class UpdateUserQuestionnaireDto
    {
        public Guid id { get; set; }
        public string CompanyErpSolutionName { get; set; }
        public string CompanyErpSolutionVersion { get; set; }
        public string CompanyKad { get; set; }
        public string CompanyLegalName { get; set; }
        public bool CompanyOwnsBankAccount { get; set; }
        public string CompanyReprEmail { get; set; }
        public string CompanyRepFullName { get; set; }
        public string CompanyRepNumber { get; set; }
        public string CompanyTaxId { get; set; }
        public bool CompanyUsesErp { get; set; }
        public string CompanyWebsite { get; set; }
        public Guid CorporateUserId { get; set; }
        public string ErpBankingActivities { get; set; }
        public Guid ProductCategoriesId { get; set; }
    }
}
