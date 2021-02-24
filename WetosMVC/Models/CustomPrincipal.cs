using System;
using System.Security;
using System.Security.Principal;

public class CustomPrincipal : System.Security.Principal.IPrincipal
{
    public CustomPrincipal(CustomIdentity identity)
    {
        this.Identity = identity;
    }

    #region IPrincipal Members

    public IIdentity Identity { get; private set; }
    public string LastLogin { get; set; }

    public bool IsInRole(string role)
    {
        return true;
    }

    #endregion
}
