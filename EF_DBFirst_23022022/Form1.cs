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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
             
        KuzeyYeliEntities ctx = new KuzeyYeliEntities();
        private void Form1_Load(object sender, EventArgs e)
        {


            //dataGridView1.DataSource = ctx.Urunlers.ToList();
            urunlistele();
            cmbKategori.DataSource = ctx.Kategorilers.ToList();
            cmbKategori.DisplayMember = "KategoriAdi";
            cmbKategori.ValueMember = "KategoriID";

            cmbTedarikci.DataSource = ctx.Tedarikcilers.ToList();
            cmbTedarikci.DisplayMember = "SirketAdi";
            cmbTedarikci.ValueMember = "TedarikciID";


        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            Urunler u = new Urunler();
            u.UrunAdi = txtUrunAd.Text;
            u.Fiyat = nudFiyat.Value;
            u.Stok =(short)nudStok.Value;
            u.KategoriID =(int)cmbKategori.SelectedValue;
            u.TedarikciID =(int)cmbTedarikci.SelectedValue;
            
            ctx.Urunlers.Add(u);
            ctx.SaveChanges();
            dataGridView1.DataSource = ctx.Urunlers.ToList();
        }

        void urunlistele()
        {
            var urunler = ctx.Urunlers.Join(
                ctx.Kategorilers,
                u => u.KategoriID,
                k => k.KategoriID,
                (urn, ktg) => new
                {
                    urn.UrunID,
                    urn.UrunAdi,
                    urn.Fiyat,
                    urn.Stok,
                    ktg.KategoriAdi,
                }).ToList();

            var urunler2 = ctx.Urunlers.Join(
                ctx.Kategorilers,
                u => u.KategoriID,
                k => k.KategoriID,
                (urn, ktg) => new
                {
                    urn,
                    ktg
                }).Join(ctx.Tedarikcilers,
                uk => uk.urn.TedarikciID,
                t => t.TedarikciID,
                (urun, ted) => new
                {
                    urun.urn.UrunID,
                    urun.urn.UrunAdi,
                    urun.urn.Fiyat,
                    urun.urn.Stok,
                    urun.ktg.KategoriAdi,
                    urun.ktg.KategoriID,
                    urun.urn.Sonlandi,
                    ted.TedarikciID,
                    ted.SirketAdi
                });

            if(radioButton1.Checked)

            dataGridView1.DataSource = urunler2.OrderBy(x=>x.Fiyat).Where(x=>x.Sonlandi==false).ToList();

            dataGridView1.Columns["UrunID"].Visible = false;
            dataGridView1.Columns["TedarikciID"].Visible = false;
            dataGridView1.Columns["KategoriID"].Visible = false;
            dataGridView1.Columns["Sonlandi"].Visible = false;

            if (radioButton2.Checked)
            {
                dataGridView1.DataSource = urunler2.OrderByDescending(x => x.Fiyat).ToList();
                dataGridView1.Columns["UrunID"].Visible = false;
                dataGridView1.Columns["TedarikciID"].Visible = false;
                dataGridView1.Columns["KategoriID"].Visible = false;

            }
            
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            urunlistele();
        }

        private void txtAra_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = ctx.Urunlers.Where(x => x.UrunAdi.Contains(txtAra.Text) & x.Sonlandi==false).ToList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = ctx.Urunlers.Take(10).ToList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Rapor r = new Rapor();
            r.Show();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           DataGridViewRow row=dataGridView1.CurrentRow;
            txtUrunAd.Text = row.Cells["UrunAdi"].Value.ToString();
            nudFiyat.Value =(decimal)row.Cells["Fiyat"].Value;
            nudStok.Value =(short) row.Cells["Stok"].Value;
            cmbKategori.SelectedValue = row.Cells["KategoriID"].Value;
            cmbTedarikci.SelectedValue = row.Cells["TedarikciID"].Value;

            txtUrunAd.Tag = row.Cells["UrunID"].Value;

        }

        private void silToolStripMenuItem_Click(object sender, EventArgs e)
        {
          DialogResult cevap=MessageBox.Show("Seçilen Kayıt Silinsin mi?", "Kayıt Silme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(cevap==DialogResult.Yes)
            {
               int id=(int)dataGridView1.CurrentRow.Cells["UrunID"].Value;

               Urunler u=ctx.Urunlers.FirstOrDefault(x => x.UrunID == id);

                ctx.Urunlers.Remove(u);
                ctx.SaveChanges();
                urunlistele();

            }

        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            int id =(int)txtUrunAd.Tag;
            Urunler u=ctx.Urunlers.FirstOrDefault(x => x.UrunID == id);

            u.UrunAdi = txtUrunAd.Text;
            u.Fiyat = nudFiyat.Value;
            u.Stok =(short)nudStok.Value;
            u.KategoriID =(int)cmbKategori.SelectedValue;
            u.TedarikciID =(int)cmbTedarikci.SelectedValue;

            ctx.SaveChanges();
            urunlistele();


        }
    }
}
