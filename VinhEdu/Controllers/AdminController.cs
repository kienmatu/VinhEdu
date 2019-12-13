﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VinhEdu.Models;
using VinhEdu.Repository;
using VinhEdu.ViewModels;
using static VinhEdu.Models.AdditionalDefinition;

namespace VinhEdu.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        // GET: Admin
        UnitOfWork db = new UnitOfWork();
        EduVinhContext Context = new EduVinhContext();
        public ActionResult Index()
        {
            int configID = (int)Session["ConfigID"];
            int UserID = (int)Session["UserID"];
            ViewBag.countSchool = db.SchoolRepository.GetAll().Count();
            ViewBag.countClass = db.ClassRepository.GetAll().Count();
            ViewBag.countStudent = db.UserRepository.AllUser()
                .Where(c => c.Type == UserType.Student)
                .Count();
            ViewBag.countTeacher = db.UserRepository.AllUser()
                .Where(c => c.Type == UserType.Teacher)
                .Count();
            return View();
        }
        public ActionResult CreateStudent()
        {
            List<School> lst = db.SchoolRepository.GetAll().ToList();
            List<Configure> configures = db.ConfigRepository.GetAll().ToList();
            ViewBag.Config = new SelectList(configures, "ID", "SchoolYear");
            ViewBag.SchoolList = new SelectList(lst, "SchoolID", "SchoolName");
            return View();
        }
        public ViewResult AllStudent()
        {
            List<School> lst = db.SchoolRepository.GetAll().ToList();
            List<Configure> configures = db.ConfigRepository.GetAll().OrderByDescending(e => e.IsActive).ToList();
            int SchoolId = lst.First().SchoolID;
            List<Class> classes = db.ClassRepository.GetAll().Where(e => e.SchoolID == SchoolId).ToList();
            ViewBag.Class = new SelectList(classes, "ClassID", "ClassName");
            ViewBag.Config = new SelectList(configures, "ID", "SchoolYear");
            ViewBag.SchoolList = new SelectList(lst, "SchoolID", "SchoolName");
            return View();
        }
        public ViewResult CreateTeacher()
        {
            TeacherViewModel model = new TeacherViewModel
            {
                Subject = db.SubjectRepository.GetAll().Select(m =>
                    new SelectListItem
                    {
                        Text = m.SubjectName,
                        Value = m.ID.ToString(),
                    }
                    ).ToList(),
            };
            List<School> lst = db.SchoolRepository.GetAll().ToList();
            List<Configure> configures = db.ConfigRepository.GetAll().ToList();
            ViewBag.Config = new SelectList(configures, "ID", "SchoolYear");
            ViewBag.SchoolList = new SelectList(lst, "SchoolID", "SchoolName");
            return View(model);
        }
        public ViewResult AllTeacher()
        {
            List<School> lst = db.SchoolRepository.GetAll().ToList();
            List<Configure> configures = db.ConfigRepository.GetAll().OrderByDescending(e => e.IsActive).ToList();
            int SchoolId = lst.First().SchoolID;
            List<Class> classes = new List<Class>();
            classes.Add(new Class
            {
                ClassID = 0,
                ClassName = "Tất cả"
            });
            classes.AddRange(db.ClassRepository.GetAll().Where(e => e.SchoolID == SchoolId).ToList());
            ViewBag.Class = new SelectList(classes, "ClassID", "ClassName");
            ViewBag.Config = new SelectList(configures, "ID", "SchoolYear");
            ViewBag.SchoolList = new SelectList(lst, "SchoolID", "SchoolName");
            return View();
        }
        public ViewResult TeacherClass()
        {
            List<School> lst = db.SchoolRepository.GetAll().ToList();
            List<Configure> configures = db.ConfigRepository.GetAll().OrderByDescending(e => e.IsActive).ToList();
            int SchoolId = lst.First().SchoolID;
            List<Class> classes = db.ClassRepository.GetAll().Where(e => e.SchoolID == SchoolId).ToList();
            //List<SubjectList> subjects = db.SubjectRepository.GetAll()
            //    .Select(c => new SubjectList
            //    { 
            //        SubjectID = c.ID,
            //        SubjectName = c.SubjectName,
            //    }).ToList();
            ViewBag.Class = new SelectList(classes, "ClassID", "ClassName");
            ViewBag.Config = new SelectList(configures, "ID", "SchoolYear");
            ViewBag.SchoolList = new SelectList(lst, "SchoolID", "SchoolName");
            //ViewBag.Subjects = subjects;
            return View();
        }
        [HttpGet]
        public ViewResult Setting()
        {
            List<Configure> configures = db.ConfigRepository.GetAll().OrderByDescending(e => e.IsActive).ToList();
            ViewBag.Config = new SelectList(configures, "ID", "SchoolYear");
            Setting setting = Context.Settings.FirstOrDefault();
            return View(setting);
        }
        [HttpPost]
        public JsonResult Setting(string org,Semester semester, int ConfigID)
        {
            
            Setting setting = Context.Settings.FirstOrDefault();
            setting.OrganizationName = org;
            setting.Semester = semester;
            List<Configure> configure = Context.Configures.ToList();
            configure.ForEach((e) =>
            {
                if (e.IsActive)
                {
                    e.IsActive = false;
                }
                if (e.ID == ConfigID)
                {
                    e.IsActive = true;
                }
            });
            Context.SaveChanges();
            return Json("Cập nhật thành công");
        }
    }

}