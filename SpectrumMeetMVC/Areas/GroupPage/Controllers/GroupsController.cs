﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SpectrumMeetEF;
using SpectrumMeetMVC.Models;

namespace SpectrumMeetMVC.Areas.GroupPage.Controllers
{
    public class GroupsController : Controller
    {
        private readonly SpectrumMeetEntities db = new SpectrumMeetEntities();

        // GET: GroupPage/Groups
        public ActionResult Index()
        {
            var groups = db.Groups.Include(g => g.Condition);
            return View(groups.ToList());
        }

        // GET: GroupPage/Groups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups
                
            .Include(x => x.Condition)
            .Include(x => x.GroupMembers)
            .Include(x => x.Messages.Select(m => m.User))
            .Include(x => x.Messages)
            .FirstOrDefault(g => g.GroupID == (int)id);
                
            string detailsUrl = Url.Action("Details", "Groups", new { id = group.GroupID });
            ViewBag.DetailsUrl = detailsUrl;
            ViewBag.GroupName = group.Name;

            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // GET: GroupPage/Groups/Create
        public ActionResult Create()
        {
            ViewBag.ConditionID = new SelectList(db.Conditions, "ConditionID", "Name");
            return View();
        }

        // POST: GroupPage/Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupID,ConditionID,Name")] Group group)
        {
            if (ModelState.IsValid)
            {
                db.Groups.Add(group);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ConditionID = new SelectList(db.Conditions, "ConditionID", "Name", group.ConditionID);
            return View(group);
        }

        // GET: GroupPage/Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            ViewBag.ConditionID = new SelectList(db.Conditions, "ConditionID", "Name", group.ConditionID);
            return View(group);
        }

        // POST: GroupPage/Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupID,ConditionID,Name")] Group group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ConditionID = new SelectList(db.Conditions, "ConditionID", "Name", group.ConditionID);
            return View(group);
        }

        // GET: GroupPage/Groups/Delete/5
        public ActionResult Delete(int? id) // TODO: how to make only creator to be able to delete reference user class 
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        [HttpPost,ActionName("PostMessage")]
        [ValidateAntiForgeryToken]
        public ActionResult PostMessage(int groupId, string messageSubject, string messageContent)
        {
            if (ModelState.IsValid)
            {
                var group = db.Groups.Find(groupId);
                if (Session["AccountID"] == null)
                {
                    TempData["ErrorMessage"] = "You must be logged in to post messages!";
                    return RedirectToAction("Details", new { id = groupId});
                }
                if (group == null)
                {
                    return HttpNotFound();
                }
                if ( messageSubject == null)
                {
                    TempData["ErrorMessage"] = "Please enter a subject!";
                    return RedirectToAction("Details", new {id = groupId});
                }
                var message = new Message
                {
                    GroupID = groupId,
                    Title = messageSubject,
                    Content = messageContent,
                    PostedDate = DateTime.Now,
                    AccountID = (int)Session["AccountID"]
                };

                db.Messages.Add(message);
                db.SaveChanges();

                return RedirectToAction("Details", new { id = groupId });
            }


            return RedirectToAction("Details", new { id = groupId });
        }

            // POST: GroupPage/Groups/Delete/5
            [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
            db.SaveChanges();
            return RedirectToAction("Index");
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
