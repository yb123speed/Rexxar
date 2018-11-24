using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Rexxar.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rexxar.Identity
{
    public class ProfileService : IProfileService
    {
        UserManager<RexxarUser> _userManager;

        // 注入AspNetCore Identity的用户管理类
        public ProfileService(UserManager<RexxarUser> userManager)
        {
            this._userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var claims = context.Subject.Claims.ToList();
            // sub属性就是用户id
            var userId = claims.First(r => r.Type == "sub");
            // 查找用户
            var user = await _userManager.FindByIdAsync(userId.Value);
            claims.Add(new Claim("username", user.UserName));
            claims.Add(new Claim("email", user.Email));
            claims.Add(new Claim("avatar", user.Avatar));
            // 这里是设置token包含的用户属性claim
            context.IssuedClaims = claims;
        }

        /// <summary>
        /// 用户是否激活
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            await Task.CompletedTask;
        }
    }
}
