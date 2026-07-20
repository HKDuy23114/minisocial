using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniSocialNetwork.Web.Models;
using MiniSocialNetwork.Web.Services;

namespace MiniSocialNetwork.Web.Controllers;

public sealed class GroupsController : Controller
{
    private readonly ApiClient _api;
    public GroupsController(ApiClient api) => _api = api;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vm = new GroupsIndexViewModel();
        try { vm.AllGroups = await _api.GetGroupsAsync(); } catch (ApiException) { }
        if (User.Identity?.IsAuthenticated == true)
        {
            try { vm.MyGroups = await _api.GetMyGroupsAsync(); } catch (ApiException) { }
        }
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var group = await _api.GetGroupAsync(id);
        if (group == null) return NotFound();
        var uid = User.UserId();
        var vm = new GroupDetailsViewModel
        {
            Group = group,
            IsOwner = uid != null && group.OwnerId == uid,
            IsMember = uid != null && group.Members.Any(m => m.UserId == uid)
        };
        try { vm.Feed = await _api.GetGroupFeedAsync(id, 1, 30); } catch (ApiException) { }
        return View(vm);
    }

    [Authorize, HttpGet]
    public IActionResult Create() => View(new CreateGroupRequest());

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateGroupRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name))
        {
            ModelState.AddModelError(nameof(req.Name), "Name is required.");
            return View(req);
        }
        try
        {
            var id = await _api.CreateGroupAsync(req);
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(req);
        }
    }

    [Authorize, HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var group = await _api.GetGroupAsync(id);
        if (group == null) return NotFound();
        return View(new CreateGroupRequest { Name = group.Name, Description = group.Description ?? "", AvatarUrl = group.AvatarUrl });
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CreateGroupRequest req)
    {
        try { await _api.UpdateGroupAsync(id, req); return RedirectToAction(nameof(Details), new { id }); }
        catch (ApiException ex) { ModelState.AddModelError(string.Empty, ex.Message); return View(req); }
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try { await _api.DeleteGroupAsync(id); TempData["Info"] = "Group deleted."; }
        catch (ApiException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Join(Guid id)
    {
        try { await _api.JoinGroupAsync(id); } catch (ApiException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Leave(Guid id)
    {
        try { await _api.LeaveGroupAsync(id); } catch (ApiException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Kick(Guid id, string userId)
    {
        try { await _api.KickMemberAsync(id, userId); } catch (ApiException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(Guid id, string userId, int role)
    {
        try { await _api.ChangeRoleAsync(id, userId, role); } catch (ApiException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Details), new { id });
    }
}
