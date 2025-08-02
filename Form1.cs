using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Управление_ЭВМ
{
    public partial class Form1: Form

    {


        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();

        public string lastPrintedText = "";
        public Form1()
        {
            InitializeComponent();
            LoadLastPrintedText(); // Загрузка последнего текста для печати

            AllocConsole(); // Запускает консольное окно
            Task.Run(() => RunCommandLineInterface());

            // Включаем обработку нажатий клавиш
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown); // Явно добавляем обработчик

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit(); // Завершение программы при нажатии Esc
            }
            else
            {
                label3.Text = $"Вы нажали: {e.KeyCode}"; // Вывод нажатой клавиши
            }
        }

        // ✅ Возвращает фокус в окно после каждого ввода в консоли
        private void FocusOnForm()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.Activate()));
            }
            else
            {
                this.Activate();
            }
        }


        // Исправленный обработчик клавиш
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit(); // Завершение программы при нажатии Esc
            }
          
        }


        // Командный интерфейс (CLI)
        private void RunCommandLineInterface()
        {
            Console.WriteLine("Система управления ЭВМ");
            Console.WriteLine("Введите 'help' для списка команд.");

            while (true)
            {
                Console.Write("\n> ");
                string command = Console.ReadLine()?.Trim().ToLower();

                switch (command)
                {
                    case "help":
                        ShowHelp();
                        break;
                    case "status":
                        ShowSystemStatus();
                        break;
                    case "exit":
                        Console.WriteLine("Завершение программы...");
                        Application.Exit();
                        return;
                    default:
                        Console.WriteLine("Неизвестная команда. Введите 'help' для списка команд.");
                        break;
                }
            }
        }

        private void ShowSystemStatus()
        {
            Console.WriteLine("\nСостояние системы:");
            Console.WriteLine($"Процессор: {GetCPUUsage():F2}%");
            Console.WriteLine($"Свободная память: {GetAvailableMemory():F2} MB");
        }


        private void ShowHelp()
        {
            Console.WriteLine("\nДоступные команды:");
            Console.WriteLine("  help    - показать список команд");
            Console.WriteLine("  status  - вывести текущее состояние системы");
            Console.WriteLine("  exit    - выйти из программы");
        }




        // Загрузка последнего введённого текста
        private void LoadLastPrintedText()
        {
            if (File.Exists("last_print.txt"))
            {
                lastPrintedText = File.ReadAllText("last_print.txt");
                txtPrintText.Text = lastPrintedText;
            }
        }
        // Обработчик кнопки "Показать состояние системы"
        private void button1_Click(object sender, EventArgs e)
        {
          
            lblCPU.Text = $"Процессор: {GetCPUUsage()}%";
            lblRAM.Text = $"Свободная память: {GetAvailableMemory()} MB"; 
        }


        // Обработчик кнопки "Печать"
        private void button2_Click(object sender, EventArgs e)
        {
            string pdfFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Распечатанный_документ.pdf");

            PrintDocument printDocument = new PrintDocument();
            printDocument.PrinterSettings.PrinterName = "Microsoft Print to PDF";

            if (printDocument.PrinterSettings.PrinterName != "Microsoft Print to PDF")
            {
                MessageBox.Show("Принтер 'Microsoft Print to PDF' не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            printDocument.PrinterSettings.PrintToFile = true;
            printDocument.PrinterSettings.PrintFileName = pdfFilePath;

            printDocument.PrintPage += (s, ev) =>
            {
                ev.Graphics.DrawString(txtPrintText.Text, new Font("Arial", 12), Brushes.Black, 100, 100);
            };

            try
            {
                printDocument.Print();
                MessageBox.Show($"Документ сохранён в PDF: {pdfFilePath}", "Печать завершена", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Открываем PDF после печати
                Process.Start(new ProcessStartInfo(pdfFilePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка печати: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        // Получение загрузки CPU
        private float GetCPUUsage()
        {
            using (PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
            {
                cpuCounter.NextValue();
                System.Threading.Thread.Sleep(500); // Даем время для корректного считывания
                return cpuCounter.NextValue();
            }
        }
        // Получение свободной памяти
        private float GetAvailableMemory()
        {
            using (PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes"))
            {
                return ramCounter.NextValue();
            }
        }
        // Отслеживание нажатий клавиш
        private void button3_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Нажимайте клавиши. Для выхода нажмите 'Esc'.");
            this.KeyDown += new KeyEventHandler(Form1_KeyDown); // Убедимся, что обработчик активирован
            this.Focus(); // Принудительно вернуть фокус на форму
        }

        
        // Отслеживание движения мыши
        private void button4_Click_1(object sender, EventArgs e)
        {
            this.MouseMove += new MouseEventHandler(Form1_MouseMove);
            MessageBox.Show("Двигайте мышь. Для выхода закройте окно.");
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            label4.Text = $"X: {e.X}, Y: {e.Y}";
        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private bool IsInternetAvailable()
        {
            try
            {
                using (var client = new System.Net.WebClient())
                using (client.OpenRead("https://www.google.com/"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            if (IsInternetAvailable())
            {
                MessageBox.Show("Интернет доступен ✅", "Состояние сети", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Интернет недоступен ❌", "Состояние сети", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
    }
}
