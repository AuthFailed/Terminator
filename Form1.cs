using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace New_app
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Метод для выдачи готового оповещения 
        static void Mess(string line1, string line2)
        {
            MessageBox.Show(line1, line2, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Метод удаления файла
        void Del(string line)
        {
            // При отсутствии файла выводим сообщение об ошибке и очищаем поля от информации
            if (File.Exists(line))
            {
                // При наличии файла спрашиваем у пользователя о его решении
                DialogResult result = MessageBox.Show("Вы уверены?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string filename = new FileInfo(TextBox.Text).Name;
                    try
                    {
                        KillProcessesAssociatedToFile(line);
                        File.Delete(line);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Не удалось удалить файл! \n Код ошибки скопирован в буфер обмена.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Clipboard.Clear();
                        Clipboard.SetText(ex.ToString());
                    }
                    if (!File.Exists(line))
                    {
                        // Если пользователь ответил согласием - удаляем файл и выводим оповещение об удалении
                        Mess($"Файл {filename} успешно удален", "");
                        TextBox.Text = null;
                        label1.Visible = false;
                        label2.Text = null;
                        button.Enabled = false;
                    }
                }
            }
            // Проверяем, а может это папка, а не файл?
            else if (Directory.Exists(line))
            {
                // Если все-таки находим папку по такому пути, то спрашиваем пользователя о его решении
                DialogResult result = MessageBox.Show("Вы уверены?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string foldername = new DirectoryInfo(TextBox.Text).Name;
                    Directory.Delete((line), true); //true - если директория не пуста удаляем все ее содержимое
                    if (!Directory.Exists(line))
                    {
                        // Если пользователь ответил согласием - удаляем папку и выводим оповещение об удалении
                        Mess($"Папка {foldername} успешно удалена", "");
                        TextBox.Text = null;
                        label1.Visible = false;
                        label2.Text = null;
                        button.Enabled = false;
                    }
                }
            }
            else
            {
                // Если это и не файл, и не папка, то выдаем сообщение об ошибке и очищаем поля
                MessageBox.Show("По этому пути ничего не найдено!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button.Enabled = false;
                TextBox.Text = null;
                label2.Visible = false;
                label2.Text = null;
            }
        }

        // Добавляем плавность открытия и свойства контролов
        async void Form1_Load(object sender, EventArgs e)
        {
            label1.Visible = false;
            for (Opacity = 0; Opacity < .95; Opacity += .03)
            {
                await Task.Delay(5).ConfigureAwait(false);
            }
        }

        // Диалоговое окно с выбором файла и запись пути в textbox
        void Button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog myFile = new OpenFileDialog
            {
                Title = "Выбор файла"
            };
            if (myFile.ShowDialog() == DialogResult.OK)
            {
                TextBox.Text = Convert.ToString(myFile.FileName);
            }
        }

        //Диалоговое окно с выбором папки
        void Button2_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string folderName = folderBrowserDialog1.SelectedPath;
                TextBox.Text = folderName;
            }
        }

        //Информация о файле при вставке пути на файл вручную
        void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox.Text = TextBox.Text.Trim();
            TextBox.Text = TextBox.Text.Trim('"');
            // Проверка на наличие чего-либо в textbox
            if (TextBox.Text != "")
            {
                // Проверяем файл на существование и выдаем о нем информацию информации
                if (File.Exists(TextBox.Text))
                {
                    button.Enabled = true;
                    label1.Visible = true;
                    label1.Text = "Информация о файле:";
                    label2.Text = $"Создан: " + File.GetCreationTime(TextBox.Text) +
                                   "\n Открывался: " + File.GetLastAccessTime(TextBox.Text) +
                                   "\n Изменялся: " + File.GetLastWriteTime(TextBox.Text) +
                                   "\n Размер файла: " + (Math.Round(Convert.ToDouble(File.ReadAllBytes(TextBox.Text).Length) / 1048576.0, 2)) + " мбайт";
                }
                // Если не находим файла, то пробуем найти папку по этому пути. Находим - выдаем информацию о папке
                else if (Directory.Exists(TextBox.Text))
                {
                    button.Enabled = true;
                    label1.Visible = true;
                    label1.Text = "Информация о папке:";
                    DirectoryInfo FolderInfo = new DirectoryInfo(TextBox.Text);
                    label2.Text = "Создана: " + FolderInfo.CreationTime.ToString().ToLower();
                }
                else //Если ничего не найдено по введеному пути - очищаем все поля программы
                {
                    label1.Visible = false;
                    label2.Text = null;
                    button.Enabled = false;
                }
            }
            //Если textbox стал пустым - скрываем лишнюю информацию
            else
            {
                label1.Visible = false;
                label2.Text = null;
                button.Enabled = false;
            }
        }
        
        // Вызываем метод удаления по нажатию на кнопку
        void Button_Click(object sender, EventArgs e)
        {
            try //Запускаем обработчик исключений
            {
                Del(TextBox.Text); // Задаем методу путь до файла/папки
            }
            catch(Exception ex) // Если в методе Del() происходит исключение, то выводим следующий текст
            {
                MessageBox.Show("Код ошибки скопирован в буфер. \n Отправьте его разработчику", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Clipboard.Clear();                          //Очищаем буфер обмена чтобы избежать ошибки
                Clipboard.SetText(ex.ToString());           //Добавляем ошибку в буфер для дальнейшей вставки
            }
        }

        // Меняем окантовку кнопки в зависимости от того поставлена на нее мышка или нет
        void Button_MouseEnter(object sender, EventArgs e)
        {
            if (button.Enabled == true)
            {
                button.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            }
            else
            {
                button.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            }
        }

        // Убиваем процессы, ассоциированные с нашим файлом
         static public void KillProcessesAssociatedToFile(string file)
        {
            GetProcessesAssociatedToFile(file).ForEach(p =>
            {
                p.Kill();
                p.WaitForExit(10000);
            });
        }

        // Получаем список процессов которые имеют прямое отношение к удаляемому файлу
        static public List<Process> GetProcessesAssociatedToFile(string file)
        {
            return Process.GetProcesses()
                .Where(p => !p.HasExited
                    && p.Modules.Cast<ProcessModule>().ToList()
                        .Exists(y => y.FileName.ToLowerInvariant() == file.ToLowerInvariant())
                    ).ToList();
        }
    }
}