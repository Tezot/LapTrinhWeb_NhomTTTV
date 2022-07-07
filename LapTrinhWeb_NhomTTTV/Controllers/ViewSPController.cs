using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LapTrinhWeb_NhomTTTV.Models;

namespace LapTrinhWeb_NhomTTTV.Controllers
{
    public class ViewSPController : Controller
    {
        QLLKDTDataContext data = new QLLKDTDataContext();
        // GET: ViewSP
        public ActionResult dtCPU()
        {
            var test = from sp in data.Sanphams where sp.Maloaisp == 1  select sp;
            return PartialView(test);
        }
        public ActionResult dtGPU()
        {
            var test = from sp in data.Sanphams where sp.Maloaisp == 2 select sp;
            return PartialView(test);
        }
        public ActionResult dtRAM()
        {
            var test = from sp in data.Sanphams where sp.Maloaisp == 3 select sp;
            return PartialView(test);
        }
        public ActionResult dtSSD()
        {
            var test = from sp in data.Sanphams where sp.Maloaisp == 4 select sp;
            return PartialView(test);
        }

        public ActionResult dtHHD()
        {
            var test = from sp in data.Sanphams where sp.Maloaisp == 5 select sp;
            return PartialView(test);
        }

        public ActionResult dtNguon()
        {
            var test = from sp in data.Sanphams where sp.Maloaisp == 6 select sp;
            return PartialView(test);
        }

        public ActionResult dtMain()
        {
            var test = from sp in data.Sanphams where sp.Maloaisp == 7 select sp;
            return PartialView(test);
        }

        public ActionResult dtCase()
        {
            var test = from sp in data.Sanphams where sp.Maloaisp == 8 select sp;
            return PartialView(test);
        }

        public ActionResult dtFan()
        {
            var test = from sp in data.Sanphams where sp.Maloaisp == 9 select sp;
            return PartialView(test);
        }
        public ActionResult chitiet(int Masp)
        {
            var sanpham = from s in data.Sanphams
                          where s.Masp == Masp
                          select s;
            return View(sanpham.Single());
        }
    }
}