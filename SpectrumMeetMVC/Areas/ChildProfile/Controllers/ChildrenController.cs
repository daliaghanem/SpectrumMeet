﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using SpectrumMeetEF;

namespace SpectrumMeetMVC.Areas.ChildProfile.Controllers
{
    public class ChildrenController : Controller
    {
        private SpectrumMeetEntities db = new SpectrumMeetEntities();

        // GET: ChildProfile/Children
        public ActionResult Index()
        {
            var children = db.Children.Include(c => c.SupportLevel);
            return View(children.ToList());
        }

        // GET: ChildProfile/Children/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Child child = db.Children.Find(id);
            if (child == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ChildName = child.Name;
            //ViewBag.ChildBirthDate = child.BirthDate;
            //ViewBag.ChildVerbal = child.Verbal;
            //ViewBag.ChildDescription = child.Description;
            //ViewBag.LevelName = db.SupportLevels.FirstOrDefault(s => s.LevelID == child.LevelID)?.Name;
            return View(child);
        }

        // GET: ChildProfile/Children/Create
        public ActionResult Create(int? accountId)
        {

            if (accountId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            ViewBag.AccountID = accountId;
            ViewBag.LevelID = new SelectList(db.SupportLevels, "LevelID", "Name");
            return View();
        }

        // POST: ChildProfile/Children/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,BirthDate,Verbal,Description,LevelID")] Child child, int? accountId)
        {
            if (ModelState.IsValid)
            {
                // Initialize the relationship with the account
                var parentChild = new ParentChild
                {
                    AccountID = accountId ?? 0,
                    Child = child
                };

                db.ParentChilds.Add(parentChild);
                db.Children.Add(child);
                db.SaveChanges();
                return RedirectToAction("Details", "UserProfile", new { id = accountId });
            }


            ViewBag.LevelID = new SelectList(db.SupportLevels, "LevelID", "Name", child.LevelID);
            return View(child);
        }

        // GET: ChildProfile/Children/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Child child = db.Children.Find(id);
            if (child == null)
            {
                return HttpNotFound();
            }
            ViewBag.LevelID = new SelectList(db.SupportLevels, "LevelID", "Name", child.LevelID);
            var conditions = db.Conditions.ToList();
            conditions.ForEach(c=>c.isSelected = false);
            var childconditionIDs = db.ChildConditions.Select(x => x.ConditionID);
            var childconnditions = conditions.Where( c=> childconditionIDs.Contains(c.ConditionID) );
            childconnditions.ForEach(c=>c.isSelected = true);

            ViewBag.ConditionOptions = new SelectList(conditions, "Name", "ConditionID");
            return View(child);
        }

        // POST: ChildProfile/Children/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ChildID,Name,BirthDate,Verbal,Description,LevelID")] Child child)
        {
            if (ModelState.IsValid)
            {
                db.Entry(child).State = EntityState.Modified;
                db.SaveChanges();
               return RedirectToAction("Details", "UserProfile", new { id = child.ChildID});//how do i get tis to work TODO
            }
            ViewBag.LevelID = new SelectList(db.SupportLevels, "LevelID", "Name", child.LevelID);
            return View(child);
        }

        // GET: ChildProfile/Children/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Child child = db.Children.Find(id);
            if (child == null)
            {
                return HttpNotFound();
            }
            return View(child);
        }

        // POST: ChildProfile/Children/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Child child = db.Children.Find(id);
            db.Children.Remove(child);
            db.SaveChanges();
            return RedirectToAction("Details", "UserProfile", new { id = child.ChildID }); //todo
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
