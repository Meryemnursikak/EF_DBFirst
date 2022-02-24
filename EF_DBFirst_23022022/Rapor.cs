using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EF_DBFirst_23022022
{
    public partial class Rapor : Form
    {
        public Rapor()
        {
            InitializeComponent();
        }

        KuzeyYeliEntities ctx = new KuzeyYeliEntities();
        private void Rapor_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = ctx.Urunlers.ToList();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //            select u.UrunAdi,
            //sum(sd.Fiyat * sd.Adet) as SatisTutari,
            //sum(sd.Adet) SatisAdedi
            //from Urunler u
            //inner join SatisDetay sd
            //on u.UrunID = sd.UrunID
            //group by u.UrunAdi
            /*hangi üründen toplamda 
kaç dolarlık ve kaç adetlik 
satış yapılmıştır*/

            dataGridView1.DataSource = ctx.Urunlers.Join(ctx.SatisDetays,
               u => u.UrunID,
               s => s.UrunID,
               (u, s) => new { u.UrunAdi, s.Fiyat, s.Adet }
               ).GroupBy(x => x.UrunAdi).Select(z => new
               {
                   UrunAdı=z.Key,
                   ToplamSatisTutari = z.Sum(y => y.Adet * y.Fiyat),
                   ToplamSatisAdedi = z.Sum(y => y.Adet)
               }).ToList();        

    }

        private void button2_Click(object sender, EventArgs e)
        {
            /* Hangi Tedarikçiden hangi kategoriden
toplam kac dolarlık ve toplam kaç adet
satış yapılmıştır.
*/

            dataGridView1.DataSource = ctx.Urunlers.Join(
                ctx.SatisDetays,
                u => u.UrunID,
                s => s.UrunID,
                (urn, sd) => new
                {
                    urn.Tedarikciler,
                    urn.Kategoriler,
                    sd.Fiyat,
                    sd.Adet
                }).GroupBy(x => new { x.Kategoriler.KategoriAdi, x.Tedarikciler.SirketAdi }).Select(y =>
                   new
                   {
                       y.Key.SirketAdi,
                       y.Key.KategoriAdi,
                       ToplamFiyat = y.Sum(z =>z.Adet * z.Fiyat),
                       ToplamAdet = y.Sum(z => z.Adet)
                   }).ToList();

            dataGridView1.Columns["ToplamFiyat"].HeaderText = "Toplam Tutar";
//            select t.SirketAdi,k.KategoriAdi,
//sum(s.Adet * s.Fiyat) ToplamSatış,
//sum(s.Adet) ToplamAdet
//from Urunler u
//inner join Tedarikciler t
//on u.TedarikciID = t.TedarikciID
//inner join Kategoriler k
//on k.KategoriID = u.KategoriID
//inner join SatisDetay s
//on s.UrunID = u.UrunID
//group by t.SirketAdi,k.KategoriAdi
        }
    }
}
