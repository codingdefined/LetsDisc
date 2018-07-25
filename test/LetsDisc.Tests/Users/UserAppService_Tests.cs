﻿using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using Abp.Application.Services.Dto;
using LetsDisc.Users;
using LetsDisc.Users.Dto;
using LetsDisc.Roles;
using LetsDisc.Roles.Dto;
using System.Collections.Generic;

namespace LetsDisc.Tests.Users
{
    public class UserAppService_Tests : LetsDiscTestBase
    {
        private readonly IUserAppService _userAppService;
        private readonly IRoleAppService _roleAppService;

        public UserAppService_Tests()
        {
            _userAppService = Resolve<IUserAppService>();
            _roleAppService = Resolve<IRoleAppService>();
        }

        [Fact]
        public async Task GetUsers_Test()
        {
            // Act
            var output = await _userAppService.GetAll(new PagedResultRequestDto{MaxResultCount=20, SkipCount=0} );

            // Assert
            output.Items.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task CreateUser_Test()
        {
            // Act
            await _userAppService.Create(
                new CreateUserDto
                {
                    EmailAddress = "john@volosoft.com",
                    IsActive = true,
                    Name = "John",
                    Surname = "Nash",
                    Password = "123qwe",
                    UserName = "john.nash"
                });

            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task CreateRole_Test()
        {
            List<string> permissions = new List<string>();
            permissions.Add("Pages.Users");
            await _roleAppService.Create(new CreateRoleDto
            {
                Name = "User",
                IsStatic = false,
                DisplayName = "User",
                Permissions = permissions
            });

            await UsingDbContextAsync(async context =>
            {
                var userRole = await context.Roles.FirstOrDefaultAsync(u => u.Name == "User");
                userRole.ShouldNotBeNull();
            });

        }
    }
}
