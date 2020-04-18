﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Security
{ 
    public class CustomEmailConfirmationTokenProvider<TUser>
        : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public CustomEmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
                    IOptions<CustomEmailConfirmationTokenProviderOptions> options, ILogger<CustomEmailConfirmationTokenProvider<TUser>> logger )
            : base(dataProtectionProvider, options,logger)
        {

        }
        
    }
}
