namespace SafeEscape.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> UserHasPermissionAsync(Guid userId, Guid buildingId);
    }
}
