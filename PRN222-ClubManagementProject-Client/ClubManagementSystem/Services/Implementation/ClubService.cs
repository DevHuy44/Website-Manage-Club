using BussinessObjects.Models;
using BussinessObjects.Models.Dtos;
using Repositories.Implementation;
using Repositories.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IAccountService _accountService;
        private readonly IImageHelperService _imageHelperService;
        private readonly IClubMemberService _clubMemberService;
        private readonly IPostService _postService;
        public ClubService(IClubRepository clubRepository, IAccountService accountService, IImageHelperService imageHelperService, IClubMemberService clubMemberService, IPostService postService)
        {
            _clubRepository = clubRepository;
            _accountService = accountService;
            _imageHelperService = imageHelperService;
            _clubMemberService = clubMemberService;
            _postService = postService;
        }

        public async Task<ClubDetailsViewDto> GetClubDetailsAsync(int clubId, int postNumber, int postSize)
        {
            var club = await _clubRepository.GetClubByIdWithMembersPostsAsync(clubId);

            if (club == null) return null;

            var clubMemberDtos = (await _clubMemberService.GetClubMembersAsync(club.ClubId, true)) 
                .Select(member => new ClubMemberDto
                {
                    UserId = member.UserId,
                    Username = member.User.Username,
                    ProfilePictureBase64 = _imageHelperService.ConvertToBase64(member.User.ProfilePicture, "png")
                })
                .ToList();


            var postsQuery = (await _postService.GetPostsAsync(club.ClubId, "Approved"))
                    .OrderByDescending(post => post.CreatedAt);


            var totalPosts = postsQuery.Count();
            var posts = postsQuery.Skip((postNumber - 1) * postSize).Take(postSize).ToList();

            var postDtos = posts.Select(post => new PostDetailsDto
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                ImageBase64 = _imageHelperService.ConvertToBase64(post.Image, "png"),
                CreatedAt = post.CreatedAt,
                Status = post.Status,

                User = new UserDto
                {
                    UserId = post.ClubMember.User.UserId,
                    Username = post.ClubMember.User.Username,
                    Email = post.ClubMember.User.Email
                }
              
            }).ToList();


            return new ClubDetailsViewDto
            {
                ClubId = club.ClubId,
                ClubName = club.ClubName,
                LogoBase64 = _imageHelperService.ConvertToBase64(club.Logo, "png"),
                CoverBase64 = _imageHelperService.ConvertToBase64(club.Cover, "png"),
                Description = club.Description,
                ClubMembers = clubMemberDtos,
                Posts = postDtos,

                TotalPosts = totalPosts,
                PostNumber = postNumber,
                PostSize = postSize
            };
        }
        public async Task AddClubAsync(Club club)
        {
            await _clubRepository.AddClubAsync(club);
        }

        public async Task<IEnumerable<Club>> GetAllClubsAsync()
        {
            return await _clubRepository.GetAllClubAsync();
        } 
        public async Task<IEnumerable<Club>> GetAllClubsApprovedAsync()
        {
            var club = await _clubRepository.GetAllClubAsync();
            return club.Where(p => p.Status == true);
        }

        public async Task<(bool success, string message)> UpdateClubAsync(ClubEditDto clubEditDto)
        {
            var clubs = await _clubRepository.GetAllClubAsync();
            var clubCheck = clubs.FirstOrDefault(m => m.ClubName.Equals(clubEditDto.ClubName) && m.ClubId != clubEditDto.ClubId);
            if (clubCheck != null)
            {
                return (false, "This club name already exist!");
            }
            var club = clubs.FirstOrDefault(m => m.ClubId == clubEditDto.ClubId);
            if(!String.IsNullOrEmpty(clubEditDto.ClubName) && !String.IsNullOrEmpty(clubEditDto.Description))
            {
                club.ClubName = clubEditDto.ClubName;
                club.Description = clubEditDto.Description;
            }
            if (clubEditDto.Logo != null)
            {
                club.Logo = clubEditDto.Logo;
            }
            if (clubEditDto.Cover != null)
            {
                club.Cover = clubEditDto.Cover;
            }
            await _clubRepository.UpdateClubAsync(club);
            return (true, "Club update successfully!");
        }

        public async Task<(bool success, string message)> DeleteClub(Club club)
        {
            await _clubRepository.UpdateClubAsync(club);
            var clubmembers = await _clubMemberService.GetAllClubMembersByClubIdWithAnyStatusAsync(club.ClubId);
            foreach(var member in clubmembers)
            {
                member.Status = false;
                await _clubMemberService.UpdateClubMemberAsync(member);
            }
            return (true, "Club delete successfully!");
        }

        public async Task<Club> GetClubByClubIdAsync(int clubId)
        {
            return await _clubRepository.GetClubByClubIdAsync(clubId);
        }

        public async Task<Club> GetClubAsync(int id)
        {
            return (await _clubRepository.GetAllClubAsync()).FirstOrDefault(c => c.ClubId == id);
        }

        public async Task<Club> CheckClubName(string clubName)
        {
            return (await _clubRepository.GetAllClubAsync()).FirstOrDefault(c => c.ClubName == clubName);
        }
    }
}