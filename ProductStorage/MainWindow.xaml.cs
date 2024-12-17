﻿using ClosedXML.Excel;

using ProductStorage.Data;
using ProductStorage.Models;

using System.Windows;
using System.Windows.Input;

namespace ProductStorage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated(); // Bazani yaratish
            }
        }


        // Raqam kiritishni tekshirish
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !decimal.TryParse(e.Text, out _);
        }

        // Add Button
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBarCode.Text) ||
                string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtReminderAmount.Text) ||
                string.IsNullOrWhiteSpace(txtCostPrice.Text) ||
                string.IsNullOrWhiteSpace(txtSalePrice.Text))
            {
                MessageBox.Show("Barcha maydonlarni to'ldiring!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var db = new AppDbContext())
            {
                // BarCode takrorlanishini tekshirish
                if (db.Products.Any(p => p.BarCode == txtBarCode.Text))
                {
                    MessageBox.Show("Bu mahsulot bazada mavjud!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var product = new Product
                {
                    BarCode = txtBarCode.Text,
                    Name = txtName.Text,
                    ReminderAmount = int.Parse(txtReminderAmount.Text),
                    CostPrice = decimal.Parse(txtCostPrice.Text),
                    SalePrice = decimal.Parse(txtSalePrice.Text)
                };

                db.Products.Add(product);
                int result = db.SaveChanges();
                if (result > 0)
                    ClearInputs();
                else
                    MessageBox.Show("Xatolik bor !", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearInputs()
        {
            txtBarCode.Clear();
            txtName.Clear();
            txtReminderAmount.Clear();
            txtCostPrice.Clear();
            txtSalePrice.Clear();
        }

        private void btnGetAll_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var products = db.Products.ToList();

                if (products.Count == 0)
                {
                    MessageBox.Show("Mahsulotlar mavjud emas!", "Xabar", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Products");
                worksheet.Cell(1, 1).Value = "Id";
                worksheet.Cell(1, 2).Value = "BarCode";
                worksheet.Cell(1, 3).Value = "Name";
                worksheet.Cell(1, 4).Value = "ReminderAmount";
                worksheet.Cell(1, 5).Value = "CostPrice";
                worksheet.Cell(1, 6).Value = "SalePrice";

                for (int i = 0; i < products.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = products[i].Id;
                    worksheet.Cell(i + 2, 2).Value = products[i].BarCode;
                    worksheet.Cell(i + 2, 3).Value = products[i].Name;
                    worksheet.Cell(i + 2, 4).Value = products[i].ReminderAmount;
                    worksheet.Cell(i + 2, 5).Value = products[i].CostPrice;
                    worksheet.Cell(i + 2, 6).Value = products[i].SalePrice;
                }
                worksheet.Columns().AdjustToContents();

                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var filePath = System.IO.Path.Combine(desktopPath, "Products.xlsx");

                try
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath); 
                    }

                    workbook.SaveAs(filePath);

                    var fullPath = System.IO.Path.GetFullPath(filePath);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = fullPath,
                        UseShellExecute = true
                    });

                    MessageBox.Show($"Fayl muvaffaqiyatli yaratildi va ochildi: {fullPath}", "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    workbook.Dispose(); 
                }
            }
        }
    }
}