using AutoMapper;
using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserController(UserManager<ApplicationUser> userManager , IMapper mapper)
        {
			_userManager = userManager;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(string Email)
		{
			if(string.IsNullOrEmpty(Email))
			{
				var Users= await _userManager.Users.Select(U => new UserViewModel()
				{
					Id=U.Id,
					Fname=U.FName,
					LName=U.LName,
					Email=U.Email,
					PhoneNumber=U.PhoneNumber,
					Roles= _userManager.GetRolesAsync(U).Result

				}).ToListAsync();
				return View(Users);
			}
			else
			{
				var User = await _userManager.FindByEmailAsync(Email);
				if(User is null)
				{
					return View(Enumerable.Empty<UserViewModel>());
				}
				var MappedUser = new UserViewModel()
				{
					Id = User.Id,
					Fname = User.FName,
					LName = User.LName,
					Email = User.Email,
					PhoneNumber = User.PhoneNumber,
					Roles = _userManager.GetRolesAsync(User).Result
				};
				return View(new List<UserViewModel> { MappedUser });
			}	
		}
		public async Task<IActionResult> Details(string id,string ViewName="Details")
		{
			if (id is null)
				return BadRequest();
			var user = await _userManager.FindByIdAsync(id);
			if (user is null)
				return NotFound();
			var MappedUser = _mapper.Map<ApplicationUser, UserViewModel>(user);
			return View(ViewName,MappedUser);
		}
		public async Task<IActionResult> Edit(string id)
		{
			return await Details(id, "Edit");
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit([FromRoute]string id,UserViewModel model)
		{
			if(id!=model.Id)
				return BadRequest();
			if(ModelState.IsValid)
			{
				try
				{
					var user =await _userManager.FindByIdAsync(id);
					user.FName = model.Fname;
					user.LName=model.LName;
					user.PhoneNumber = model.PhoneNumber;
					await _userManager.UpdateAsync(user);
					return RedirectToAction(nameof(Index));

				}
				catch (Exception ex)
				{
					ModelState.AddModelError(string.Empty,ex.Message);
					return RedirectToAction(nameof(HomeController.Error), "Home");
				}
			}
			return View(model);
		}
		public async Task<IActionResult> Delete(string id)
		{
			return await Details(id,"Delete");
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ConfirmDelete(string id)
		{
			try
			{
				var user = await _userManager.FindByIdAsync(id);
				await _userManager.DeleteAsync(user);
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, ex.Message);
				return RedirectToAction(nameof(HomeController.Error), "Home");
			}
			
           
		}

    }
}
