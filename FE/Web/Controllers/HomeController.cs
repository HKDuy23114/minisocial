using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniSocialNetwork.Web.Models;
using MiniSocialNetwork.Web.Services;

namespace MiniSocialNetwork.Web.Controllers;

public sealed class HomeController : Controller
{
    private readonly ApiClient _api;
    public HomeController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index(int page = 1)
    {
        var vm = new FeedViewModel { Page = page < 1 ? 1 : page };
        vm.Feed = await _api.GetFeedAsync(vm.Page, 10);

        if (User.Identity?.IsAuthenticated == true)
        {
            try { vm.MyGroups = await _api.GetMyGroupsAsync(); } catch (ApiException) { }
            var myIds = vm.MyGroups.Select(g => g.Id).ToHashSet();
            try
            {
                var all = await _api.GetGroupsAsync();
                vm.SuggestedGroups = all.Where(g => !myIds.Contains(g.Id)).Take(5).ToList();
            }
            catch (ApiException) { }
        }
        else
        {
            try { vm.SuggestedGroups = (await _api.GetGroupsAsync()).Take(5).ToList(); } catch (ApiException) { }
        }

        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
