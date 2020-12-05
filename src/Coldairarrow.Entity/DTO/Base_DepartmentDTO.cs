using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using Coldairarrow.Util.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.Entity
{
    [Map(typeof(Base_Department))]
    public class Base_DepartmentDTO : Base_Department
    {
        public new string SecretKey
        {
            set
            {
                base.SecretKey = value;
            }
            get
            {
                if (base.SecretKey.IsNullOrEmpty()) return string.Empty;
                return EncryptionHelper.Decode(base.SecretKey, this.ApiKey);
            }
        }
    }
}
