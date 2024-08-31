using BookStore.Controllers;
using BookStore.Models;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Metadata;
using System.Reflection;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookStore.Controllers
{
    [Authorize(Roles = "Admin")]
    
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<DefaultUser> _userManager;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<DefaultUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult ListAllRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(AddRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new()
                {
                    Name = model.RoleName
                };

                var result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListAllRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewData["ErrorMessage"] = $"No role with Id '{id}' was found";
                return View("Error");
            }

            EditRoleViewModel model = new()
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewData["ErrorMessage"] = $"No role with Id '{model.Id}' was found";
                return View("Error");
            }
            else
            {
                role.Name = model.RoleName;

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListAllRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            ViewData["roleId"] = id;
            ViewData["roleName"] = role.Name;

            if (role == null)
            {
                ViewData["ErrorMessage"] = $"No role with Id '{id}' was found";
                return View("Error");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in _userManager.Users)
            {
                UserRoleViewModel userRoleVM = new()
                {
                    Id = user.Id,
                    Name = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleVM.IsSelected = true;
                }
                else
                {
                    userRoleVM.IsSelected = false;
                }

                model.Add(userRoleVM);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewData["ErrorMessage"] = $"No role with Id '{id}' was found";
                return View("Error");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].Id);

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
            }

            return RedirectToAction("EditRole", new { Id = id });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDelete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewData["ErrorMessage"] = $"No role with Id '{id}' was found";
                return View("Error");
            }
            else
            {
                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListAllRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(role);
            }
        }
    }
}




//# AdminController Code Explanation

//## Overview
//The controller manages user roles in the application, providing functionality for listing, adding, editing, and deleting roles, as well as managing users within roles.

//## Key Components

//1. **Dependencies and Namespace**
//   - The controller uses models from `BookStore.Models` and view models from `BookStore.ViewModels`.
//   - It utilizes ASP.NET Core Identity (`Microsoft.AspNetCore.Identity`) for user and role management.

//2. **Controller Class**
//   - The class `AdminController` inherits from `Controller`.
//   - It's intended to be accessible only by users with the "Admin" role (currently commented out).

//3. **Dependency Injection**
//   - The controller uses constructor injection to get instances of `RoleManager<IdentityRole>` and `UserManager<DefaultUser>`.

//4. **Role Management Actions**
//   - `ListAllRoles`: Displays all roles.
//   - `AddRole`: Allows creating a new role.
//   - `EditRole`: Enables editing an existing role and viewing users in that role.
//   - `DeleteRole` and `ConfirmDelete`: Handle the role deletion process.

//5. **User-Role Management**
//   - `EditUsersInRole`: Allows adding or removing users from a specific role.

//6. **View Models**
//   - The controller uses several view models (e.g., `AddRoleViewModel`, `EditRoleViewModel`, `UserRoleViewModel`) to pass data between the controller and views.

//7. **Asynchronous Operations**
//   - Most methods are asynchronous, using `async / await` for database operations.

//8. * *Error Handling * *
//   -The controller includes error checking and handling, such as checking if a role exists before operations.

//## Key Functionalities

//1. * *Role Listing * *: Displays all roles in the system.
//2. **Role Creation**: Allows adding new roles to the system.
//3. * *Role Editing * *: Enables changing role names and viewing users in a role.
//4. **User-Role Assignment**: Provides functionality to add or remove users from specific roles.
//5. **Role Deletion**: Allows removing roles from the system.

//## Security Considerations
//- The `[Authorize(Roles = "Admin")]` attribute is commented out, which should be addressed in a production environment to ensure only admins can access these functions.

//## Best Practices Observed
//- Use of async methods for database operations.
//- Separation of concerns using view models.
//-Proper error handling and model state validation.
//- Redirects after successful operations to prevent form resubmission.

//## Potential Improvements
//1. Implement logging for important actions.
//2. Add more robust error handling and user feedback.
//3. Consider implementing a service layer to separate business logic from the controller.
//4. Ensure the `[Authorize]` attribute is properly implemented for security.

//This controller provides a comprehensive set of tools for managing roles and user - role assignments in an ASP.NET Core application, suitable for administrative purposes in a web application like a bookstore management system.