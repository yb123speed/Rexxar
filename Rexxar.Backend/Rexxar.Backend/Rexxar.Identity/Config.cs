using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rexxar
{
    public class Config
    {
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client()
                {
                    ClientId="mvc",     //客户端Id
                    ClientName="MVC Client",
                    ClientUri ="http://localhost:5001",
                    LogoUri = "http://pic.qiantucdn.com/58pic/19/02/97/03m58PICDxF_1024.jpg",

                    //AllowedGrantTypes = GrantTypes.Implicit,  //OAuth2.0 简单模式模式

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    ClientSecrets =new List<Secret>  //客户端所需密钥
                    {
                        new Secret("secret".Sha256())
                    },

                    RequireConsent=true,

                    RedirectUris={ "http://localhost:5001/signin-oidc"},
                    PostLogoutRedirectUris ={ "http://localhost:5001/signout-callback-oidc"},

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                    }, //可访问的Resource
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser=true
                },
                new Client()
                {
                    //客户端Id
                    ClientId = "chat_client",
                    //客户端名称
                    ClientName = "chat client",
                    //Token有效时长
                    AccessTokenLifetime = 3600,
                    //配置TOKEN类型，reference为引用类型，数据不会存在Token中
                    AccessTokenType =AccessTokenType.Jwt,
                    //配置客户端授权模式
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    //配置客户端连接密码
                    ClientSecrets = { new Secret("secret".Sha256()), },
                    //客户端允许的请求范围
                    AllowedScopes = {
                        "chatapi",
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                    },
                    //允许离线,即开启refresh_token
                    AllowOfflineAccess = true,
                    RequireClientSecret = false,
                }

            };
        }


        public static IEnumerable<ApiResource> GetApiResouces()
        {
            return new List<ApiResource>
            {
                new ApiResource("api","My Api"),
                // 定义api资源 这里如果使用构造函数传入Name会默认创建一个同名的Scope，
                // 这点需要注意，因为这个Api如果没有Scope，那根本无法访问
                new ApiResource
                {
                    Name="chatapi",
                    DisplayName="chat api",
                    ApiSecrets= { new Secret("secret".Sha256()) },
                    Scopes={
                        new Scope("chatapi","chat api")
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId="10000",
                    Username="yebin",
                    Password = "123456",
                    Claims =new List<Claim> {
                        new Claim("xxx","904896209@qq.com"),
                        new Claim("website","chaney.club"),
                        new Claim("nickname","叶少哲")
                    }
                }
            };
        }
    }
}
