using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using EmployeeDataApp.Models;

namespace EmployeeDataApp.Helpers
{
    public static class HelperFactory
    {
        public static EmployeeControllerHelper Create(IPrincipal principal, IEmployeeDbContext context = null)
        {
            if (principal.IsInRole("Admin"))
                return new AdminHelper();

            if (principal.IsInRole("Client"))
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context),
                        "For the \"Client\" role argument must be not null");
                return new ClientHelper(context);
            }
#if !DEBUG
            throw new InvalidOperationException("Incorrect user role for this operation");
#endif
            if (context == null)
                throw new ArgumentNullException(nameof(context),
                "For the \"Client\" role argument must be not null");
            return new ClientHelper(context);
        }
    }
}