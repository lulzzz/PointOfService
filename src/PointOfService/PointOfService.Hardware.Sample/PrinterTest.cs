﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PointOfService;
using PointOfService.Hardware.Receipt;

namespace PointOfService.Hardware.Sample
{
    public class PrinterTest
    {
        public PrinterTest()
        {
            var printer = new Printer("PosPrinter");
            printer.Open();

            var receipt = BuildReceipt();

            receipt.Print(printer);

            //Console.WriteLine("Opening Drawer...");
            //printer.CashDrawerOpenCodes = new byte[] {27, 112, 0, 100, 250};
            //printer.OpenCashDrawer();

            //Console.WriteLine("Printing...");
            //printer.Device.TransactionPrint(PrinterStation.Receipt, PrinterTransactionControl.Transaction);
            //printer.Device.PrintNormal(PrinterStation.Receipt, "Test Receipt");
            //printer.Device.TransactionPrint(PrinterStation.Receipt, PrinterTransactionControl.Normal);

            //var b = new Bitmap
            //{
            //    Alignment = Alignment.Center,
            //    FileName = @"C:\Users\hinta\Desktop\Test.bmp"
            //};

            //printer.Execute(b);

            //printer.Execute(new Barcode
            //{
            //    Alignment = Alignment.Center,
            //    Symbology = BarCodeSymbology.Upca,
            //    Data = "609032551339",
            //    Height = 750,
            //    Width = 1500,
            //    TextPosition = BarCodeTextPosition.Below
            //});

            

            // CPL Lg = 28, M = 42, Sm = 56
            //printer.ExecuteAll(new List<Line>
            //{
            //    new Line
            //    {
            //        IsBold = true,
            //        Text = "This is bold text"
            //    },
            //    new Line
            //    {
            //        IsItalic = true,
            //        Text = "This is italic text",
            //        Alignment = Alignment.Center,
            //        CharactersPerLine = 56
            //    },
            //    new Line
            //    {
            //        IsUnderline = true,
            //        Text = "This is underline text",
            //        Alignment = Alignment.Right
            //    },
            //    new Line
            //    {
            //        IsBold = true,
            //        IsItalic = true,
            //        IsUnderline = true,
            //        Text = "This is all three",
            //        CharactersPerLine = 28
            //    }
            //});

            //printer.Execute(new FeedLines{Lines = 4});

            printer.Dispose();

            Console.ReadLine();
        }

        private static void PrintProperties(Printer printer)
        {
            var lines = new List<Line>();

            var t = printer.Device.GetType();
            var p = t.GetProperties();

            foreach (var propertyInfo in p)
            {
                lines.Add(new Line
                {
                    Alignment = Alignment.Left,
                    Text = $"{EscapeSequence.Bold()}{propertyInfo.Name}: {EscapeSequence.Normal}{Format(propertyInfo.GetValue(printer.Device))}",
                    CharactersPerLine = 56
                });
            }


            printer.ExecuteAll(lines);

            printer.Execute(new FeedAndPaperCut());
        }

        private static string Format(object obj)
        {
            var sb = new StringBuilder();

            if (obj != null)
            {
                sb.AppendLine(obj.ToString());
            }

            var array = obj as Array;

            if (array != null)
            {
                for (var i = 0; i < array.Length; i++)
                {
                    sb.Append("  [");
                    sb.Append(i);
                    sb.Append("]: ");
                    sb.AppendLine(array.GetValue(i).ToString());
                }
            }

            return sb.ToString();
        }

        private static Document BuildReceipt()
        {
            var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var logoPath = Path.Combine(binDirectory, "Logo.bmp");

            var document = new Document
            {
                Commands = new List<ICommand>
                {
                    new Bitmap
                    {
                        Alignment = Alignment.Center,
                        FileName = logoPath
                    },
                    new FeedUnits
                    {
                        Units = 250
                    },
                    new Line
                    {
                        Alignment = Alignment.Center,
                        Text = "123 MAIN ST, BOSTON, MA 01841"
                    },
                    new FeedUnits
                    {
                        Units = 250
                    },
                    new Line
                    {
                        IsBold = true,
                        IsUnderline = true,
                        Text = "Item Description                             Price     "
                    },
                    new Line
                    {
                        IsBold = true,
                        Text = "DAIRY"
                    },
                    new Line
                    {
                        CharactersPerLine = 42,
                        Text = "2% GAL MILK                                  $3.99     "
                    },
                    new FeedUnits
                    {
                        Units = 250
                    },
                    new Barcode
                    {
                        Alignment = Alignment.Center,
                        Data = "09231700130015342",
                        Height = 400,
                        Width = 1000,
                        Symbology = BarCodeSymbology.Code128,
                        TextPosition = BarCodeTextPosition.Below
                    },
                    new FeedAndPaperCut()
                }
            };

            return document;
        }
    }
}
