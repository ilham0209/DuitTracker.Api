namespace DuitTracker.Api.Shared.Domain;

public class BaseClass
{
    public bool IsDeleted { get; set; }
    public string? SysUserCreated { get; set; }
    public DateTime? SysDateCreated { get; set; }
    public string? SysUserModified { get; set; }
    public DateTime? SysDateModified { get; set; }

    public BaseClass()
    {
        IsDeleted = false;
    }

    public void SetCreated(string user)
    {
        SysUserCreated = user;
        SysDateCreated = DateTime.UtcNow;
    }

    public void SetModified(string user)
    {
        SysUserModified = user;
        SysDateModified = DateTime.UtcNow;
    }

    public void SetDeleted()
    {
        IsDeleted = true;
    }
}