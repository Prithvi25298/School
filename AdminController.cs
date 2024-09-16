using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Newtonsoft.Json;
using SchoolPortalV2.Models;
using SchoolPortalV2.Operations;

namespace SchoolPortalV2.Controllers
{
    public class AdminController : Controller
    {
        DbOperations opretions = new DbOperations();
        // GET: Admin
        public ActionResult Dashboard()
        {
            return View();
        }

        #region Users
        public ActionResult Users()
        {
            UserList users = new UserList();
            try
            {
                users.dsUsers = opretions.GetList("_Get_Teacher_List");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(users);
        }
        [HttpPost]
        public ActionResult SaveUsers(User_Basic_Details ubd)
        {
            string Status = "";
            string msg = "";
            string outputMsg = "";
            string viewContentHtml = "";
            try
            {
                Int64 IsError = opretions.SaveModelData(ubd, "Save_User_Basic_Details", out outputMsg);
                if (IsError > 0)
                {
                    Status = "SUCCESS";
                    if (ubd.usr_id > 0) { msg = "updated"; }
                    else { msg = "added"; }
                }
                else if (IsError == -1)
                {
                    Status = "DUPLICATE";
                    msg = "duplicate";
                }
                else
                {
                    Status = "ERROR";
                }
                DataSet ds = opretions.GetList("_Get_Teacher_List");
                viewContentHtml = opretions.RenderViewToString("Users_Partial", ds, ControllerContext);
                return Json(new
                {
                    Status = Status,
                    Message = msg,
                    ViewContentHtml = viewContentHtml
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = "ERROR",
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        public ActionResult EditUsers(string id)
        {
            string Status = "";
            string msg = "";
            string entityTeacher = "";
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@usrId", id));
                DataTable dt = opretions.GetDetailsForEdit("_Get_Teacher_List", parameters);
                if (dt.Rows.Count > 0)
                {
                    Status = "SUCCESS";
                    entityTeacher = JsonConvert.SerializeObject(dt);
                }
                else
                {
                    Status = "ERROR";
                }
                return Json(new
                {
                    Status = Status,
                    Message = msg,
                    entity = entityTeacher
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = "ERROR",
                    Message = ex.Message
                });
            }
        }
        public ActionResult UsersAllDetails(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        [HttpPost]
        public ActionResult SaveUserDetails(User_Details ud)
        {
            string Status = "";
            string msg = "";
            string outputMsg = "";
            string viewContentHtml = "";
            try
            {
                Int64 IsError = opretions.SaveModelData(ud, "_Save_User_Detail", out outputMsg);
                if (IsError > 0)
                {
                    Status = "SUCCESS";
                    if (ud.usr_id > 0) { msg = "updated"; }
                    else { msg = "added"; }
                }
                else if (IsError == -1)
                {
                    Status = "DUPLICATE";
                    msg = "duplicate";
                }
                else
                {
                    Status = "ERROR";
                }
                DataSet ds = opretions.GetList("_Get_Teacher_List");
                viewContentHtml = opretions.RenderViewToString("Users_Partial", ds, ControllerContext);
                return Json(new
                {
                    Status = Status,
                    Message = msg,
                    ViewContentHtml = viewContentHtml
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = "ERROR",
                    Message = ex.Message
                });
            }
        }

        public ActionResult UsersDetails() { return View(); }

        #endregion

        #region Student
        public ActionResult Student()
        {
            StudentList student = new StudentList();
            try
            {
                student.dsStudent = opretions.GetList("_Get_Student_List");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(student);
        }
        public ActionResult StudentAllDetails(int id)
        {
            StudentList student = new StudentList();
            try
            {
                student.dsStudent = opretions.GetList("_Get_Student_List");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(student);
        }

        [HttpPost]
        public ActionResult SaveStudent(Student_Basic_Details std)
        {
            string Status = "";
            string msg = "";
            string outputMsg = "";
            string viewContentHtml = "";
            try
            {
                Int64 IsError = opretions.SaveModelData(std, "Save_Student_Basic_Details", out outputMsg);
                if (IsError > 0)
                {
                    Status = "SUCCESS";
                    if (std.std_id > 0) { msg = "updated"; }
                    else { msg = "added"; }
                }
                else if (IsError == -1)
                {
                    Status = "DUPLICATE";
                    msg = "duplicate";
                }
                else
                {
                    Status = "ERROR";
                }
                DataSet ds = opretions.GetList("_Get_Student_List");
                viewContentHtml = opretions.RenderViewToString("Student_Partial", ds, ControllerContext);
                return Json(new
                {
                    Status = Status,
                    Message = msg,
                    ViewContentHtml = viewContentHtml
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = "ERROR",
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        public ActionResult EditStudent(int std_id)
        {
            string Status = "";
            string msg = "";
            string entityStudent = "";
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@std_id", std_id));
                DataTable dtStudent = opretions.GetDetailsForEdit("_Get_Student_Details", parameters);
                if (dtStudent.Rows.Count > 0)
                {
                    Status = "SUCCESS";
                    entityStudent = JsonConvert.SerializeObject(dtStudent);
                }
                else
                {
                    Status = "ERROR";
                }
                return Json(new
                {
                    Status = Status,
                    Message = msg,
                    entity = entityStudent
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = "ERROR",
                    Message = ex.Message
                });
            }
        }
        #endregion

        #region Attendance
        public ActionResult Attendance()
        {
            return View();
        }

        #endregion
    }
}