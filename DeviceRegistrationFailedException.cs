// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.DeviceRegistrationFailedException
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.Crm.Services.Utility
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  internal sealed class DeviceRegistrationFailedException : Exception
  {
    internal DeviceRegistrationFailedException()
    {
    }

    internal DeviceRegistrationFailedException(DeviceRegistrationErrorCode code, string subCode)
      : this(code, subCode, (Exception) null)
    {
    }

    internal DeviceRegistrationFailedException(
      DeviceRegistrationErrorCode code,
      string subCode,
      Exception innerException)
      : base(code.ToString() + ": " + subCode, innerException)
    {
    }

    private DeviceRegistrationFailedException(SerializationInfo si, StreamingContext sc)
      : base(si, sc)
    {
    }
  }
}
