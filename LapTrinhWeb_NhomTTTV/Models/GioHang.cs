using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LapTrinhWeb_NhomTTTV.Models;

namespace LapTrinhWeb_NhomTTTV.Models
{
    public class GioHang
    {
        QLLKDTDataContext data = new QLLKDTDataContext();
        public int iMasp { get; set; }
        public string sTensp { get; set; }
        public string sAnhbia { get; set; }
        public Double dDongia { get; set; }
        public int iSoluong { get; set; }
        public Double dThanhtien
        {
            get { return iSoluong * dDongia; }
        }

        //khoi tao gio hang theo masp duoc truyen vao voi soluong mac dinh la 1
        public GioHang(int Masp)
        {
            iMasp = Masp;
            Sanpham sanpham = data.Sanphams.Single(n => n.Masp == iMasp);
            sTensp = sanpham.Tensp;
            sAnhbia = sanpham.Anhbia;
            dDongia = double.Parse(sanpham.Giatien.ToString());
            iSoluong = 1;
        }
    }
}