# MiniSocial — Sequence & Class Diagrams

PlantUML diagrams for the Group / Post / Search / Admin backend and the
frontend Localization & Theme features. Style follows the provided samples
(white background, black borders, Arial bold; numbered messages with
`alt`/`opt` validation blocks). Sequence diagrams reflect the **full backend
validation** implemented in each Service (BLL).

Layering: `Razor View → Web MVC Controller → ApiClient →(HTTP)→ API Controller → Service (BLL) → Repository (DAL) → AppDbContext → SQL Server`.

## How to render

```bash
# requires Java + graphviz (dot)
java -jar plantuml.jar -tpng "class/*.puml"    -o rendered
java -jar plantuml.jar -tpng "sequence/*.puml" -o rendered
# or SVG:
java -jar plantuml.jar -tsvg "**/*.puml" -o rendered
```

## Class diagrams (`class/`)

| File | Covers |
|------|--------|
| `class_group.puml` | GroupService (BLL), GroupRepository (DAL), GroupsController, entities Group/GroupMember, enum GroupRole, DTOs |
| `class_post.puml` | PostService/GroupPosts, PostRepository, IContentFilter, Post entity, post DTOs |
| `class_search.puml` | SearchService, SearchRepository, search DTOs (User/Group/Post) |
| `class_admin.puml` | AdminService, AdminRepository, dashboard stats / posts-per-day / users DTOs |
| `class_frontend_theme_localization.puml` | LangController, Loc (resource store), _Layout switcher, site.js theme, CSS variables |

## Sequence diagrams (`sequence/`)

| File | Feature(s) |
|------|-----------|
| `seq_group_create.puml` | Tạo nhóm mới (form + service) |
| `seq_group_edit.puml` | Sửa nhóm (Owner) |
| `seq_group_delete.puml` | Xoá nhóm (Owner, soft delete) |
| `seq_group_list.puml` | Trang danh sách nhóm `/groups` |
| `seq_group_details.puml` | Trang chi tiết nhóm `/group/{id}` + feed bài viết trong nhóm |
| `seq_group_join_leave.puml` | Join / Leave nhóm |
| `seq_group_post_create.puml` | Đăng bài trong nhóm (Group Posts) |
| `seq_group_member_manage.puml` | Quản lý thành viên (Owner: kick, đổi role) |
| `seq_search.puml` | Search Group / Người dùng / Bài viết |
| `seq_admin_dashboard.puml` | Trang Admin `/admin`: Total Users/Posts/Comments/Groups + Posts-per-day (chart.js) + Users |
| `seq_admin_delete_user.puml` | Quản lý Users — xem/xoá user (Admin) |
| `seq_theme_toggle.puml` | Toggle dark/light, CSS variables, lưu localStorage |
| `seq_language_switch.puml` | Cấu hình Localization VI/EN, resource store, language switcher |

Rendered PNGs are under `rendered/` next to each `.puml` set.

## Note on ASP.NET Localization

The codebase implements VI/EN via a lightweight `Loc` static class driven by a
`lang` cookie. Its in-memory `Map` (key → (Vi, En)) plays the role of the
requested `Home.vi.resx` / `Home.en.resx` resource files; the language switcher
lives in `_Layout.cshtml`. Diagrams reflect this actual implementation.
