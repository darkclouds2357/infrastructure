namespace Alidu.Common.Interfaces
{
    public interface IHeaderCredential
    {
        string OwnerId { get; }
        string OrgId { get; }
        string WorkingOrgId { get; }

        void SetClaims(string claims);
    }
}