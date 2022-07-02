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
    }
}