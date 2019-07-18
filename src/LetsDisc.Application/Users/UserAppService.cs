using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.IdentityFramework;
using Abp.Localization;
using Abp.Runtime.Session;
using LetsDisc.Authorization;
using LetsDisc.Authorization.Roles;
using LetsDisc.Authorization.Users;
using LetsDisc.Roles.Dto;
using LetsDisc.Users.Dto;
using System;
using LetsDisc.PostDetails;
using LetsDisc.Posts;
using Abp.AutoMapper;
using Microsoft.AspNetCore.Http;
using System.IO;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;

namespace LetsDisc.Users
{
    public class UserAppService : AsyncCrudAppService<User, UserDto, long, PagedResultRequestDto, CreateUserDto, UserDto>, IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<Role> _roleRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<UserDetails, long> _userDetailsRepository;
        private readonly IRepository<Post> _postRepository;

        public UserAppService(
            IRepository<User, long> repository,
            UserManager userManager,
            RoleManager roleManager,
            IRepository<Role> roleRepository,
            IPasswordHasher<User> passwordHasher,
            IRepository<UserDetails, long> userDetailsRepository,
            IRepository<Post> postRepository)
            : base(repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _userDetailsRepository = userDetailsRepository;
            _postRepository = postRepository;
        }

        public override async Task<UserDto> Create(CreateUserDto input)
        {
            CheckCreatePermission();

            var user = ObjectMapper.Map<User>(input);

            user.TenantId = AbpSession.TenantId;
            user.Password = _passwordHasher.HashPassword(user, input.Password);
            user.IsEmailConfirmed = true;

            CheckErrors(await _userManager.CreateAsync(user));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }

            CurrentUnitOfWork.SaveChanges();

            return MapToEntityDto(user);
        }

        public override async Task<UserDto> Update(UserDto input)
        {
            CheckUpdatePermission();

            var user = await _userManager.GetUserByIdAsync(input.Id);

            MapToEntity(input, user);

            CheckErrors(await _userManager.UpdateAsync(user));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }

            return await Get(input);
        }

        public override async Task Delete(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            await _userManager.DeleteAsync(user);
        }

        public async Task<ListResultDto<RoleDto>> GetRoles()
        {
            var roles = await _roleRepository.GetAllListAsync();
            return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
        }

        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            );
        }

        protected override User MapToEntity(CreateUserDto createInput)
        {
            var user = ObjectMapper.Map<User>(createInput);
            user.SetNormalizedNames();
            return user;
        }

        protected override void MapToEntity(UserDto input, User user)
        {
            ObjectMapper.Map(input, user);
            user.SetNormalizedNames();
        }

        protected override UserDto MapToEntityDto(User user)
        {
            var roles = _roleManager.Roles.Where(r => user.Roles.Any(ur => ur.RoleId == r.Id)).Select(r => r.NormalizedName);
            var userDto = base.MapToEntityDto(user);
            userDto.RoleNames = roles.ToArray();
            return userDto;
        }

        protected override IQueryable<User> CreateFilteredQuery(PagedResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Roles);
        }

        protected override async Task<User> GetEntityByIdAsync(long id)
        {
            var user = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new EntityNotFoundException(typeof(User), id);
            }

            return user;
        }

        public async Task<UserInfo> GetUser(long id)
        {
            var user = await Repository.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new EntityNotFoundException(typeof(User), id);
            }

            var userDetails = await _userDetailsRepository.FirstOrDefaultAsync(x => x.UserId == id);

            if(userDetails == null)
            {
                userDetails = await _userDetailsRepository.InsertAsync(new UserDetails
                                    {
                                        UserId = user.Id,
                                        CreationTime = DateTime.Now,
                                        Views = 0,
                                        Upvotes = 0,
                                        Downvotes = 0,
                                        DisplayName = user.FullName
                                    });
            }

            var questionsCount = await _postRepository.CountAsync(p => p.PostTypeId == (int)PostTypes.Question && p.CreatorUserId == id);
            var answersCount = await _postRepository.CountAsync(p => p.PostTypeId == (int)PostTypes.Answer && p.CreatorUserId == id);

            userDetails.Views++;
            var userDto = base.MapToEntityDto(user);
            var userDetailsDto = ObjectMapper.Map<UserDetailsDto>(userDetails);

            return new UserInfo
            {
                User = userDto,
                UserDetails = userDetailsDto,
                questionsCount = questionsCount,
                answersCount = answersCount
            };
        }

        public async Task<UserInfo> UpdateUserInfo(UserInfo input)
        {
            CheckUpdatePermission();

            var user = await Repository.FirstOrDefaultAsync(x => x.Id == input.User.Id);
            var userDetails = await _userDetailsRepository.FirstOrDefaultAsync(x => x.UserId == input.User.Id);

            MapToEntity(input.User, user);

            userDetails.AboutMe = input.UserDetails.AboutMe;
            userDetails.Location = input.UserDetails.Location;
            userDetails.WebsiteUrl = input.UserDetails.WebsiteUrl;
            userDetails.ProfileImageUrl = input.UserDetails.ProfileImageUrl;

            CheckErrors(await _userManager.UpdateAsync(user));
            await _userDetailsRepository.UpdateAsync(userDetails);   
            
            return await GetUser(user.Id);
        }

        public async Task<PagedResultDto<UserDetailsDto>> GetALLUsers(PagedResultRequestDto input)
        {
            var userDetailsCount = await _userDetailsRepository.CountAsync();
            var userDetails = await _userDetailsRepository.GetAll().Include(a => a.User).ToListAsync();

            return new PagedResultDto<UserDetailsDto>
            {
                TotalCount = userDetailsCount,
                Items = userDetails.MapTo<List<UserDetailsDto>>()
            };
        }

        public async Task<string> UploadProfilePicture([FromForm(Name = "uploadedFile")] IFormFile file, long userId)
        {
            if (file == null || file.Length == 0)
                throw new UserFriendlyException("Please select profile picture");

            var folderName = Path.Combine("Resources", "ProfilePics");
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            //var fileUniqueId = Guid.NewGuid().ToString().ToLower().Replace("-", string.Empty);
            var uniqueFileName = $"{userId}_profilepic.png";

            var dbPath = Path.Combine(folderName, uniqueFileName);

            using (var fileStream = new FileStream(Path.Combine(filePath, uniqueFileName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return dbPath;
        }

    protected override IQueryable<User> ApplySorting(IQueryable<User> query, PagedResultRequestDto input)
        {
            return query.OrderBy(r => r.UserName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
