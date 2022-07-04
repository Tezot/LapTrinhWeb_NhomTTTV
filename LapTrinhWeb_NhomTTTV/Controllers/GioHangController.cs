using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LapTrinhWeb_NhomTTTV.Models;

namespace LapTrinhWeb_NhomTTTV.Controllers
{
    public class GioHangController : Controller
    {
        QLLKDTDataContext data = new QLLKDTDataContext();
        // GET: GioHang
        public List<GioHang> Laygiohang()
        {
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang == null)
            {
                //Neu gio hang chua ton tai thi khoi tao listGiohang
                lstGiohang = new List<GioHang>();
                Session["GioHang"] = lstGiohang;
            }
            return lstGiohang;
        }

        public ActionResult ThemGiohang(int iMasp, string strURL)
        {
            //Lay ra Session gio hang
            List<GioHang> lstGiohang = Laygiohang();
            //Kiem tra sp nay ton tai trong Session["Giohang"] chua?
            GioHang sanpham = lstGiohang.Find(n => n.iMasp == iMasp);
            if (sanpham == null)
            {
                sanpham = new GioHang(iMasp);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }

        //Tong so luong
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }

        //Tinh tong tien
        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }

        //Xay dung trang gio hang
        public ActionResult GioHang()
        {
            List<GioHang> lstGiohang = Laygiohang();
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.TongsoLuong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }

        public ActionResult GiohangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }
        public ActionResult XoaGiohang(int iMasp)
        {
            //Lay gio hang tu session
            List<GioHang> lstGiohang = Laygiohang();
            //Kiem tra san pham da co trong Session["Giohang"]
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.iMasp == iMasp);
            //Neu ton tai thi cho sua Soluong
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iMasp == iMasp);
                return RedirectToAction("GioHang");
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("GioHang");
        }

        //Cap nhat gio hang
        public ActionResult CapnhatGiohang(int iMasp, FormCollection f)
        {
            //Lay gio hang tu session
            List<GioHang> lstGiohang = Laygiohang();
            //Kiem tra san pham da co trong session["Giohang"]
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.iMasp == iMasp);
            //Neu ton tai thi cho sua Soluong
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult XoaTatcaGiohang()
        {
            //Lay gio hang tu session
            List<GioHang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "Home");
        }

        //Hien thi view Dathang de cap nhat cac thong tin cho don hang
        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "User");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //Lay gi hang tu session
            List<GioHang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();

            return View(lstGiohang);
        }

        public ActionResult DatHang(FormCollection collection)
        {
            //Them don hang
            Donhang ddh = new Donhang();
            Nguoidung kh = (Nguoidung)Session["Taikhoan"];
            List<GioHang> gh = Laygiohang();
            ddh.MaNguoidung = kh.MaNguoiDung;
            ddh.Ngaydat = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["Ngaygiao"]);
            ddh.Ngaygiao = DateTime.Parse(ngaygiao);
            ddh.Tinhtrang = 0;
            //ddh.Dathanhtoan = false;
            data.Donhangs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            //Them chi tiet don hang
            foreach (var item in gh)
            {
                Chitietdonhang ctdh = new Chitietdonhang();
                ctdh.Madon = ddh.Madon;
                ctdh.Masp = item.iMasp;
                ctdh.Soluong = item.iSoluong;
                ctdh.Dongia = (decimal)item.dDongia;
                data.Chitietdonhangs.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();
            Session["GioHang"] = null;
            return RedirectToAction("Xacnhandonhang", "GioHang");
        }

        public ActionResult Xacnhandonhang()
        {
            return View();
        }
    }
}
