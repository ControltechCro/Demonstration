using System;
using UAManagedCore;

//-------------------------------------------
// WARNING: AUTO-GENERATED CODE, DO NOT EDIT!
//-------------------------------------------

[MapType(NamespaceUri = "Demonstration", Guid = "07b10933e4ee4d4cbf84518e190ea9be")]
public class FTRemoteAccessWidgetDataObject : UAObject
{
#region Children properties
    //-------------------------------------------
    // WARNING: AUTO-GENERATED CODE, DO NOT EDIT!
    //-------------------------------------------
    public bool IncomingConnectionRequest
    {
        get
        {
            return (bool)Refs.GetVariable("IncomingConnectionRequest").Value.Value;
        }
        set
        {
            Refs.GetVariable("IncomingConnectionRequest").SetValue(value);
        }
    }
    public IUAVariable IncomingConnectionRequestVariable
    {
        get
        {
            return (IUAVariable)Refs.GetVariable("IncomingConnectionRequest");
        }
    }
    public string Username
    {
        get
        {
            return (string)Refs.GetVariable("Username").Value.Value;
        }
        set
        {
            Refs.GetVariable("Username").SetValue(value);
        }
    }
    public IUAVariable UsernameVariable
    {
        get
        {
            return (IUAVariable)Refs.GetVariable("Username");
        }
    }
    public string IpAddress
    {
        get
        {
            return (string)Refs.GetVariable("IpAddress").Value.Value;
        }
        set
        {
            Refs.GetVariable("IpAddress").SetValue(value);
        }
    }
    public IUAVariable IpAddressVariable
    {
        get
        {
            return (IUAVariable)Refs.GetVariable("IpAddress");
        }
    }
    public UAManagedCore.ByteString SupervisorId
    {
        get
        {
            return (UAManagedCore.ByteString)Refs.GetVariable("SupervisorId").Value.Value;
        }
        set
        {
            Refs.GetVariable("SupervisorId").SetValue(value);
        }
    }
    public IUAVariable SupervisorIdVariable
    {
        get
        {
            return (IUAVariable)Refs.GetVariable("SupervisorId");
        }
    }
    public UAManagedCore.NodeId FTRemoteAccessNode
    {
        get
        {
            return (UAManagedCore.NodeId)Refs.GetVariable("FTRemoteAccessNode").Value.Value;
        }
        set
        {
            Refs.GetVariable("FTRemoteAccessNode").SetValue(value);
        }
    }
    public FTOptix.Core.Alias FTRemoteAccessNodeVariable
    {
        get
        {
            return (FTOptix.Core.Alias)Refs.GetVariable("FTRemoteAccessNode");
        }
    }
#endregion
}
