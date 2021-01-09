// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using IdentityServer4.Models;

namespace FinanceMonitor.Identity
{
    public static class Config
    {
        public static ApiResource[] ApiResources =
        {
            new("api", new[] {"openid", "profile"})
            {
                Scopes = new List<string> {"scope2"}
            }
        };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new[]
            {
                new("scope1"),
                new ApiScope("scope2")
            };

        public static IEnumerable<Client> Clients =>
            new[]
            {
                // m2m client credentials flow client
                new()
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = {new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256())},

                    AllowedScopes = {"scope1"}
                },

                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = {new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256())},

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris =
                    {
                        "https://localhost:44300/signin-oidc",
                        "http://localhost:3000/signin"
                    },
                    FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                    PostLogoutRedirectUris =
                    {
                        "http://localhost:3000/logout-callback",
                        "https://localhost:44300/signout-callback-oidc"
                    },
                    AllowedCorsOrigins = {"http://localhost:3000"},

                    AllowOfflineAccess = true,
                    AllowedScopes = {"openid", "profile", "scope2"},
                    AccessTokenLifetime = (int) TimeSpan.FromDays(10).TotalSeconds
                },

                new Client
                {
                    ClientId = "swagger",
                    ClientSecrets = {new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256())},

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris =
                    {
                        "https://localhost:5001/swagger/oauth2-redirect.html"
                    },
                    AllowedCorsOrigins = {"https://localhost:5001"},

                    AllowOfflineAccess = true,
                    AllowedScopes = {"scope2"},
                    AccessTokenLifetime = (int) TimeSpan.FromDays(10).TotalSeconds
                }
            };
    }
}