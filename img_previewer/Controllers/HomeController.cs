using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using img_previewer.Models;
using System.Drawing;

namespace img_previewer.Controllers
{
    public class HomeController : Controller
    {
        //Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(UserAccount user)
        {
            using (TaskDbContext db = new TaskDbContext())
            {
                var usr = db.userAccount.Where(e => e.Email == user.Email && e.Password == user.Password).FirstOrDefault();
                if(usr != null)
                {
                    Session["UserId"] = usr.UserId.ToString();
                    Session["Email"] = usr.Email.ToString();
                    return RedirectToAction("LoggedIn");
                }
                else
                {
                    ModelState.AddModelError("", "UserName or Password is wrong.");
                }
            }
            return View();
        }

        public ActionResult LoggedIn(string filter = null, int page = 1, int pageSize = 20)
        {
            using (GalleryContext db = new GalleryContext())
            {
                var records = new PagedList<Photo>();
                ViewBag.filter = filter;

                records.Content = db.Photos
                            .Where(x => filter == null || (x.Description.Contains(filter)))
                            .OrderByDescending(x => x.PhotoId)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                // Count
                records.TotalRecords = db.Photos
                                .Where(x => filter == null || (x.Description.Contains(filter))).Count();

                records.CurrentPage = page;
                records.PageSize = pageSize;

                return View(records);
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserAccount account)
        {
            if(ModelState.IsValid)
            {
                using (TaskDbContext db = new TaskDbContext())
                {
                    var usr = db.userAccount.Where(e => e.Email == account.Email).FirstOrDefault();
                    if (usr != null)
                    {
                        Session["UserId"] = usr.UserId.ToString();
                        Session["Email"] = usr.Email.ToString();
                        ModelState.AddModelError("", "This email is already registered.");
                    }
                    else
                    {
                        db.userAccount.Add(account);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                //ModelState.Clear();
                //ViewBag.Message = account.UserName + " succesfully registered.";
                
            }
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            var photo = new Photo();
            return View(photo);
        }

        public Size NewImageSize(Size imageSize, Size newSize)
        {
            Size finalSize;
            double tempval;
            if (imageSize.Height > newSize.Height || imageSize.Width > newSize.Width)
            {
                if (imageSize.Height > imageSize.Width)
                    tempval = newSize.Height / (imageSize.Height * 1.0);
                else
                    tempval = newSize.Width / (imageSize.Width * 1.0);

                finalSize = new Size((int)(tempval * imageSize.Width), (int)(tempval * imageSize.Height));
            }
            else
                finalSize = imageSize; // image is already small size

            return finalSize;
        }

        private void SaveToFolder(Image img, string fileName, string extension, Size newSize, string pathToSave)
        {
            // Get new resolution
            Size imgSize = NewImageSize(img.Size, newSize);
            using (System.Drawing.Image newImg = new Bitmap(img, imgSize.Width, imgSize.Height))
            {
                newImg.Save(Server.MapPath(pathToSave), img.RawFormat);
            }
        }

        [HttpPost]
        public ActionResult Create(Photo photo, IEnumerable<HttpPostedFileBase> files)
        {
            if (!ModelState.IsValid)
                return View(photo);
            if (files.Count() == 0 || files.FirstOrDefault() == null)
            {
                ViewBag.error = "Please choose a file";
                return View(photo);
            }
            using (GalleryContext db = new GalleryContext())
            {
                var model = new Photo();
                foreach (var file in files)
                {
                    if (file.ContentLength == 0) continue;

                    model.Description = photo.Description;
                    var fileName = Guid.NewGuid().ToString();
                    var extension = System.IO.Path.GetExtension(file.FileName).ToLower();

                    using (var img = System.Drawing.Image.FromStream(file.InputStream))
                    {
                        model.ThumbPath = String.Format("/GalleryImages/thumbs/{0}{1}", fileName, extension);
                        model.ImagePath = String.Format("/GalleryImages/{0}{1}", fileName, extension);

                        // Save thumbnail size image, 100 x 100
                        SaveToFolder(img, fileName, extension, new Size(100, 100), model.ThumbPath);

                        // Save large size image, 800 x 800
                        SaveToFolder(img, fileName, extension, new Size(600, 600), model.ImagePath);
                    }

                    // Save record to database
                    model.CreatedOn = DateTime.Now;
                    db.Photos.Add(model);
                    db.SaveChanges();
                }

                return RedirectPermanent("/home/loggedin");
            }
        }
    }
}