using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LapTrinhWeb_NhomTTTV.Models;

namespace LapTrinhWeb_NhomTTTV.Controllers
{
    public class UserController : Controller
    {
        QLLKDTDataContext data = new QLLKDTDataContext();
        // GET: User
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(FormCollection collection, Nguoidung nd)
        {
            var hoten = collection["HotenKH"];
            var email = collection["Email"];
            var matkhau = MaHoa.GetMD5(collection["Matkhau"]);
            var diachi = collection["Diachi"];
            var dienthoai = collection["Dienthoai"];
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Họ tên khách hàng không được để trống*";
            }
            if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi5"] = "Email không được bỏ trống*";
            }

            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = "Phải nhập mật khẩu*";
            }

            if (String.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi6"] = "Phải nhập số điện thoại*";
            }
            else
            {
                //Gán giá trị cho đối tượng được tạo mới (KH)
                nd.Hoten = hoten;
                nd.Email = email;
                nd.Matkhau = MaHoa.GetMD5(matkhau);
                nd.Diachi = diachi;
                nd.Dienthoai = dienthoai;
                data.Nguoidungs.InsertOnSubmit(nd);
                data.SubmitChanges();
                return RedirectToAction("Dangnhap");
            }
            return this.DangKy();
        }

        [HttpGet]
        public ActionResult Dangnhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangnhap(FormCollection collection)
        {
            //Gan cac gia tri nguoi dung nhap lieu cho cac bien
            var email = collection["Email"];
            var matkhau = MaHoa.GetMD5(collection["Matkhau"]);
            if (string.IsNullOrEmpty(email))
            {
                ViewData["Loi1"] = "Phải nhập tên đang nhập*";
            }
            else if (string.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu*";
            }
            else
            {
                //Gán giá trị cho đối tượng được tạo mới (KH)
                Nguoidung nd = data.Nguoidungs.SingleOrDefault(n => n.Email == email && n.Matkhau == MaHoa.GetMD5(matkhau));
                if (nd != null)
                {
                    //ViewBag.Thongbao = "Chúc mừng đăng nhập thành công";
                    Session["User"] = nd.Hoten;
                    Session["Taikhoan"] = nd;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            return View();
        }

        public ActionResult Dangxuat()
        {
            Session["User"] = null;
            Session["Taikhoan"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}