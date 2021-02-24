using System;

public class CustomIdentity : System.Security.Principal.IIdentity
{
    public CustomIdentity(String name)
    {
        this.Name = name;
    }

    #region IIdentity Members

    public string AuthenticationType
    {
        get { return "Custom"; }
    }

    public bool IsAuthenticated
    {
        get { return !string.IsNullOrEmpty(this.Name); }
    }

    public string Name
    {
        get;
        private set;
    }

    public string LastLogin
    {
        get;
        set;
    }

    #endregion
}

