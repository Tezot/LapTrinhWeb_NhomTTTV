using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LapTrinhWeb_NhomTTTV.Models;
using PagedList;

namespace LapTrinhWeb_NhomTTTV.Controllers
{
    public class HomeController : Controller
    {
        QLLKDTDataContext data = new QLLKDTDataContext();
        private List<Sanpham> Laysanpham(int count)
        {
            return data.Sanphams.OrderByDescending(a => a.Masp).Take(count).ToList();
        }
        public ActionResult Index(int? page)
        {
            int pageSize = 6;
            //tao bien so trang
            int pageNum = (page ?? 1);
            //Lay top 5 san pham ban chay nhat
            var sanphammoi = Laysanpham(20);
            return View(sanphammoi.ToPagedList(pageNum, pageSize));
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult KetQuaTimKiem(FormCollection f)
        {

            if (f["txtTimKiem"] == null)
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
                string sTuKhoa = f["txtTimKiem"].ToString();
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
    }
}