using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rexxar.Identity.Data
{
    public class SeedData
    {
        public static List<RexxarUser> Users()
        {
            return new List<RexxarUser>{
                new RexxarUser
                {
                    UserName = "laowang",
                    Email = "520@qq.com",
                    Id = Guid.NewGuid(),
                    EmailConfirmed = true,
                    TwoFactorEnabled = false,
                    Avatar = "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1528131041794&di=78ae71a3573dc86bc010e301005fea53&imgtype=0&src=http%3A%2F%2Fpic2.orsoon.com%2F2017%2F0309%2F20170309032925886.png"
                },
                new RexxarUser
                {
                    UserName = "zhangsan",
                    Email = "521@qq.com",
                    Id = Guid.NewGuid(),
                    EmailConfirmed = true,
                    TwoFactorEnabled = false,
                    Avatar = "http://pic20.photophoto.cn/20110804/0010023712739303_b.jpg"
                },
                new RexxarUser
                {
                    UserName = "lisi",
                    Email = "521@qq.com",
                    Id = Guid.NewGuid(),
                    EmailConfirmed = true,
                    TwoFactorEnabled = false,
                    Avatar = "http://p1.qzone.la/upload/0/14vy5x96.jpg"
                }
            };
        }
    }
}
