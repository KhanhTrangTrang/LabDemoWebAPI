using DemoLabModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LabDemoWebASPMVC.Controllers
{
    /// <summary>
    ///   Controller hiển thị mà hình quản lí nhân viên, hiển thị thông tin, thêm, sửa xóa nhân viên.
    /// </summary>
    /// <Modified>
    /// Name Date Comments
    /// khanhnn 5/24/2021 created
    /// </Modified>
    [Authorize]
    public class UsersController : Controller
    {
        HttpClient client = new HttpClient();
        /// <summary>Hiển thị tất cả các user có trong database</summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString">Tìm kiếm nhân viên</param>
        /// <param name="pageNumber">Chỉ số trang</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <Modified>
        /// Name Date Comments
        /// khanhnn 5/24/2021 created
        /// </Modified>
        public async Task<IActionResult> Index(string sortOrder,
                                               string currentFilter,
                                               string searchString,
                                               int? pageNumber)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("https://localhost:44325/api/employees");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                IEnumerable<Employee> myList = JsonConvert.DeserializeObject<List<Employee>>(responseBody);
                ViewData["CurrentSort"] = sortOrder;
                ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

                if (searchString != null)
                {
                    pageNumber = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewData["CurrentFilter"] = searchString;

                if (!String.IsNullOrEmpty(searchString))
                {
                    myList = myList.Where(s => s.Name.Contains(searchString));
                }
                int pageSize = 3;
                return View(PaginatedList<Employee>.CreateAsync(myList, pageNumber ?? 1, pageSize));
            }
            catch (HttpRequestException e)
            {
                IEnumerable<Employee> students = new List<Employee>();
                int pageSize = 3;
                return View(PaginatedList<Employee>.CreateAsync(students, pageNumber ?? 1, pageSize));
            }
        }

        // Get
        /// <summary>Chọn nút thêm nhân viên</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <Modified>
        /// Name Date Comments
        /// khanhnn 5/24/2021 created
        /// </Modified>
        public IActionResult AddUser()
        {
            TempData["Case"] = "add";
            return RedirectToAction("Confirm", new { Id = 0 });
        }

        /// <summary>Chọn nút xóa nhân viên</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <Modified>
        /// Name Date Comments
        /// khanhnn 5/24/2021 created
        /// </Modified>
        public IActionResult DeleteUser(int id)
        {
            TempData["Case"] = "delete";
            return RedirectToAction("Confirm", new { Id = id });
        }

        // Get
        /// <summary>Chọn nút sửa thông tin nhân viên</summary>
        /// <param name="user">The user.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <Modified>
        /// Name Date Comments
        /// khanhnn 5/24/2021 created 
        /// </Modified>
        public async Task<IActionResult> EditUser(Employee user)
        {
            JsonContent content = JsonContent.Create(user);

            HttpResponseMessage response = await client.PutAsync("https://localhost:44325/api/employees", content);
            // Case change email
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                TempData["edit"] = "Email đã được sử dụng";
                return RedirectToAction("DetailUser", new { Id = user.Id });
            }
            TempData["Case"] = "edit";
            TempData["EditName"] = user.Name;
            TempData["EditEmail"] = user.Email;
            TempData["EditTel"] = user.Tel;
            return RedirectToAction("Confirm", new { Id = user.Id });
        }

        // Post
        /// <summary>Gửi đi hành động thêm nhân viên</summary>
        /// <param name="user">The user.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <Modified>
        /// Name Date Comments
        /// khanhnn 5/24/2021 created
        /// </Modified>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserPostAsync(Employee user)
        {
            JsonContent content = JsonContent.Create(user);

            HttpResponseMessage response = await client.PostAsync("https://localhost:44325/api/employees", content);
            // Case change email
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                TempData["AddError"] = "Email đã được sử dụng";
                TempData["AddName"] = user.Name;
                TempData["AddEmail"] = user.Email;
                TempData["AddTel"] = user.Tel;
                return RedirectToAction("AddUser");
            }
            TempData["Result"] = "Đăng kí thành công";
            return RedirectToAction("Result");
        }

        // Post
        /// <summary>Gửi đi hành động sửa nhân viên</summary>
        /// <param name="user">The user.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <Modified>
        /// Name Date Comments
        /// khanhnn 5/24/2021 created
        /// </Modified>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserPostAsync(Employee user)
        {
            JsonContent content = JsonContent.Create(user);
            HttpResponseMessage response = await client.PutAsync(string.Format("https://localhost:44325/api/employees/{0}", user.Id), content);
            TempData["Result"] = "Updat thông tin thành công";
            return RedirectToAction("Result");
        }

        /// <summary>Gửi đi hành động xóa nhân viên</summary>
        /// <param name="user">The user.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <Modified>
        /// Name Date Comments
        /// khanhnn 5/24/2021 created
        /// </Modified>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserPostAsync(Employee user)
        {
            JsonContent content = JsonContent.Create(user);
            HttpResponseMessage response = await client.DeleteAsync(string.Format("https://localhost:44325/api/employees/{0}", user.Id));
            TempData["Result"] = "Delete thông tin thành công";
            return RedirectToAction("Result");
        }

        // Get
        /// <summary>Hiển thị thông tin nhân viên</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <Modified>
        /// Name Date Comments
        /// khanhnn 5/24/2021 created
        /// </Modified>
        public async Task<IActionResult> DetailUserAsync(int id)
        {
            if (TempData["edit"] != null)
            {
                ViewBag.Message = TempData["edit"].ToString();
                TempData.Remove("edit");
            }
            HttpResponseMessage response = await client.GetAsync(string.Format("https://localhost:44325/api/employees/{0}", id));
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Employee>(responseBody);
            return View(result);
        }

        /// <summary>Xác nhận kết quả cho việc thêm, sửa xóa nhân viên</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <Modified>
        /// Name Date Comments
        /// khanhnn 5/24/2021 created
        /// </Modified>
        public IActionResult Result()
        {
            if (TempData["Result"] != null)
            {
                ViewBag.Result = TempData["Result"].ToString();
                TempData.Remove("Result");
            }
            return View();
        }

        /// <summary>Confirms the specified identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        ///   Xác nhận thông tin thêm, sửa, xóa, cảnh báo lỗi khi thông tin không hợp lệ
        /// </returns>
        /// <Modified>
        /// Name Date Comments
        /// khanhnn 5/24/2021 created
        /// </Modified>
        public async Task<IActionResult> Confirm(int id)
        {
            if (TempData["Case"] != null)
            {
                // Lưu thông tin trường hợp sửa thông tin nhân viên không hợp lệ
                if (TempData["Case"].ToString() == "add")
                {
                    if (TempData["AddError"] != null)
                    {
                        ViewBag.AddError = TempData["AddError"].ToString();
                        TempData.Remove("AddError");
                    }
                    else
                    {
                        ViewBag.AddError = "";
                    }
                    // Save input fields in case add user fail 
                    if (TempData["AddName"] != null)
                    {
                        ViewBag.AddName = TempData["AddName"].ToString();
                        TempData.Remove("AddName");
                    }
                    else
                    {
                        ViewBag.AddName = "";
                    }
                    if (TempData["AddEmail"] != null)
                    {
                        ViewBag.AddEmail = TempData["AddEmail"].ToString();
                        TempData.Remove("AddEmail");
                    }
                    else
                    {
                        ViewBag.AddEmail = "";
                    }
                    if (TempData["AddTel"] != null)
                    {
                        ViewBag.AddTel = TempData["AddTel"].ToString();
                        TempData.Remove("AddTel");
                    }
                    else
                    {
                        ViewBag.AddTel = "";
                    }
                }
                ViewBag.Case = TempData["Case"].ToString();
                TempData.Remove("Case");
            }
            try
            {
                //var result = _db.Users.SingleOrDefault(b => b.Id == id);
                HttpResponseMessage response = await client.GetAsync(string.Format("https://localhost:44325/api/employees/{0}", id));
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Employee>(responseBody);
                // Lưu thông tin nhập vào trường hợp thông tin user thêm vào không hợp lệ
                if (TempData["EditName"] != null)
                {
                    result.Name = TempData["EditName"].ToString();
                    TempData.Remove("EditName");
                }
                if (TempData["EditEmail"] != null)
                {
                    result.Email = TempData["EditEmail"].ToString();
                    TempData.Remove("EditEmail");
                }
                if (TempData["EditTel"] != null)
                {
                    result.Tel = TempData["EditTel"].ToString();
                    TempData.Remove("EditTel");
                }
                return View(result);
            }
            catch (HttpRequestException e)
            {
                return View(new Employee());
            }

        }

        /// <summary>Quay trở lại màn hình hiển thị thông tin nhân viên</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <Modified>
        /// Name Date Comments
        /// khanhnn 5/24/2021 created
        /// </Modified>
        public IActionResult Back(int id)
        {
            return RedirectToAction("DetailUser", new { Id = id });
        }


    }
}
