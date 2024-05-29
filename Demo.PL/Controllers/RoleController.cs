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
    [Authorize(Roles ="Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public RoleController(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager,IMapper mapper)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(string searchByName)
        {
            if(string.IsNullOrEmpty(searchByName)) 
            {
                var Roles = await _roleManager.Roles.ToListAsync(); 
                var MappedRole = _mapper.Map<IEnumerable<IdentityRole>,IEnumerable<RoleViewModel>>(Roles);
                return View(MappedRole);
            }
            else
            {
                var Role =await _roleManager.FindByNameAsync(searchByName);
                if (Role is null)
                    return View(Enumerable.Empty<RoleViewModel>());
                var MappedRole = _mapper.Map < IdentityRole, RoleViewModel>(Role);
                return View(new List<RoleViewModel> { MappedRole});
            }
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Role = _mapper.Map<RoleViewModel,IdentityRole>(model);
                await _roleManager.CreateAsync(Role);
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public async Task<IActionResult> Details(string id, string ViewName = "Details")
        {
            if (id is null)
                return BadRequest();
            var Role = await _roleManager.FindByIdAsync(id);
            if (Role is null)
                return NotFound();
            var MappedRole = _mapper.Map<IdentityRole, RoleViewModel>(Role);
            return View(ViewName, MappedRole);
        }
        public async Task<IActionResult> Edit(string id)
        {
            return await Details(id, "Edit");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id, RoleViewModel model)
        {
            if (id != model.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    var Role = await _roleManager.FindByIdAsync(id);
                    Role.Name = model.Name;
                    await _roleManager.UpdateAsync(Role);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return RedirectToAction(nameof(HomeController.Error), "Home");
                }
            }
            return View(model);
        }
        public async Task<IActionResult> Delete(string id)
        {
            return await Details(id, "Delete");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(string id)
        {
            try
            {
                var Role = await _roleManager.FindByIdAsync(id);
                await _roleManager.DeleteAsync(Role);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }


        }
        public async Task<IActionResult> AddOrRemoveUserRole(string roleId)
        {
            var role =await _roleManager.FindByIdAsync(roleId);
            if (role is null)
               return NotFound();
            ViewBag.RoleId = roleId;
            var usersInRole = new List<UserInRoleViewModel>();
            var Users = await _userManager.Users.ToListAsync();
            foreach (var user in Users)
            {
                var userInRole = new UserInRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };
                if(await _userManager.IsInRoleAsync(user,role.Name))
                    userInRole.IsSelected = true;
                else 
                    userInRole.IsSelected = false;
                usersInRole.Add(userInRole);
            }
            return View(usersInRole);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUserRole(string roleId,List<UserInRoleViewModel> usersInRole)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null)
                return NotFound();
            if(ModelState.IsValid)
            {

                foreach (var userInRole in usersInRole)
                {
                    var appUser = await _userManager.FindByIdAsync(userInRole.UserId);
                    if(appUser is not null)
                    {
                        if (userInRole.IsSelected && !await _userManager.IsInRoleAsync(appUser, role.Name))
                            await _userManager.AddToRoleAsync(appUser, role.Name);
                        else if (!userInRole.IsSelected && await _userManager.IsInRoleAsync(appUser, role.Name))
                            await _userManager.RemoveFromRoleAsync(appUser, role.Name);
                    }
                }
                return RedirectToAction("Edit",new { id = roleId });
            }
            return View(usersInRole);
        }
    }
}
