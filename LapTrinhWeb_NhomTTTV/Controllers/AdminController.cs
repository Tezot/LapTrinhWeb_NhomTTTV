using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LapTrinhWeb_NhomTTTV.Models;
using PagedList;

namespace LapTrinhWeb_NhomTTTV.Controllers
{
    public class AdminController : Controller
    {
        QLLKDTDataContext data = new QLLKDTDataContext();
        // GET: Admin
        public ActionResult KetQuaTimKiem(FormCollection collection)
        {

            if (collection["txtTimKiem"] == null)
            {

                List<Sanpham> lstKQTK = data.Sanphams.Where(n => n.Tensp.Contains((string)Session["TuKhoa"])).ToList();

                if (lstKQTK.Count == 0)
                {
                    ViewBag.ThongBaoTimKiem = "Không tìm thấy sản phẩm nào";
                    return View(data.Sanphams.OrderBy(n => n.Tensp));
                }
                ViewBag.ThongBaoTimKiem = "Đã tìm thấy " + lstKQTK.Count + " kết quả !";
                return View(lstKQTK.OrderBy(n => n.Tensp));
            }
            else
            {
                string sTuKhoa = collection["txtTimKiem"].ToString();
                Session["TuKhoa"] = sTuKhoa;
                List<Sanpham> lstKQTK = data.Sanphams.Where(n => n.Tensp.Contains(sTuKhoa)).ToList();

                if (lstKQTK.Count == 0)
                {
                    ViewBag.ThongBaoTimKiem = "Không tìm thấy sản phẩm nào";
                    return View(data.Sanphams.OrderBy(n => n.Tensp));
                }
                ViewBag.ThongBaoTimKiem = "Đã tìm thấy " + lstKQTK.Count + " kết quả !";
                return View(lstKQTK.OrderBy(n => n.Tensp));
            }
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            //Gán các giá trị người dùng nhập liệu cho các biến
            var tendn = collection["name"];
            var matkhau = collection["pass"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                //Gán giá trị cho đối tượng được tạo mới (ad)
                Admin ad = data.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Sanpham", "Admin");
                }
                else
                {
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng*";
                }
            }
            return View();
        }
        public ActionResult Sanpham(int? page)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            var list = data.Sanphams.OrderByDescending(s => s.Masp).ToList();
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ChitietSP(int id)
        {
            Sanpham tbsanpham = data.Sanphams.SingleOrDefault(n => n.Masp == id);
            ViewBag.MaSP = tbsanpham.Masp;
            if (tbsanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(tbsanpham);
        }

        //----------------------------------- Sản Phẩm ------------------------------------
        //Thêm mới sản phẩm
        [HttpGet]
        public ActionResult Themmoisanpham()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            ViewBag.MaLoaiSP = new SelectList(data.Loaisps.ToList().OrderBy(n => n.Maloaisp), "Maloaisp", "Tenloaisp");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themmoisanpham(Sanpham sp, FormCollection collection, HttpPostedFileBase fileupload)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            //Đưa dữ liệu vào dropdownload
            ViewBag.MaLoaiSP = new SelectList(data.Loaisps.ToList().OrderBy(n => n.Maloaisp), "Maloaisp", "Tenloaisp");

            var Tensp = collection["Ten"];
            var gia = collection["Gia"];
            var SoLuong = collection["SoLuong"];
            var Mota = collection["textarea"];
            //var Date = collection["Date"];
            var loai = collection["Maloaisp"];


            //Kiểm tra đường dẫn file
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            //Thêm vào CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //Lưu tên file, bổ sung thư viện using System.IO;
                    var fileName = Path.GetFileName(fileupload.FileName);
                    //Lưu đường dẫn của file
                    var path = Path.Combine(Server.MapPath("~/img"), fileName);
                    //Kiểm tra hình ảnh tồn tại chưa?
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        //Lưu hình ảnh vào đường dẫn
                        fileupload.SaveAs(path);
                    }
                    sp.Anhbia = fileName;
                    //Lưu vào CSDL
                    sp.Tensp = Tensp;
                    sp.Giatien = decimal.Parse(gia);
                    sp.Mota = Mota;
                    //sp.Ngaycapnhat = DateTime.Parse(Date);
                    sp.Maloaisp = Int32.Parse(loai);
                    sp.Anhbia = fileName;
                    data.Sanphams.InsertOnSubmit(sp);
                    data.SubmitChanges();
                }
                return RedirectToAction("Sanpham", "Admin");
            }
        }
        //Chỉnh sửa sản phẩm
        [HttpGet]
        public ActionResult Suasanpham(int id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            Sanpham sp = data.Sanphams.SingleOrDefault(n => n.Masp == id);
            
            ViewBag.Maloaisp = new SelectList(data.Loaisps.ToList().OrderBy(n => n.Maloaisp), "Maloaisp", "Tenloaisp", sp.Maloaisp);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        [HttpPost, ActionName("Suasanpham")]
        //[ValidateInput(false)]
        public ActionResult XacNhanSuasanpham(FormCollection collection, int id, HttpPostedFileBase fileupload)
        {
            var img = "";
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            if (fileupload != null)
            {
                img = Path.GetFileName(fileupload.FileName);
                var path = Path.Combine(Server.MapPath("~/img"), img);
                if (!System.IO.File.Exists(path))//Sản Phẩm Chưa Tồn Tại
                {
                    fileupload.SaveAs(path);
                }
            }
            else
            {
                img = collection["Anh"];
            }
            Sanpham sp = data.Sanphams.SingleOrDefault(n => n.Masp == id);
            sp.Anhbia = img;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            UpdateModel(sp);
            data.SubmitChanges();
            return RedirectToAction("Sanpham");
        }
        //Xoá sản phẩm
        [HttpGet]
        public ActionResult Xoasanpham(int id)
        {
            //Lấy ra 1 sản phẩm cần xoá
            Sanpham sanpham = data.Sanphams.SingleOrDefault(n => n.Masp == id);
            ViewBag.Maloaisp = sanpham.Maloaisp;
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sanpham);
        }

        [HttpPost, ActionName("Xoasanpham")]
        public ActionResult Xacnhanxoa(int id)
        {
            Sanpham sanpham = data.Sanphams.SingleOrDefault(n => n.Masp == id);
            ViewBag.MaSP = sanpham.Maloaisp;
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.Sanphams.DeleteOnSubmit(sanpham);
            data.SubmitChanges();
            return RedirectToAction("Sanpham", "Admin");
        }

        //----------------------------------- Loại Sản Phẩm ------------------------------------
        public ActionResult DSloaisp(int? page)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            int pagesize = 10;
            int pageNum = (page ?? 1);
            var list = data.Loaisps.OrderBy(s => s.Maloaisp).ToList();
            return View(list.ToPagedList(pageNum, pagesize));
        }
        [HttpGet]
        public ActionResult Themlsp()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult Themlsp(Loaisp lsp, FormCollection collection)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            var Tenlsp = collection["Ten"];

            lsp.Tenloaisp = Tenlsp;

            data.Loaisps.InsertOnSubmit(lsp);
            data.SubmitChanges();
            return RedirectToAction("DSloaisp", "Admin");

        }
        public ActionResult Sualsp(int id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                Loaisp lsp = data.Loaisps.SingleOrDefault(n => n.Maloaisp == id);
                if (lsp == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(lsp);
            }
        }

        [HttpPost, ActionName("Sualsp")]
        public ActionResult XacNhanSuaLSP(FormCollection collection, int id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            Loaisp lsp = data.Loaisps.SingleOrDefault(n => n.Maloaisp == id);
            if (lsp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            UpdateModel(lsp);
            data.SubmitChanges();
            return RedirectToAction("DSloaisp");
        }
        [HttpGet]
        public ActionResult Xoalsp(int id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                Loaisp lsp = data.Loaisps.SingleOrDefault(n => n.Maloaisp == id);
                ViewBag.MaloaiSP = lsp.Maloaisp;
                if (lsp == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(lsp);
            }
        }
        [HttpPost, ActionName("Xoalsp")]
        public ActionResult XacNhanXoaLSP(int id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                Loaisp lsp = data.Loaisps.SingleOrDefault(n => n.Maloaisp == id);
                ViewBag.MaLoaiSP = lsp.Maloaisp;
                if (lsp == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                data.Loaisps.DeleteOnSubmit(lsp);
                data.SubmitChanges();
                return RedirectToAction("DSloaisp");
            }
        }
    }
}